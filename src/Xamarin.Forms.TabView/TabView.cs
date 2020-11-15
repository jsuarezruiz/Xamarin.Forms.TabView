﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms.TabView
{
    [ContentProperty(nameof(TabItems))]
    public class TabView : ContentView, IDisposable
    {
        const uint TabIndicatorAnimationDuration = 100;

        readonly Grid _mainContainer;
        readonly Grid _tabStripContainer;
        readonly Grid _tabStripBackground;
        readonly ScrollView _tabStripContainerScroll;
        readonly Grid _tabStripIndicator;
        readonly Grid _tabStripContent;
        readonly Grid _tabStripContentContainer;
        readonly CarouselView _contentContainer;

        readonly List<double> _contentWidthCollection;
        IList _tabItemsSource;
        ObservableCollection<TabViewItem> _contentTabItems;

        public TabView()
        {
            TabItems = new ObservableCollection<TabViewItem>();

            _contentWidthCollection = new List<double>();

            BatchBegin();

            _tabStripBackground = new Grid
            {
                BackgroundColor = TabStripBackgroundColor,
                HeightRequest = TabStripHeight,
                VerticalOptions = LayoutOptions.Start
            };

            _tabStripIndicator = new Grid
            {
                BackgroundColor = TabIndicatorColor,
                HeightRequest = TabIndicatorHeight,
                HorizontalOptions = LayoutOptions.Start
            };

            UpdateTabIndicatorPlacement(TabIndicatorPlacement);

            _tabStripContent = new Grid
            {
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                ColumnSpacing = 0
            };

            _tabStripContentContainer = new Grid
            {
                BackgroundColor = Color.Transparent,
                Children = { _tabStripIndicator, _tabStripContent },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start
            };

            _tabStripContainerScroll = new ScrollView()
            {
                BackgroundColor = Color.Transparent,
                Orientation = ScrollOrientation.Horizontal,
                Content = _tabStripContentContainer,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start
            };

            if (Device.RuntimePlatform == Device.macOS || Device.RuntimePlatform == Device.UWP)
                _tabStripContainerScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Default;

            _tabStripContainer = new Grid
            {
                BackgroundColor = Color.Transparent,
                Children = { _tabStripBackground, _tabStripContainerScroll }
            };

            _contentContainer = new CarouselView
            {
                BackgroundColor = Color.Transparent,
                ItemsSource = TabItems.Where(t => t.Content != null),
                ItemTemplate = new DataTemplate(() =>
                {
                    var tabViewItemContent = new ContentView();
                    tabViewItemContent.SetBinding(ContentProperty, "CurrentContent");
                    return tabViewItemContent;
                }),
                IsSwipeEnabled = IsSwipeEnabled,
                IsScrollAnimated = IsTabTransitionEnabled,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Never,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Workaround to fix a Xamarin.Forms CarouselView issue that create a wrong 1px margin.
            if (Device.RuntimePlatform == Device.iOS)
                _contentContainer.Margin = new Thickness(-1, -1, 0, 0);

            _mainContainer = new Grid
            {
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = { _contentContainer, _tabStripContainer },
                RowSpacing = 0
            };

            _mainContainer.RowDefinitions.Add(new RowDefinition { Height = TabStripHeight > 0 ? TabStripHeight : GridLength.Auto });
            _mainContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            _mainContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            Grid.SetRow(_tabStripContainer, 0);
            Grid.SetRowSpan(_tabStripContainer, 2);

            Grid.SetRow(_contentContainer, 1);
            Grid.SetRowSpan(_contentContainer, 2);

            Content = _mainContainer;

            BatchCommit();

            UpdateIsEnabled();
            UpdateFlowDirection();
        }

        public void Dispose()
        {
            if (_contentContainer != null)
            {
                _contentContainer.PropertyChanged -= OnContentContainerPropertyChanged;
                _contentContainer.Scrolled -= OnContentContainerScrolled;
            }

            if (_tabItemsSource is INotifyCollectionChanged notifyTabItemsSource)
                notifyTabItemsSource.CollectionChanged -= OnTabItemsSourceCollectionChanged;

            if (TabItems != null)
                TabItems.CollectionChanged -= OnTabItemsCollectionChanged;
        }

        public ObservableCollection<TabViewItem> TabItems { get; set; }

        public static readonly BindableProperty TabItemsSourceProperty =
            BindableProperty.Create(nameof(TabItemsSource), typeof(IList), typeof(TabView), null,
                propertyChanged: OnTabItemsSourceChanged);

        public IList TabItemsSource
        {
            get => (IList)GetValue(TabItemsSourceProperty);
            set { SetValue(TabItemsSourceProperty, value); }
        }

        static void OnTabItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabItemsSource();
        }

        public static readonly BindableProperty TabViewItemDataTemplateProperty =
            BindableProperty.Create(nameof(TabViewItemDataTemplate), typeof(DataTemplate), typeof(TabView), null);

        public DataTemplate TabViewItemDataTemplate
        {
            get => (DataTemplate)GetValue(TabViewItemDataTemplateProperty);
            set { SetValue(TabViewItemDataTemplateProperty, value); }
        }

        public static readonly BindableProperty TabContentDataTemplateProperty =
           BindableProperty.Create(nameof(TabContentDataTemplate), typeof(DataTemplate), typeof(TabView), null);

        public DataTemplate TabContentDataTemplate
        {
            get => (DataTemplate)GetValue(TabContentDataTemplateProperty);
            set { SetValue(TabContentDataTemplateProperty, value); }
        }

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(TabView), -1,
                propertyChanged: OnSelectedIndexChanged);

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set { SetValue(SelectedIndexProperty, value); }
        }

        static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabView tabView && tabView.TabItems != null)
            {
                int selectedIndex = (int)newValue;

                if (selectedIndex < 0)
                {
                    return;
                }

                tabView.UpdateSelectedIndex(selectedIndex);
            }
        }

        public static readonly BindableProperty TabStripPlacementProperty =
            BindableProperty.Create(nameof(TabStripPlacement), typeof(TabStripPlacement), typeof(TabView), TabStripPlacement.Top,
                propertyChanged: OnTabStripPlacementChanged);

        public TabStripPlacement TabStripPlacement
        {
            get { return (TabStripPlacement)GetValue(TabStripPlacementProperty); }
            set { SetValue(TabStripPlacementProperty, value); }
        }

        static void OnTabStripPlacementChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabStripPlacement((TabStripPlacement)newValue);
        }

        public static readonly BindableProperty TabStripBackgroundColorProperty =
            BindableProperty.Create(nameof(TabStripBackgroundColor), typeof(Color), typeof(TabView), Color.Default,
                propertyChanged: OnTabStripBackgroundColorChanged);

        public Color TabStripBackgroundColor
        {
            get { return (Color)GetValue(TabStripBackgroundColorProperty); }
            set { SetValue(TabStripBackgroundColorProperty, value); }
        }

        static void OnTabStripBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabStripBackgroundColor((Color)newValue);
        }

        public static readonly BindableProperty TabStripBackgroundViewProperty =
           BindableProperty.Create(nameof(TabStripBackgroundColor), typeof(View), typeof(TabView), null,
               propertyChanged: OnTabStripBackgroundViewChanged);

        public View TabStripBackgroundView
        {
            get { return (View)GetValue(TabStripBackgroundViewProperty); }
            set { SetValue(TabStripBackgroundViewProperty, value); }
        }

        static void OnTabStripBackgroundViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabStripBackgroundView((View)newValue);
        }

        public static readonly BindableProperty TabContentBackgroundColorProperty =
            BindableProperty.Create(nameof(TabContentBackgroundColor), typeof(Color), typeof(TabView), Color.Default,
                propertyChanged: OnTabContentBackgroundColorChanged);

        public Color TabContentBackgroundColor
        {
            get { return (Color)GetValue(TabContentBackgroundColorProperty); }
            set { SetValue(TabContentBackgroundColorProperty, value); }
        }

        static void OnTabContentBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabContentBackgroundColor((Color)newValue);
        }

        public static readonly BindableProperty TabStripHeightProperty =
          BindableProperty.Create(nameof(TabStripHeight), typeof(double), typeof(TabView), 48d,
              propertyChanged: OnTabStripHeightChanged);

        public double TabStripHeight
        {
            get { return (double)GetValue(TabStripHeightProperty); }
            set { SetValue(TabStripHeightProperty, value); }
        }

        static void OnTabStripHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabStripHeight((double)newValue);
        }

        public static readonly BindableProperty IsTabStripVisibleProperty =
          BindableProperty.Create(nameof(IsTabStripVisible), typeof(bool), typeof(TabView), true,
              propertyChanged: OnIsTabStripVisibleChanged);

        public bool IsTabStripVisible
        {
            get { return (bool)GetValue(IsTabStripVisibleProperty); }
            set { SetValue(IsTabStripVisibleProperty, value); }
        }

        static void OnIsTabStripVisibleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateIsTabStripVisible((bool)newValue);
        }

        public static readonly BindableProperty TabContentHeightProperty =
            BindableProperty.Create(nameof(TabContentHeight), typeof(double), typeof(TabView), -1d,
               propertyChanged: OnTabContentHeightChanged);

        public double TabContentHeight
        {
            get { return (double)GetValue(TabContentHeightProperty); }
            set { SetValue(TabContentHeightProperty, value); }
        }

        static void OnTabContentHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabContentHeight((double)newValue);
        }

        public static readonly BindableProperty TabIndicatorColorProperty =
            BindableProperty.Create(nameof(TabIndicatorColor), typeof(Color), typeof(TabView), Color.Default,
               propertyChanged: OnTabIndicatorColorChanged);

        public Color TabIndicatorColor
        {
            get { return (Color)GetValue(TabIndicatorColorProperty); }
            set { SetValue(TabIndicatorColorProperty, value); }
        }

        static void OnTabIndicatorColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabIndicatorColor((Color)newValue);
        }

        public static readonly BindableProperty TabIndicatorHeightProperty =
            BindableProperty.Create(nameof(TabIndicatorHeight), typeof(double), typeof(TabView), 3d,
                propertyChanged: OnTabIndicatorHeightChanged);

        public double TabIndicatorHeight
        {
            get { return (double)GetValue(TabIndicatorHeightProperty); }
            set { SetValue(TabIndicatorHeightProperty, value); }
        }

        static void OnTabIndicatorHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabIndicatorHeight((double)newValue);
        }

        public static readonly BindableProperty TabIndicatorWidthProperty =
           BindableProperty.Create(nameof(TabIndicatorWidth), typeof(double), typeof(TabView), default(double),
               propertyChanged: OnTabIndicatorWidthChanged);

        public double TabIndicatorWidth
        {
            get { return (double)GetValue(TabIndicatorWidthProperty); }
            set { SetValue(TabIndicatorWidthProperty, value); }
        }

        static void OnTabIndicatorWidthChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabIndicatorWidth((double)newValue);
        }

        public static readonly BindableProperty TabIndicatorViewProperty =
            BindableProperty.Create(nameof(TabIndicatorView), typeof(View), typeof(TabView), null,
                propertyChanged: OnTabIndicatorViewChanged);

        public View TabIndicatorView
        {
            get { return (View)GetValue(TabIndicatorViewProperty); }
            set { SetValue(TabIndicatorViewProperty, value); }
        }

        static void OnTabIndicatorViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabIndicatorView((View)newValue);
        }

        public static readonly BindableProperty TabIndicatorPlacementProperty =
          BindableProperty.Create(nameof(TabIndicatorPlacement), typeof(TabIndicatorPlacement), typeof(TabView), TabIndicatorPlacement.Bottom,
             propertyChanged: OnTabIndicatorPlacementChanged);

        public TabIndicatorPlacement TabIndicatorPlacement
        {
            get { return (TabIndicatorPlacement)GetValue(TabIndicatorPlacementProperty); }
            set { SetValue(TabIndicatorPlacementProperty, value); }
        }

        static void OnTabIndicatorPlacementChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateTabIndicatorPlacement((TabIndicatorPlacement)newValue);
        }

        public static readonly BindableProperty IsTabTransitionEnabledProperty =
           BindableProperty.Create(nameof(IsTabTransitionEnabled), typeof(bool), typeof(TabView), true,
               propertyChanged: OnIsTabTransitionEnabledChanged);

        public bool IsTabTransitionEnabled
        {
            get => (bool)GetValue(IsTabTransitionEnabledProperty);
            set { SetValue(IsTabTransitionEnabledProperty, value); }
        }

        static void OnIsTabTransitionEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateIsTabTransitionEnabled((bool)newValue);
        }

        public static readonly BindableProperty IsSwipeEnabledProperty =
            BindableProperty.Create(nameof(IsSwipeEnabled), typeof(bool), typeof(TabView), true,
               propertyChanged: OnIsSwipeEnabledChanged);

        public bool IsSwipeEnabled
        {
            get { return (bool)GetValue(IsSwipeEnabledProperty); }
            set { SetValue(IsSwipeEnabledProperty, value); }
        }

        static void OnIsSwipeEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateIsSwipeEnabled((bool)newValue);
        }

        //MyCode
        public static readonly BindableProperty IsFloatingProperty =
          BindableProperty.Create(nameof(IsFloating), typeof(bool), typeof(TabView), false,
              propertyChanged: OnIsFloatingChanged);

        public bool IsFloating
        {
            get => (bool)GetValue(IsFloatingProperty);
            set { SetValue(IsFloatingProperty, value); }
        }
        static void OnIsFloatingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateIsFloating();
        }

        public static readonly BindableProperty MarginFromViewProperty =
          BindableProperty.Create(nameof(MarginFromView), typeof(Thickness), typeof(TabView), new Thickness(0, 0),
              propertyChanged: OnMarginFromViewChanged);

        public Thickness MarginFromView
        {
            get => (Thickness)GetValue(MarginFromViewProperty);
            set { SetValue(MarginFromViewProperty, value); }
        }
        static void OnMarginFromViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as TabView)?.UpdateIMargin();
        }

        void UpdateIsFloating()
        {
            UpdateTabContentLayout();
        }
        void UpdateIMargin()
        {
            _tabStripContainer.Padding = MarginFromView;
        }

        public delegate void TabSelectionChangedEventHandler(object sender, TabSelectionChangedEventArgs e);

        public event TabSelectionChangedEventHandler SelectionChanged;

        public delegate void TabViewScrolledEventHandler(object sender, ItemsViewScrolledEventArgs e);

        public event TabViewScrolledEventHandler Scrolled;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEnabledProperty.PropertyName)
                UpdateIsEnabled();
            else if (propertyName == FlowDirectionProperty.PropertyName)
                UpdateFlowDirection();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (TabItems == null || TabItems.Count == 0)
                return;

            foreach (var tabViewItem in TabItems)
                UpdateTabViewItemBindingContext(tabViewItem);
        }

        void OnTabViewItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tabViewItem = (TabViewItem)sender;

            if (e.PropertyName == IsEnabledProperty.PropertyName)
                UpdateTabViewItemIsEnabled(tabViewItem);
            else if (e.PropertyName == IsVisibleProperty.PropertyName)
                UpdateTabViewItemIsVisible(tabViewItem);
            else if (e.PropertyName == TabViewItem.TabWidthProperty.PropertyName)
                UpdateTabViewItemTabWidth(tabViewItem);
        }

        void OnTabItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (TabViewItem oldItem in e.OldItems)
                {
                    ClearTabViewItem(oldItem);
                }
            }

            if (e.NewItems != null)
            {
                foreach (TabViewItem newTabViewItem in e.NewItems)
                {
                    AddTabViewItem(newTabViewItem, TabItems.IndexOf(newTabViewItem));
                }
            }
        }

        void OnContentContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CarouselView.ItemsSource) ||
               e.PropertyName == nameof(CarouselView.VisibleViews))
            {
                var items = _contentContainer.ItemsSource;

                UpdateItemsSource(items);
            }
            else if (e.PropertyName == nameof(CarouselView.Position))
            {
                var selectedIndex = _contentContainer.Position;

                UpdateSelectedIndex(selectedIndex, true);
            }
        }

        void OnContentContainerScrolled(object sender, ItemsViewScrolledEventArgs args)
        {
            for (int i = 0; i < TabItems.Count; i++)
                TabItems[i].UpdateCurrentContent();

            UpdateTabIndicatorPosition(args);

            OnTabViewScrolled(args);
        }

        void ClearTabStrip()
        {
            foreach (var tabViewItem in TabItems)
                ClearTabViewItem(tabViewItem);

            if (_tabStripContent.Children.Count > 0)
                _tabStripContent.Children.Clear();

            bool hasItems = TabItems.Count > 0 || TabItemsSource.Count > 0;
            _tabStripContainer.IsVisible = hasItems;
        }

        void ClearTabViewItem(TabViewItem tabViewItem)
        {
            tabViewItem.PropertyChanged -= OnTabViewItemPropertyChanged;
            _tabStripContent.Children.Remove(tabViewItem);
        }

        void AddTabViewItem(TabViewItem tabViewItem, int index = -1)
        {
            tabViewItem.PropertyChanged -= OnTabViewItemPropertyChanged;
            tabViewItem.PropertyChanged += OnTabViewItemPropertyChanged;

            if (tabViewItem.ControlTemplate == null)
            {
                if (Device.RuntimePlatform == Device.Android)
                    tabViewItem.ControlTemplate = new ControlTemplate(typeof(MaterialTabViewItemTemplate));
                else if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.macOS)
                    tabViewItem.ControlTemplate = new ControlTemplate(typeof(CupertinoTabViewItemTemplate));
                else if (Device.RuntimePlatform == Device.UWP)
                    tabViewItem.ControlTemplate = new ControlTemplate(typeof(WindowsTabViewItemTemplate));
                else
                {
                    // Default ControlTemplate for other platforms
                    tabViewItem.ControlTemplate = new ControlTemplate(typeof(MaterialTabViewItemTemplate));
                }
            }

            AddSelectionTapRecognizer(tabViewItem);

            AddTabViewItemToTabStrip(tabViewItem, index);

            UpdateTabContentSize();
            UpdateTabStripSize();

            UpdateSelectedIndex(0);
        }

        void UpdateTabStripSize()
        {
            var tabStripSize = _tabStripContent.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);

            if (_tabStripContainer.HeightRequest != tabStripSize.Request.Height)
                _tabStripContainer.HeightRequest = tabStripSize.Request.Height;
        }

        void UpdateTabContentSize()
        {
            var items = _contentContainer.ItemsSource;

            int count = 0;

            var enumerator = items.GetEnumerator();

            while (enumerator.MoveNext())
                count++;

            BatchBegin();

            VerticalOptions = count != 0 ? LayoutOptions.FillAndExpand : LayoutOptions.Start;
            _mainContainer.HeightRequest = count != 0 ? (TabContentHeight + TabStripHeight) : TabStripHeight;
            UpdateTabContentHeight(count != 0 ? TabContentHeight : 0);

            BatchCommit();
        }

        void AddTabViewItemFromTemplate(object item, int index = -1)
        {
            AddTabViewItemFromTemplateToTabStrip(item, index);
        }

        void UpdateTabViewItemBindingContext(TabViewItem tabViewItem)
        {
            if (tabViewItem == null || tabViewItem.Content == null)
                return;

            tabViewItem.Content.BindingContext = BindingContext;
        }

        void AddSelectionTapRecognizer(View view)
        {
            var tapRecognizer = new TapGestureRecognizer();

            tapRecognizer.Tapped += (object sender, EventArgs args) =>
            {
                var capturedIndex = _tabStripContent.Children.IndexOf((View)sender);

                if (view is TabViewItem tabViewItem)
                {
                    var tabTappedEventArgs = new TabTappedEventArgs
                    {
                        Position = capturedIndex
                    };

                    tabViewItem.OnTabTapped(tabTappedEventArgs);
                }

                if (CanUpdateSelectedIndex(capturedIndex))
                    UpdateSelectedIndex(capturedIndex);
            };

            view.GestureRecognizers.Add(tapRecognizer);
        }

        void AddTabViewItemToTabStrip(View item, int index = -1)
        {
            var tabViewItemSizeRequest = item.Measure(double.PositiveInfinity, double.PositiveInfinity, MeasureFlags.IncludeMargins);

            if (tabViewItemSizeRequest.Request.Height < TabStripHeight)
            {
                item.HeightRequest = TabStripHeight;
                item.VerticalOptions = TabStripPlacement == TabStripPlacement.Top ? LayoutOptions.Start : LayoutOptions.End;
            }

            _tabStripContent.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = (item is TabViewItem tabViewItem && tabViewItem.TabWidth > 0) ? tabViewItem.TabWidth : GridLength.Star
            });

            if (index >= 0)
            {
                _tabStripContent.Children.Insert(index, item);

                for (int i = index; i < _tabStripContent.Children.Count; i++)
                    Grid.SetColumn(_tabStripContent.Children[i], i);
            }
            else
            {
                _tabStripContent.Children.Add(item);
                int count = _tabStripContent.Children.Count - 1;
                item.SetValue(Grid.ColumnProperty, count);
            }

            UpdateTabViewItemTabWidth(item as TabViewItem);
        }

        void AddTabViewItemFromTemplateToTabStrip(object item, int index = -1)
        {
            View view = !(TabViewItemDataTemplate is DataTemplateSelector tabItemDataTemplate) ?
                (View)TabViewItemDataTemplate.CreateContent() :
                (View)tabItemDataTemplate.SelectTemplate(item, this).CreateContent();

            view.BindingContext = item;

            view.Effects.Add(new VisualFeedbackEffect());

            AddSelectionTapRecognizer(view);
            AddTabViewItemToTabStrip(view, index);
        }

        void UpdateIsEnabled()
        {
            if (IsEnabled)
            {
                _contentContainer.PropertyChanged += OnContentContainerPropertyChanged;
                _contentContainer.Scrolled += OnContentContainerScrolled;

                TabItems.CollectionChanged += OnTabItemsCollectionChanged;
            }
            else
            {
                _contentContainer.PropertyChanged -= OnContentContainerPropertyChanged;
                _contentContainer.Scrolled -= OnContentContainerScrolled;

                TabItems.CollectionChanged -= OnTabItemsCollectionChanged;
            }

            _tabStripContent.IsEnabled = IsEnabled;
            _contentContainer.IsEnabled = IsEnabled;
        }

        void UpdateFlowDirection()
        {
            // TODO: Update FlowDirection
            Console.WriteLine($"Update TabViewItem FlowDirection: {FlowDirection}");
        }

        void UpdateTabViewItemIsEnabled(TabViewItem tabViewItem)
        {
            // TODO: Enable / Disable TabViewItem
            Console.WriteLine($"Update TabViewItem IsEnabled: {tabViewItem.IsEnabled}");
        }

        void UpdateTabViewItemIsVisible(TabViewItem tabViewItem)
        {
            // TODO: Hide / Show TabViewItem
            Console.WriteLine($"Update TabViewItem IsVisible: {tabViewItem.IsVisible}");

            bool isTabStripVisible = false;

            foreach (var tabItem in TabItems)
            {
                if (tabItem.IsVisible)
                {
                    isTabStripVisible = true;
                    break;
                }
            }

            UpdateIsTabStripVisible(isTabStripVisible);
        }

        void UpdateTabViewItemTabWidth(TabViewItem tabViewItem)
        {
            if (tabViewItem == null)
                return;

            var index = _tabStripContent.Children.IndexOf(tabViewItem);
            var colummns = _tabStripContent.ColumnDefinitions;

            ColumnDefinition column = null;

            if (index < colummns.Count)
                column = colummns[index];

            if (column == null)
                return;

            column.Width = tabViewItem.TabWidth > 0 ? tabViewItem.TabWidth : GridLength.Star;
            UpdateTabIndicatorPosition(SelectedIndex);
        }

        void UpdateTabItemsSource()
        {
            if (TabItemsSource == null || TabViewItemDataTemplate == null)
                return;

            if (_tabItemsSource is INotifyCollectionChanged oldnNotifyTabItemsSource)
                oldnNotifyTabItemsSource.CollectionChanged -= OnTabItemsSourceCollectionChanged;
            
            _tabItemsSource = TabItemsSource;

            if (_tabItemsSource is INotifyCollectionChanged newNotifyTabItemsSource)
                newNotifyTabItemsSource.CollectionChanged += OnTabItemsSourceCollectionChanged;
            
            ClearTabStrip();

            _contentContainer.ItemTemplate = TabContentDataTemplate;
            _contentContainer.ItemsSource = TabItemsSource;

            foreach (object item in TabItemsSource)
                AddTabViewItemFromTemplate(item);

            UpdateTabContentSize();
            UpdateTabStripSize();

            UpdateSelectedIndex(0);
        }

        void OnTabItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateTabItemsSource();
        }

        void UpdateItemsSource(IEnumerable items)
        {
            _contentWidthCollection.Clear();

            if (_contentContainer.VisibleViews.Count == 0)
                return;

            double contentWidth = _contentContainer.VisibleViews.FirstOrDefault().Width;
            int tabItemsCount = items.Cast<object>().Count();

            for (int i = 0; i < tabItemsCount; i++)
                _contentWidthCollection.Add(contentWidth * i);
        }

        bool CanUpdateSelectedIndex(int selectedIndex)
        {
            if (TabItems == null || TabItems.Count == 0)
                return true;

            var tabItem = TabItems[selectedIndex];

            if (tabItem != null && tabItem.Content == null)
            {
                var itemsCount = TabItems.Count;
                var contentItemsCount = TabItems.Count(t => t.Content == null);

                return itemsCount == contentItemsCount;
            }

            return true;
        }

        void UpdateSelectedIndex(int position, bool hasCurrentItem = false)
        {
            if (position < 0)
                return;

            int oldPosition = SelectedIndex;
            int newPosition = position;

            if (oldPosition == newPosition)
                return;

            SelectedIndex = newPosition;

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (_contentTabItems == null)
                    _contentTabItems = new ObservableCollection<TabViewItem>(TabItems.Where(t => t.Content != null));

                int contentIndex = position;
                int tabStripIndex = position;

                if (TabItems.Count > 0)
                {
                    TabViewItem currentItem = null;

                    if (hasCurrentItem)
                        currentItem = (TabViewItem)_contentContainer.CurrentItem;

                    var tabViewItem = TabItems[SelectedIndex];

                    contentIndex = _contentTabItems.IndexOf(currentItem ?? tabViewItem);
                    tabStripIndex = TabItems.IndexOf(currentItem ?? tabViewItem);

                    position = SelectedIndex = tabStripIndex;

                    for (int index = 0; index < TabItems.Count; index++)
                    {
                        if (index == position)
                            TabItems[position].IsSelected = true;
                        else
                            TabItems[index].IsSelected = false;
                    }

                    var currentTabItem = TabItems[position];
                    currentTabItem.SizeChanged += OnCurrentTabItemSizeChanged;
                    UpdateTabIndicatorPosition(currentTabItem);
                }
                else
                    UpdateTabIndicatorPosition(position);

                if (contentIndex >= 0)
                    _contentContainer.Position = contentIndex;

                if (_tabStripContent.Children.Count > 0)
                    await _tabStripContainerScroll.ScrollToAsync(_tabStripContent.Children[tabStripIndex], ScrollToPosition.MakeVisible, false);
            });

            if (newPosition != oldPosition)
            {
                var selectionChangedArgs = new TabSelectionChangedEventArgs()
                {
                    NewPosition = newPosition,
                    OldPosition = oldPosition
                };

                OnTabSelectionChanged(selectionChangedArgs);
            }
        }

        void OnCurrentTabItemSizeChanged(object sender, EventArgs e)
        {
            var currentTabItem = (View)sender;
            UpdateTabIndicatorWidth(TabIndicatorWidth > 0 ? TabIndicatorWidth : currentTabItem.Width);
            UpdateTabIndicatorPosition(currentTabItem);
            currentTabItem.SizeChanged -= OnCurrentTabItemSizeChanged;
        }

        void UpdateTabStripPlacement(TabStripPlacement tabStripPlacement)
        {
            if (tabStripPlacement == TabStripPlacement.Top)
            {
                _tabStripBackground.VerticalOptions = LayoutOptions.Start;

                Grid.SetRow(_tabStripContainer, 0);
                Grid.SetRowSpan(_tabStripContainer, 2);

                _mainContainer.RowDefinitions[0].Height = TabStripHeight > 0 ? TabStripHeight : GridLength.Auto;
                _mainContainer.RowDefinitions[1].Height = GridLength.Auto;
                _mainContainer.RowDefinitions[2].Height = GridLength.Star;
            }

            if (tabStripPlacement == TabStripPlacement.Bottom)
            {
                _tabStripBackground.VerticalOptions = LayoutOptions.End;

                Grid.SetRow(_tabStripContainer, 1);
                Grid.SetRowSpan(_tabStripContainer, 2);

                _mainContainer.RowDefinitions[0].Height = GridLength.Star;
                _mainContainer.RowDefinitions[1].Height = GridLength.Auto;
                _mainContainer.RowDefinitions[2].Height = TabStripHeight > 0 ? TabStripHeight : GridLength.Auto;
            }

            UpdateTabContentLayout();
        }

        void UpdateTabContentLayout()
        {
            BatchBegin();

            if (_tabStripContainer.IsVisible)
            {
                if (TabStripPlacement == TabStripPlacement.Top)
                {
                    Grid.SetRow(_contentContainer, 1);
                    Grid.SetRowSpan(_contentContainer, 2);
                }
                else if (IsFloating == true)
                {
                    Grid.SetRow(_contentContainer, 0);
                    Grid.SetRowSpan(_contentContainer, 3);
                }
                else
                {
                    Grid.SetRow(_contentContainer, 0);
                    Grid.SetRowSpan(_contentContainer, 2);
                }
            }
            else
            {
                Grid.SetRow(_contentContainer, 0);
                Grid.SetRowSpan(_contentContainer, 3);
            }

            BatchCommit();
        }

        void UpdateTabStripBackgroundColor(Color tabStripBackgroundColor)
        {
            _tabStripBackground.BackgroundColor = tabStripBackgroundColor;

            if (Device.RuntimePlatform == Device.macOS)
                _tabStripContainerScroll.BackgroundColor = tabStripBackgroundColor;
        }

        void UpdateTabStripBackgroundView(View tabStripBackgroundView)
        {
            if (tabStripBackgroundView != null)
                _tabStripBackground.Children.Add(tabStripBackgroundView);
            else
                _tabStripBackground.Children.Clear();
        }

        void UpdateTabContentBackgroundColor(Color tabContentBackgroundColor)
        {
            _contentContainer.BackgroundColor = tabContentBackgroundColor;
        }

        void UpdateTabStripHeight(double tabStripHeight)
        {
            _tabStripBackground.HeightRequest = tabStripHeight;
        }

        void UpdateIsTabStripVisible(bool isTabStripVisible)
        {
            _tabStripContainer.IsVisible = isTabStripVisible;

            UpdateTabContentLayout();
        }

        void UpdateTabContentHeight(double tabContentHeight)
        {
            _contentContainer.HeightRequest = tabContentHeight;
        }

        void UpdateTabIndicatorColor(Color tabIndicatorColor)
        {
            _tabStripIndicator.BackgroundColor = tabIndicatorColor;
        }

        void UpdateTabIndicatorHeight(double tabIndicatorHeight)
        {
            _tabStripIndicator.HeightRequest = tabIndicatorHeight;
        }

        void UpdateTabIndicatorWidth(double tabIndicatorWidth)
        {
            _tabStripIndicator.WidthRequest = tabIndicatorWidth;
        }

        void UpdateTabIndicatorView(View tabIndicatorView)
        {
            if (tabIndicatorView != null)
                _tabStripIndicator.Children.Add(tabIndicatorView);
            else
                _tabStripIndicator.Children.Clear();
        }

        void UpdateTabIndicatorPlacement(TabIndicatorPlacement tabIndicatorPlacement)
        {
            switch (tabIndicatorPlacement)
            {
                case TabIndicatorPlacement.Top:
                    _tabStripIndicator.VerticalOptions = LayoutOptions.Start;
                    break;
                case TabIndicatorPlacement.Center:
                    _tabStripIndicator.VerticalOptions = LayoutOptions.Center;
                    break;
                case TabIndicatorPlacement.Bottom:
                default:
                    _tabStripIndicator.VerticalOptions = LayoutOptions.End;
                    break;
            }
        }

        void UpdateIsSwipeEnabled(bool isSwipeEnabled)
        {
            _contentContainer.IsSwipeEnabled = isSwipeEnabled;
        }

        void UpdateIsTabTransitionEnabled(bool isTabTransitionEnabled)
        {
            _contentContainer.IsScrollAnimated = isTabTransitionEnabled;
        }

        void UpdateTabIndicatorPosition(int tabViewItemIndex)
        {
            if (_tabStripContent == null || _tabStripContent.Children.Count == 0 || tabViewItemIndex == -1)
                return;

            var currentTabViewItem = _tabStripContent.Children[tabViewItemIndex];

            if (currentTabViewItem.Width <= 0)
                currentTabViewItem.SizeChanged += OnCurrentTabItemSizeChanged;
            else
                UpdateTabIndicatorWidth(currentTabViewItem.Width);

            UpdateTabIndicatorPosition(currentTabViewItem);
        }

        void UpdateTabIndicatorPosition(ItemsViewScrolledEventArgs args)
        {
            if (args.HorizontalOffset == 0)
            {
                UpdateTabIndicatorPosition(SelectedIndex);
                return;
            }

            if (_tabStripContent == null || TabItems.Count == 0)
                return;

            if (_contentWidthCollection.Count == 0)
                UpdateItemsSource(_contentContainer.ItemsSource);

            var offset = args.HorizontalOffset;
            bool toRight = args.HorizontalDelta > 0;

            int nextIndex = toRight ? _contentWidthCollection.FindIndex(c => c > offset) : _contentWidthCollection.FindLastIndex(c => c < offset);
            int previousIndex = toRight ? nextIndex - 1 : nextIndex + 1;

            if (previousIndex < 0 || nextIndex < 0)
                return;

            var itemsCount = TabItems.Count;

            if (previousIndex >= 0 && previousIndex < itemsCount)
            {
                var currentTabViewItem = TabItems[previousIndex];
                var currentTabViewItemWidth = currentTabViewItem.Width;

                UpdateTabIndicatorWidth(currentTabViewItemWidth);

                var contentItemsCount = _contentWidthCollection.Count;

                if (previousIndex >= 0 && previousIndex < contentItemsCount)
                {
                    double progress = (offset - _contentWidthCollection[previousIndex]) / (_contentWidthCollection[nextIndex] - _contentWidthCollection[previousIndex]);
                    double position = toRight ? currentTabViewItem.X + (currentTabViewItemWidth * progress) : currentTabViewItem.X - (currentTabViewItemWidth * progress);

                    _tabStripIndicator.TranslateTo(position, 0, TabIndicatorAnimationDuration, Easing.Linear);
                }
            }
        } 

        void UpdateTabIndicatorPosition(View currentTabViewItem)
        {
            double width = TabIndicatorWidth > 0 ? (currentTabViewItem.Width - _tabStripIndicator.Width) : 0;
            double position = currentTabViewItem.X + (width / 2);
            _tabStripIndicator.TranslateTo(position, 0, TabIndicatorAnimationDuration, Easing.Linear);
        }

        internal virtual void OnTabSelectionChanged(TabSelectionChangedEventArgs e)
        {
            TabSelectionChangedEventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        internal virtual void OnTabViewScrolled(ItemsViewScrolledEventArgs e)
        {
            TabViewScrolledEventHandler handler = Scrolled;
            handler?.Invoke(this, e);
        }
    }
}