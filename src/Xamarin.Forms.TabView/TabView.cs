using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xamarin.Forms
{
    [ContentProperty(nameof(TabItems))]
    public class TabView : ContentView
    {
        const uint TabIndicatorAnimationDuration = 100;

        Grid _mainContainer;
        Grid _tabStripContainer;
        Grid _tabStripBackground;
        ScrollView _tabStripContainerScroll;
        Grid _tabStripIndicator;
        Grid _tabStripContent;
        Grid _tabStripContentContainer;
        CarouselView _contentContainer;

        public TabView()
        {
            Initialize();
        }

        public ObservableCollection<TabViewItem> TabItems { get; set; }

        // TODO: Include HasTabStripShadow, etc.

        public static readonly BindableProperty TabItemsSourceProperty =
            BindableProperty.Create(nameof(TabItemsSource), typeof(IEnumerable), typeof(TabView), null,
                propertyChanged: OnTabItemsSourceChanged);

        public IEnumerable TabItemsSource
        {
            get => (IEnumerable)GetValue(TabItemsSourceProperty);
            set { SetValue(TabItemsSourceProperty, value); }
        }

        static void OnTabItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabView tabView)
            {
                // TODO: If ItemsSource implements INotifyCollectionChanged, detect changes in the collection and update the TabView.
                tabView.UpdateTabItemsSource();
            }
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

        public static readonly BindableProperty IsLazyProperty =
            BindableProperty.Create(nameof(IsLazy), typeof(bool), typeof(TabView), true);

        public bool IsLazy
        {
            get => (bool)GetValue(IsLazyProperty);
            set { SetValue(IsLazyProperty, value); }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabStripPlacement((TabStripPlacement)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabStripBackgroundColor((Color)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabStripBackgroundView((View)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabContentBackgroundColor((Color)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabStripHeight((double)newValue);
            }
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
            if (bindable is TabView tabView && tabView._contentContainer != null)
            {
                tabView.UpdateTabContentHeight((double)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabIndicatorColor((Color)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabIndicatorHeight((double)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabIndicatorWidth((double)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabIndicatorView((View)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateTabIndicatorPlacement((TabIndicatorPlacement)newValue);
            }
        }

        public static readonly BindableProperty IsTabTransitionEnabledProperty =
           BindableProperty.Create(nameof(IsTabTransitionEnabled), typeof(bool), typeof(TabView), false,
               propertyChanged: OnIsTabTransitionEnabledChanged);

        public bool IsTabTransitionEnabled
        {
            get => (bool)GetValue(IsTabTransitionEnabledProperty);
            set { SetValue(IsTabTransitionEnabledProperty, value); }
        }

        static void OnIsTabTransitionEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TabView tabView)
            {
                tabView.UpdateIsTabTransitionEnabled((bool)newValue);
            }
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
            if (bindable is TabView tabView)
            {
                tabView.UpdateIsSwipeEnabled((bool)newValue);
            }
        }

        public delegate void TabSelectionChangedEventHandler(object sender, TabSelectionChangedEventArgs e);

        public event TabSelectionChangedEventHandler SelectionChanged;

        public delegate void TabViewScrolledEventHandler(object sender, TabViewScrolledEventArgs e);

        public event TabViewScrolledEventHandler Scrolled;

        void Initialize()
        {
            TabItems = new ObservableCollection<TabViewItem>();

            _tabStripBackground = new Grid
            {
                BackgroundColor = TabStripBackgroundColor,
                HeightRequest = TabStripHeight,
                VerticalOptions = LayoutOptions.Start
            };

            _tabStripIndicator = new Grid
            {
                HeightRequest = TabIndicatorHeight,
                BackgroundColor = TabIndicatorColor,
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

            _tabStripContainer = new Grid
            {
                BackgroundColor = Color.Transparent,
                Children = { _tabStripBackground, _tabStripContainerScroll },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start
            };

            _contentContainer = new CarouselView
            {
                ItemsSource = TabItems,
                ItemTemplate = new DataTemplate(() =>
                {
                    var tabViewItemContent = new ContentView();
                    tabViewItemContent.SetBinding(ContentProperty, "CurrentContent");
                    return tabViewItemContent;
                }),
                IsSwipeEnabled = IsSwipeEnabled,
                IsScrollAnimated = IsTabTransitionEnabled,
                IsBounceEnabled = false,
                BackgroundColor = Color.Transparent,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // TODO: Unsubscribe CarouselView events
            _contentContainer.PropertyChanged += OnContentContainerPropertyChanged;
            _contentContainer.PositionChanged += OnContentContainerPositionChanged;
            _contentContainer.Scrolled += OnContentContainerScrolled;

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

            // TODO: Unsubscribe CollectionChanged event
            TabItems.CollectionChanged += OnTabItemsCollectionChanged;

            UpdateIsEnabled();
            UpdateFlowDirection();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEnabledProperty.PropertyName)
                UpdateIsEnabled();
            else if (propertyName == FlowDirectionProperty.PropertyName)
                UpdateFlowDirection();
        }

        void OnTabViewItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tabViewItem = (TabViewItem)sender;

            if (e.PropertyName == IsEnabledProperty.PropertyName)
                UpdateTabViewItemIsEnabled(tabViewItem);
            else if (e.PropertyName == IsVisibleProperty.PropertyName)
                UpdateTabViewItemIsVisible(tabViewItem);
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

        void OnContentContainerPositionChanged(object sender, PositionChangedEventArgs e)
        {
            if (SelectedIndex != e.CurrentPosition)
            {
                UpdateSelectedIndex(e.CurrentPosition);
            }
        }

        void OnContentContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CarouselView.Position))
            {
                var selectedIndex = _contentContainer.Position;

                UpdateSelectedIndex(selectedIndex);
            }
        }

        void OnContentContainerScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            for (int i = 0; i < TabItems.Count; i++)
            {
                if (IsLazy)
                {
                    bool isOnScreen = i == e.FirstVisibleItemIndex || i == e.CenterItemIndex || i == e.LastVisibleItemIndex;
                    TabItems[i].UpdateCurrentContent(isOnScreen);
                }
                else
                    TabItems[i].UpdateCurrentContent();
            }

            // TODO: Update TabIndicator position based on scroll offset (immediate visual feedback).

            var tabViewScrolledEventArgs = new TabViewScrolledEventArgs
            {
                HorizontalDelta = e.HorizontalDelta,
                VerticalDelta = e.VerticalDelta,
                HorizontalOffset = e.HorizontalOffset,
                VerticalOffset = e.VerticalOffset,
                FirstVisibleTabViewItemIndex = e.FirstVisibleItemIndex,
                CenterTabViewItemIndex = e.CenterItemIndex,
                LastVisibleTabViewItemIndex = e.LastVisibleItemIndex
            };

            OnTabViewScrolled(tabViewScrolledEventArgs);
        }

        void ClearTabStrip()
        {
            foreach (var tabViewItem in TabItems)
                ClearTabViewItem(tabViewItem);

            if (_tabStripContent.Children.Count > 0)
                _tabStripContent.Children.Clear();
        }

        void ClearTabViewItem(TabViewItem tabViewItem)
        {
            tabViewItem.PropertyChanged -= OnTabViewItemPropertyChanged;
            tabViewItem.Parent = null;
            _tabStripContent.Children.Remove(tabViewItem);
        }

        void AddTabViewItem(TabViewItem tabViewItem, int index = -1)
        {
            if (tabViewItem.Content == null)
            {
                throw new ArgumentException("The TabViewItem must have the Content property set.");
            }

            tabViewItem.PropertyChanged -= OnTabViewItemPropertyChanged;
            tabViewItem.PropertyChanged += OnTabViewItemPropertyChanged;

            tabViewItem.Parent = this;

            if (tabViewItem.ControlTemplate == null)
            {
                if (Device.RuntimePlatform == Device.Android)
                    tabViewItem.ControlTemplate = new ControlTemplate(typeof(MaterialTabViewItemTemplate));
                else if (Device.RuntimePlatform == Device.iOS)
                    tabViewItem.ControlTemplate = new ControlTemplate(typeof(CupertinoTabViewItemTemplate));
                else
                {
                    // Default ControlTemplate for other platforms
                    tabViewItem.ControlTemplate = new ControlTemplate(typeof(MaterialTabViewItemTemplate));
                }
            }

            AddSelectionTapRecognizer(tabViewItem);

            AddTabViewItemToTabStrip(tabViewItem, index);

            UpdateSelectedIndex(0);
        }

        void AddTabViewItemFromTemplate(object item, int index = -1)
        {
            AddTabViewItemFromTemplateToTabStrip(item, index);
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
        }

        void AddTabViewItemFromTemplateToTabStrip(object item, int index = -1)
        {
            DataTemplateSelector tabItemDataTemplate = TabViewItemDataTemplate as DataTemplateSelector;

            View view = tabItemDataTemplate == null ? (View)TabViewItemDataTemplate.CreateContent() : (View)tabItemDataTemplate.SelectTemplate(item, this).CreateContent();
            view.Parent = this;
            view.BindingContext = item;

            AddSelectionTapRecognizer(view);
            AddTabViewItemToTabStrip(view, index);
        }
        
        void UpdateIsEnabled()
        {
            _tabStripContent.IsEnabled = IsEnabled;
            _contentContainer.IsEnabled = IsEnabled;
        }

        void UpdateFlowDirection()
        {
            // TODO: Update FlowDirection
            Console.WriteLine(FlowDirection);
        }

        void UpdateTabViewItemIsEnabled(TabViewItem tabViewItem)
        {
            // TODO: Enable / Disable TabViewItem
            Console.WriteLine(tabViewItem.Text);
        }

        void UpdateTabViewItemIsVisible(TabViewItem tabViewItem)
        {
            // TODO: Hide / Show TabViewItem
            Console.WriteLine(tabViewItem.Text);
        }

        void UpdateTabItemsSource()
        {
            if (TabItemsSource == null || TabViewItemDataTemplate == null)
                return;

            ClearTabStrip();

            _contentContainer.ItemsSource = TabItemsSource;
            _contentContainer.ItemTemplate = TabContentDataTemplate;

            foreach (object item in TabItemsSource)
            {
                AddTabViewItemFromTemplate(item);
            }

            UpdateSelectedIndex(0);
        }

        void UpdateSelectedIndex(int position)
        {
            if (position < 0)
            {
                return;
            }

            int oldPosition = SelectedIndex;
            int newPosition = position;

            SelectedIndex = newPosition;
            _contentContainer.Position = SelectedIndex;

            Device.BeginInvokeOnMainThread(
                async () => await _tabStripContainerScroll.ScrollToAsync(_tabStripContent.Children[position], ScrollToPosition.MakeVisible, false));

            if (TabItems.Count > 0)
            {
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
                Grid.SetRow(_contentContainer, 1);
                Grid.SetRowSpan(_contentContainer, 2);
                _mainContainer.RowDefinitions[0].Height = TabStripHeight > 0 ? TabStripHeight : GridLength.Auto;
                _mainContainer.RowDefinitions[1].Height = GridLength.Auto;
                _mainContainer.RowDefinitions[2].Height = GridLength.Star;
            }

            if (tabStripPlacement == TabStripPlacement.Bottom)
            {
                _tabStripBackground.VerticalOptions = LayoutOptions.End;
                Grid.SetRow(_contentContainer, 0);
                Grid.SetRowSpan(_contentContainer, 2);
                Grid.SetRow(_tabStripContainer, 1);
                Grid.SetRowSpan(_tabStripContainer, 2);
                _mainContainer.RowDefinitions[0].Height = GridLength.Star;
                _mainContainer.RowDefinitions[1].Height = GridLength.Auto;
                _mainContainer.RowDefinitions[2].Height = TabStripHeight > 0 ? TabStripHeight : GridLength.Auto;
            }
        }

        void UpdateTabStripBackgroundColor(Color tabStripBackgroundColor)
        {
            _tabStripBackground.BackgroundColor = tabStripBackgroundColor;
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
            switch(tabIndicatorPlacement)
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
            var currentTabViewItem = _tabStripContent.Children[tabViewItemIndex];

            if(currentTabViewItem.Width <= 0)
                currentTabViewItem.SizeChanged += OnCurrentTabItemSizeChanged;
            else
                UpdateTabIndicatorWidth(currentTabViewItem.Width);

            UpdateTabIndicatorPosition(currentTabViewItem);
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

        internal virtual void OnTabViewScrolled(TabViewScrolledEventArgs e)
        {
            TabViewScrolledEventHandler handler = Scrolled;
            handler?.Invoke(this, e);
        }
    }
}