using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics; // Kell a Trace-hez

namespace Hellbingojatel
{
    public partial class MainWindow : Window
    {
        // --- 1. JAVÍTÁS: Class használata Struct helyett ---
        // Így már módosíthatóak lesznek az adatok a listában (CS1612 hiba megoldva)
        public class EnPlayers
        {
            public string enPlayerName;
            public int[] enPlayerNumbers;
            public int enPlayPulledNums;
        }
        
        // Ellenségek listája
        List<EnPlayers> enPlayer = new List<EnPlayers>();

        // Saját játékos osztálya
        public class MyPlayer
        {
            public int[] myPlayerNumbers;
            public int myPlayPulledNums; 
        }
        MyPlayer player = new MyPlayer();

        // --- 2. JAVÍTÁS: A kihúzott számok listája itt van, nem a gombnyomásban ---
        // Így nem törlődik minden körben.
        public List<int> pulledNumbers = new List<int>();

        // Pénz és powerup változók
        public int moneyink = 15;
        public int changenumber = 0;
        public int enemymissround = 0;
        public int wildcards = 0;
        public int night = 1;

        // Powerup ablak állapota
        public bool powerupwindowopen = false;

        public MainWindow()
        {
            InitializeComponent();
            
            // Kezdeti láthatóságok beállítása
            game.Visibility = Visibility.Hidden;
            buyphase.Visibility = Visibility.Hidden;
            Won.Visibility = Visibility.Hidden;
            powerupwindow.Visibility = Visibility.Hidden;
        }

        // --- START BUY PHASE (Bolt megnyitása) ---
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            menu.Visibility = Visibility.Hidden;
            game.Visibility = Visibility.Hidden;
            buyphase.Visibility = Visibility.Visible;
            powerupwindow.Visibility = Visibility.Hidden;

            UpdateUI(); // Kiírjuk a pénzt és a powerupokat
        }

        // Segédfüggvény a UI frissítéshez (hogy ne kelljen mindig leírni ugyanazt)
        private void UpdateUI()
        {
            inkcount.Content = "Your Inks: " + moneyink;
            numberchangercount.Content = "Your Number Changes: " + changenumber;
            enemymissroundcount.Content = "Your Enemy Miss Rounds: " + enemymissround;
            wildcardcount.Content = "Your Wildcards: " + wildcards;
        }

        // --- GAME START (Játék indítása) ---
        public async void Button_Click(object sender, RoutedEventArgs e)
        {
            game.Visibility = Visibility.Visible;
            buyphase.Visibility = Visibility.Hidden;
            menu.Visibility = Visibility.Hidden;
            Won.Visibility = Visibility.Hidden;

            // FONTOS: Töröljük az előző játék adatait!
            enPlayer.Clear();
            pulledNumbers.Clear(); 

            var random = new Random();

            // 1. Ellenségek generálása (50 db)
            for (int i = 0; i < 50; i++)
            {
                EnPlayers enemyPlayer = new EnPlayers();
                enemyPlayer.enPlayerName = "player" + (i + 1);
                // A számgenerálást kiszerveztük egy külön függvénybe (lásd lentebb)
                enemyPlayer.enPlayerNumbers = GenerateBingoNumbers(random); 
                enemyPlayer.enPlayPulledNums = 0;
                enPlayer.Add(enemyPlayer);
            }

            // 2. Saját játékos generálása
            player.myPlayerNumbers = GenerateBingoNumbers(random);
            player.myPlayPulledNums = 0;

            nightscount.Content = "Night: " + night;

            // 3. Gombok beállítása (m1, m2 ... m15)
            int index = 1;
            foreach (var num in player.myPlayerNumbers)
            {
                var btn = (Button)FindName($"m{index}");
                if (btn != null)
                {
                    btn.Content = num;
                    btn.Background = Brushes.White; // Visszaállítjuk fehérre
                }
                index++;
            }

            // Diagnosztika kiírása
            foreach (var item in enPlayer)
            {
                Trace.WriteLine("Enemy: " + item.enPlayerName + " Nums: " + string.Join(", ", item.enPlayerNumbers));
            }

            // Kis várakozás a kezdés előtt
            nextround.IsEnabled = false;
            await Task.Delay(1500);
            nextround.IsEnabled = true;
        }

        // --- SEGÉDFÜGGVÉNY: 15 EGYEDI SZÁM GENERÁLÁSA ---
        private int[] GenerateBingoNumbers(Random random)
        {
            HashSet<int> numbers = new HashSet<int>();
            // Addig generálunk, amíg nincs meg a 15 különböző szám
            while (numbers.Count < 15)
            {
                numbers.Add(random.Next(1, 91));
            }
            return numbers.ToArray();
        }

