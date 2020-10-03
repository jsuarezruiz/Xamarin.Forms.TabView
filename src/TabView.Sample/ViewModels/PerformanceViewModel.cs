using System.Collections.ObjectModel;
using TabView.Sample.Models;
using Xamarin.Forms;

namespace TabView.Sample.ViewModels
{
    public class PerformanceViewModel : BindableObject
    {
        public PerformanceViewModel()
        {
            LoadMonkeys();
        }

        public ObservableCollection<Monkey> Monkeys1 { get; set; }
        public ObservableCollection<Monkey> Monkeys2 { get; set; }
        public ObservableCollection<Monkey> Monkeys3 { get; set; }

        void LoadMonkeys()
        {
            Monkeys1 = new ObservableCollection<Monkey>();

            for (int i = 0; i < 1000; i++)
            {
                Monkeys1.Add(new Monkey
                {
                    Index = $"Baboon {i}",
                    Name = "Baboon",
                    Location = "Africa & Asia",
                    Details = "Baboons are African and Arabian Old World monkeys belonging to the genus Papio, part of the subfamily Cercopithecinae.",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg",
                    Color = Color.LightSalmon
                });

                Monkeys1.Add(new Monkey
                {
                    Index = $"Capuchin Monkey {i}",
                    Name = "Capuchin Monkey",
                    Location = "Central & South America",
                    Details = "The capuchin monkeys are New World monkeys of the subfamily Cebinae. Prior to 2011, the subfamily contained only a single genus, Cebus.",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg",
                    Color = Color.LightBlue
                });

                Monkeys1.Add(new Monkey
                {
                    Index = $"Blue Monkey {i}",
                    Name = "Blue Monkey",
                    Location = "Central and East Africa",
                    Details = "The blue monkey or diademed monkey is a species of Old World monkey native to Central and East Africa, ranging from the upper Congo River basin east to the East African Rift and south to northern Angola and Zambia",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/8/83/BlueMonkey.jpg/220px-BlueMonkey.jpg",
                    Color = Color.LightSlateGray
                });
            }

            Monkeys2 = new ObservableCollection<Monkey>();

            for (int i = 0; i < 500; i++)
            {
                Monkeys2.Add(new Monkey
                {
                    Index = $"Baboon {i}",
                    Name = "Baboon",
                    Location = "Africa & Asia",
                    Details = "Baboons are African and Arabian Old World monkeys belonging to the genus Papio, part of the subfamily Cercopithecinae.",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg",
                    Color = Color.LightSalmon
                });

                Monkeys2.Add(new Monkey
                {
                    Index = $"Capuchin Monkey {i}",
                    Name = "Capuchin Monkey",
                    Location = "Central & South America",
                    Details = "The capuchin monkeys are New World monkeys of the subfamily Cebinae. Prior to 2011, the subfamily contained only a single genus, Cebus.",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg",
                    Color = Color.LightBlue
                });

                Monkeys2.Add(new Monkey
                {
                    Index = $"Blue Monkey {i}",
                    Name = "Blue Monkey",
                    Location = "Central and East Africa",
                    Details = "The blue monkey or diademed monkey is a species of Old World monkey native to Central and East Africa, ranging from the upper Congo River basin east to the East African Rift and south to northern Angola and Zambia",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/8/83/BlueMonkey.jpg/220px-BlueMonkey.jpg",
                    Color = Color.LightSlateGray
                });
            }

            Monkeys3 = new ObservableCollection<Monkey>();

            for (int i = 0; i < 2000; i++)
            {
                Monkeys3.Add(new Monkey
                {
                    Index = $"Baboon {i}",
                    Name = "Baboon",
                    Location = "Africa & Asia",
                    Details = "Baboons are African and Arabian Old World monkeys belonging to the genus Papio, part of the subfamily Cercopithecinae.",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg",
                    Color = Color.LightSalmon
                });

                Monkeys3.Add(new Monkey
                {
                    Index = $"Capuchin Monkey {i}",
                    Name = "Capuchin Monkey",
                    Location = "Central & South America",
                    Details = "The capuchin monkeys are New World monkeys of the subfamily Cebinae. Prior to 2011, the subfamily contained only a single genus, Cebus.",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg",
                    Color = Color.LightBlue
                });

                Monkeys3.Add(new Monkey
                {
                    Index = $"Blue Monkey {i}",
                    Name = "Blue Monkey",
                    Location = "Central and East Africa",
                    Details = "The blue monkey or diademed monkey is a species of Old World monkey native to Central and East Africa, ranging from the upper Congo River basin east to the East African Rift and south to northern Angola and Zambia",
                    Image = "http://upload.wikimedia.org/wikipedia/commons/thumb/8/83/BlueMonkey.jpg/220px-BlueMonkey.jpg",
                    Color = Color.LightSlateGray
                });
            }
        }
    }
}