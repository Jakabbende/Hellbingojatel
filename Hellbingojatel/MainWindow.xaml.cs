using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Hellbingojatel
{

    public partial class MainWindow : Window
    {
        //game numbers int, string
        public int[] numbers = new int[16];

        //personal numbers
        public int[] mynumberst = new int[4];
        public int[] ennumberst = new int[4];

        //money
        public int moneyink = 15;
        //powerups
        public int changenumber = 0;
        public int enemymissround = 0;
        public int wildcards = 0;

        // Whose round is it
        public string[] whoseround = new string[2];
        //random generator
        Random rand = new Random();

        //whatnight
        public int night = 1;


        public MainWindow()
        {
            InitializeComponent();
            game.Visibility = Visibility.Hidden;
            buyphase.Visibility = Visibility.Hidden;
            Won.Visibility = Visibility.Hidden;
            // Generate 16 random numbers between 1 and 100 to play bingo with
            for (int i = 0; i < 16; i++)
            {
                var num = 0;
                do
                {
                    num = rand.Next(1, 100);
                }
                while (numbers.Contains(num));
                numbers[i] = num;
            }
        }


        // START BUY PHASE
        private void StartButton_Click(object sender, RoutedEventArgs e) 
        {

            menu.Visibility = Visibility.Hidden;
            game.Visibility = Visibility.Hidden;
            buyphase.Visibility = Visibility.Visible;
            powerupwindow.Visibility = Visibility.Hidden;


            inkcount.Content = "Your Inks: " + moneyink;

            numberchangercount.Content = "Your Number Changes: " + changenumber;
            enemymissroundcount.Content = "Your Enemy Miss Rounds: " + enemymissround;
            wildcardcount.Content = "Your Wildcards: " + changenumber;

        }

        //POWERUPS BUYPHASE
        // Buy number changer
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (moneyink >= 5)
            {
                moneyink -= 5;
                changenumber += 1;
                inkcount.Content = "Your Inks: " + moneyink;
                numberchangercount.Content = "Your Number Changes: " + changenumber;
                numberchangercount.Foreground = Brushes.Green;
                currentstatus.Content = "You bought a Number Changer!";
                currentstatus.Foreground = Brushes.Green;
            }
            else
            {
                currentstatus.Content = "Not enough Inks!";
                currentstatus.Foreground = Brushes.Red;
            }
        }

        // Buy miss round
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (moneyink >= 10)
            {
                moneyink -= 10;
                enemymissround += 1;
                inkcount.Content = "Your Inks: " + moneyink;
                enemymissroundcount.Content = "Your Enemy Miss Rounds: " + enemymissround;
                enemymissroundcount.Foreground = Brushes.Green;
                currentstatus.Content = "You bought an Enemy Miss Round!";
                currentstatus.Foreground = Brushes.Green;

            }
            else
            {
                currentstatus.Content = "Not enough Inks!";
                currentstatus.Foreground = Brushes.Red;
            }

        }
        // buy wildcard
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (moneyink >= 15)
            {
                moneyink -= 15;
                wildcards += 1;
                inkcount.Content = "Your Inks: " + moneyink;
                wildcardcount.Content = "Your Wildcards: " + wildcards;
                wildcardcount.Foreground = Brushes.Green;
                currentstatus.Content = "You bought a Wildcard!";
                currentstatus.Foreground = Brushes.Green;
            }
            else
            {
                currentstatus.Content = "Not enough Inks!";
                currentstatus.Foreground = Brushes.Red;
            }
        }


        // Start Game
        public async void Button_Click(object sender, RoutedEventArgs e) 
        {
            game.Visibility = Visibility.Visible;
            buyphase.Visibility = Visibility.Hidden;
            menu.Visibility = Visibility.Hidden;

            string[] enemies = { "Dr. Frankeinsten", "Skeleton", "Schizoprhenic", "Guard" };
            Random rand = new Random();
            var enemynumber = rand.Next(enemies.Length);
            enemynamelabel.Content = enemies[enemynumber] + "'s numbers:";
            nightscount.Content = "Night: " + night;

            // Put two names in whoseround array
            whoseround[0] = "Player";
            whoseround[1] = enemies[enemynumber];

            // Generate four random numbers for both player and enemy
            mynumberst[0] = rand.Next(1, 100);
            mynumberst[1] = numbers[rand.Next(numbers.Length)];
            mynumberst[2] = rand.Next(1, 100);
            mynumberst[3] = numbers[rand.Next(numbers.Length)];

            ennumberst[0] = rand.Next(1, 100);
            ennumberst[1] = numbers[rand.Next(numbers.Length)];
            ennumberst[2] = rand.Next(1, 100);
            ennumberst[3] = numbers[rand.Next(numbers.Length)];

            //shufle personal numbers
            mynumberst = mynumberst.OrderBy(x => rand.Next()).ToArray();
            ennumberst = ennumberst.OrderBy(x => rand.Next()).ToArray();

            // Display the numbers in the labels
            mynumberslabel.Content += $" {mynumberst[0]}, {mynumberst[1]}, {mynumberst[2]}, {mynumberst[3]}";
            enemynamelabel.Content += $" {ennumberst[0]}, {ennumberst[1]}, {ennumberst[2]}, {ennumberst[3]}";

            // Shuffle and assign numbers to my buttons
            Random rnd = new Random();
            int[] mnumbers = numbers.OrderBy(x => rnd.Next()).ToArray();

            for (int i = 1; i <= 16; i++)
            {
                var btn = (Button)FindName($"m{i}");
                if (btn != null)
                {
                    btn.Content = mnumbers[i - 1];
                }
            }

            // Shuffle and assign numbers to enemy buttons
            int[] enumbers = numbers.OrderBy(x => rnd.Next()).ToArray();
            for (int i = 1; i <= 16; i++)
            {
                var btn = (Button)FindName($"e{i}");
                if (btn != null)
                {
                    btn.Content = enumbers[i - 1];
                }
            }

            nextround.IsEnabled = false;
            await Task.Delay(1500);
            nextround.IsEnabled = true;
            statuscsheckeringame.Content = "It is your turn";
            statuscsheckeringame.Foreground = Brushes.Green;
            var starter = rand.Next(1,16);
            var sbtn = (Button)FindName($"m{starter}");
            sbtn.Background = Brushes.Green;
            alreadyactme.Add(starter);

        }

        //PLAYGAME
        // Next round button
        public int curr = 0;
        public List<int> alreadyactme = new List<int>();
        public List<int> alreadyacten = new List<int>();
        public int i = 0;
        public List<string> mybuttons = new List<string>();
        public List<string> enemybuttons = new List<string>();

        public string[,] winning = new string[10, 4]
        {
            //sorok
            {"m1","m2","m3","m4" },
            {"m5","m6","m7","m8" },
            {"m9","m10","m11","m12" },
            {"m13","m14","m15","m16"  },

            //oszlopok
            {"m1","m5","m9","m13" },
            {"m2","m6","m10","m14" },
            {"m3","m7","m11","m15" },
            {"m4","m8","m12","m16" },

            //átlók
            {"m1","m6","m11","m16" },
            {"m4","m7","m10","m13" }
        };

        
        private void nextround_Click(object sender, RoutedEventArgs e)
        {
            //check if all numbers pulled
            if (alreadyactme.Count + alreadyacten.Count == 32)
            {
                statuscsheckeringame.Content = "All numbers have been pulled!";
                statuscsheckeringame.Foreground = Brushes.White;
                nextround.IsEnabled = false;
                return;
            }

            //enemyturn
            if (whoseround[curr] == "Player")
            {
                powerupwindowopen = false;
                powerupwindow.Visibility = Visibility.Hidden;
                statuscsheckeringame.Content = $"It is {whoseround[1]}'s turn";
                statuscsheckeringame.Foreground = Brushes.Red;

                var activate = 0;
                do
                {
                    activate = rand.Next(1, 17);
                }
                while (alreadyacten.Contains(activate));

                var btn = (Button)FindName($"e{activate}");
                btn.Background = Brushes.Red;
                alreadyacten.Add(activate);
                enemybuttons.Add($"m{activate}");

                for (i = 0; i < ennumberst.Length; i++)
                {
                    if ((int)btn.Content == ennumberst[i])
                    {
                        statuscsheckeringame.Content = $"Enemy pulled his number {ennumberst[i]}!";
                        statuscsheckeringame.Foreground = Brushes.Violet;
                        btn.Background = Brushes.Violet;
                    }
                }

                if (HasWinningLine(enemybuttons))
                {
                    statuscsheckeringame.Content = "Enemy Won! You Lose";
                    statuscsheckeringame.Foreground = Brushes.DarkRed;
                    nextround.IsEnabled = false;
                }
                curr = 1;
            }
            //myturn
            else if (whoseround[curr] != "Player")
            {
                powerupwindowopen = true;                
                statuscsheckeringame.Content = "It is your turn";
                statuscsheckeringame.Foreground = Brushes.Green;

                var activate = 0;
                do
                {
                    activate = rand.Next(1, 17);
                }
                while (alreadyactme.Contains(activate));

                var btn = (Button)FindName($"m{activate}");
                btn.Background = Brushes.Green;
                alreadyactme.Add(activate);
                mybuttons.Add($"m{activate}");


                for (i = 0; i < mynumberst.Length; i++)
                {
                    if((int)btn.Content == mynumberst[i])
                    {
                        statuscsheckeringame.Content = $"You pulled your number {mynumberst[i]}!";
                        statuscsheckeringame.Foreground = Brushes.Yellow;
                        btn.Background = Brushes.Yellow;
                    }
                }

               if (HasWinningLine(mybuttons))
                {

                    game.Visibility = Visibility.Hidden;
                    Won.Visibility = Visibility.Visible;
                    nightsurvivedwonsc.Content = "Nights survived: " + night;
                    night += 1;
                    moneyink += 20;

                }


                curr = 0;
            }
            
        }

       //Winninglane choice
        bool HasWinningLine(List<string> active)
        {
            for (int i = 0; i < winning.GetLength(0); i++)
            {
                bool allMatch = true;

                for (int j = 0; j < winning.GetLength(1); j++)
                {
                    if (!active.Contains(winning[i, j]))
                    {
                        allMatch = false;
                        break;
                    }
                }

                if (allMatch)
                    return true; // van nyerő sor
            }

            return false; // nincs nyerő sor
        }


        // Go to next night
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            StartButton_Click(sender, e);
            Won.Visibility = Visibility.Hidden;
            
        }



        // POWERUPS IN GAME
        // Open,close powerup window
        public bool powerupwindowopen = false;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (powerupwindowopen == false && curr == 0)
            {
                powerupwindow.Visibility = Visibility.Visible;
                numberchangerpowerup.Content = "Available: " + changenumber;
                enemymissroundpowerup.Content = "Available: " + enemymissround;
                wildcardpowerup.Content = "Available: " + wildcards;
                powerupwindowopen = true;
            }
            else if (powerupwindowopen == true)
            {
                powerupwindow.Visibility = Visibility.Hidden;
                powerupwindowopen = false;
            }
        }


        // Use number changer in game
        private void usenumberchanger_Click(object sender, RoutedEventArgs e)
        {
            if (changenumber > 0)
            {
                changenumber -= 1;
                numberchangerpowerup.Content = "Available: " + changenumber;
                var st = rand.Next(0, mynumberst.Length);
                var nd = rand.Next(0, numbers.Length);
                statuscsheckeringame.Content = $"Number {mynumberst[st]} changed to {numbers[nd]}";
                statuscsheckeringame.Foreground = Brushes.Blue;
                mynumberst[st] = numbers[nd];
                powerupwindow.Visibility = Visibility.Hidden;
                mynumberslabel.Content = $"Your numbers: {mynumberst[0]}, {mynumberst[1]}, {mynumberst[2]}, {mynumberst[3]}";
            }
        }

        // Use enemy miss round in game
        private void useenemymissround_Click(object sender, RoutedEventArgs e)
        {
            if(enemymissround > 0)
            {
                enemymissround -= 1;
                enemymissroundpowerup.Content = "Available: " + enemymissround;
                statuscsheckeringame.Content = $"{whoseround[1]}'s next round is skipped!";
                statuscsheckeringame.Foreground = Brushes.Blue;
                enemymissroundcount.Content = "Enemy Miss Rounds: " + enemymissround;
                powerupwindow.Visibility = Visibility.Hidden;
            }
        }

        // use wildcard in game
        private void kaka2_Click(object sender, RoutedEventArgs e)
        {
            if(wildcards > 0)
            {
                wildcards -= 1;
                wildcardcount.Content = "Available: " + wildcards;
                statuscsheckeringame.Content = "Wildcard used";
                statuscsheckeringame.Foreground = Brushes.Blue;
                wildcardpowerup.Content = "Available: " + wildcards;
                powerupwindow.Visibility = Visibility.Hidden;
            }
        }











        //TESTING CHEATS
        //infinit ink cheat for testing
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            moneyink = 9999999;

        }
        // Back to menu
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            game.Visibility = Visibility.Hidden;
            menu.Visibility = Visibility.Visible;
            powerupwindow.Visibility = Visibility.Hidden;
        }

    }
}