        // --- NEXT ROUND (Számhúzás) ---
        private void nextround_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            
            // Ha már minden számot kihúztak (biztonsági ellenőrzés)
            if (pulledNumbers.Count >= 90)
            {
                MessageBox.Show("Vége a játéknak, minden szám kihúzva!");
                return;
            }

            // 1. Új szám húzása (ami még nem szerepel a pulledNumbers listában)
            int currentPulledNumber = 0;
            bool isNew = false;
            while (!isNew)
            {
                currentPulledNumber = random.Next(1, 91);
                if (!pulledNumbers.Contains(currentPulledNumber))
                {
                    pulledNumbers.Add(currentPulledNumber);
                    isNew = true;
                }
            }

            statuscsheckeringame.Content = "Number Pulled: " + currentPulledNumber;

            // 2. Ellenségek ellenőrzése
            foreach (var enemy in enPlayer)
            {
                // Mivel Class-t használunk, ez a módosítás megmarad!
                if (enemy.enPlayerNumbers.Contains(currentPulledNumber))
                {
                    enemy.enPlayPulledNums++;
                    
                    // Nyert az ellenség?
                    if (enemy.enPlayPulledNums >= 15)
                    {
                        MessageBox.Show(enemy.enPlayerName + " nyert! Vesztettél.");
                        // Itt lehetne resetelni a játékot vagy visszalépni a menübe
                        return;
                    }
                }
            }

            // 3. Saját számok ellenőrzése
            bool numberFound = false;
            for (int i = 0; i < player.myPlayerNumbers.Length; i++)
            {
                if (player.myPlayerNumbers[i] == currentPulledNumber)
                {
                    numberFound = true;
                    player.myPlayPulledNums++;

                    // Megkeressük a megfelelő gombot és zöldre színezzük
                    var btn = (Button)FindName($"m{i + 1}");
                    if (btn != null)
                    {
                        btn.Background = Brushes.LightGreen;
                        btn.Content = "X " + player.myPlayerNumbers[i];
                    }

                    // Nyertünk?
                    if (player.myPlayPulledNums >= 15)
                    {
                        Won.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        // --- KÖVETKEZŐ ÉJSZAKA (Next Night) ---
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            night++;
            StartButton_Click(sender, e);
            Won.Visibility = Visibility.Hidden;
        }

        // --- POWERUPOK VÁSÁRLÁSA ---

        // Buy number changer (Ára: 5)
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BuyItem(5, ref changenumber, "Number Changer");
        }

        // Buy miss round (Ára: 10)
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BuyItem(10, ref enemymissround, "Enemy Miss Round");
        }

        // Buy wildcard (Ára: 15)
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            BuyItem(15, ref wildcards, "Wildcard");
        }

        // Közös vásárló függvény
        private void BuyItem(int cost, ref int itemStock, string itemName)
        {
            if (moneyink >= cost)
            {
                moneyink -= cost;
                itemStock++;
                UpdateUI();
                currentstatus.Content = $"You bought a {itemName}!";
                currentstatus.Foreground = Brushes.Green;
            }
            else
            {
                currentstatus.Content = "Not enough Inks!";
                currentstatus.Foreground = Brushes.Red;
            }
        }

        // --- EGYÉB FUNKCIÓK ---

        // Powerup ablak nyitás/zárás
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            powerupwindowopen = !powerupwindowopen;
            if (powerupwindowopen)
            {
                powerupwindow.Visibility = Visibility.Visible;
                numberchangerpowerup.Content = "Available: " + changenumber;
                enemymissroundpowerup.Content = "Available: " + enemymissround;
                wildcardpowerup.Content = "Available: " + wildcards;
            }
            else
            {
                powerupwindow.Visibility = Visibility.Hidden;
            }
        }

        // Powerup használat (egyelőre üres)
        private void usenumberchanger_Click(object sender, RoutedEventArgs e) { }
        private void useenemymissround_Click(object sender, RoutedEventArgs e) { }
        private void kaka2_Click(object sender, RoutedEventArgs e) { }

        // Végtelen pénz cheat
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            moneyink = 9999999;
            UpdateUI();
        }

        // Vissza a menübe
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            game.Visibility = Visibility.Hidden;
            menu.Visibility = Visibility.Visible;
            powerupwindow.Visibility = Visibility.Hidden;
        }
    }
}