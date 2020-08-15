using System;
using System.IO;
using OpenQA.Selenium.Chrome;
using SpisSekcjiManager.Utils;

namespace SpisSekcjiManager
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("start-maximized");
            chromeOptions.AddArgument("enable-automation");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-infobars");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--disable-browser-side-navigation");
            chromeOptions.AddArgument("--disable-gpu");

            Setup setup = Setup.FromJson(File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/data/settings.json"));

            Console.Clear();
            Console.WriteLine("=======================");
            Console.WriteLine("Spis Sekcji Manager v1");
            Console.WriteLine("=======================");

            Console.WriteLine("[1] - Parsowanie grup");
            Console.WriteLine("[2] - Naprawa grup");
            Console.WriteLine("[3] - Przyrownaj grupy");
            Console.WriteLine("[4] - Zaaktualizuj dane");
            Console.WriteLine("[5] - Odswiez ustawienia");
            Console.WriteLine("[6] - Zresetuj status");
            Console.WriteLine("[7] - Wyczyść prośby o dodanie grup");
            Console.WriteLine();

            Console.Write("Podaj swoj wybor: ");
            string userChoice = Console.ReadLine();
            while (userChoice != "1" && userChoice != "2" && userChoice != "3" && userChoice != "4" && userChoice != "5" && userChoice != "6" && userChoice != "7")
            {
                Console.WriteLine("Niewlasciwy wybor!");
                Console.WriteLine();
                Console.Write("Podaj swoj wybor: ");
                userChoice = Console.ReadLine();
            }

            Console.Clear();
            Dataset oldGroups, newGroups;

            if (userChoice == "1")
            {
                ChromeDriver chromeDriver = new ChromeDriver(AppDomain.CurrentDomain.BaseDirectory, chromeOptions);
                User user = new User() { Email = setup.Login, Password = setup.Password };
                chromeDriver.FacebookLogin(user);

                for (int i = 0; i < setup.Files.Count; i++)
                {
                    oldGroups = Dataset.FromJson(File.ReadAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Input}")));
                    newGroups = GroupUtils.ParseGroups(chromeDriver, setup, oldGroups);

                    if (setup.Settings.AutoFix == true) newGroups = GroupUtils.FixGroups(newGroups);
                    if (setup.Settings.AutoCompare == true) newGroups = GroupUtils.CompareGroups(oldGroups, newGroups);
                    if (setup.Settings.AutoUpdate == true && setup.Files[i].Path != null) FirebaseUtils.PostGroups(newGroups, setup, i);
                    if (i == setup.Files.Count - 1 && setup.Settings.ShouldUpdateStatus == true) FirebaseUtils.InitStatus(setup, newGroups);

                    File.WriteAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Output}"), newGroups.ToJson());
                }
            }
            else if (userChoice == "2")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    oldGroups = Dataset.FromJson(File.ReadAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Input}")));
                    File.WriteAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Output}"), GroupUtils.FixGroups(oldGroups).ToJson());
                }
            }
            else if (userChoice == "3")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    oldGroups = Dataset.FromJson(File.ReadAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Input}")));
                    newGroups = Dataset.FromJson(File.ReadAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Output}")));
                    File.WriteAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Output}"), GroupUtils.CompareGroups(oldGroups, newGroups).ToJson());
                }
            }
            else if (userChoice == "4")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    newGroups = Dataset.FromJson(File.ReadAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Input}")));
                    if (setup.Files[i].Path != null) FirebaseUtils.PostGroups(newGroups, setup, i);
                }
            }
            else if (userChoice == "5")
            {
                Setup.FromJson(File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/data/settings.json"));
            }
            else if (userChoice == "6")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    newGroups = Dataset.FromJson(File.ReadAllText(Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}/data/{setup.Files[i].Input}")));
                    FirebaseUtils.InitStatus(setup, newGroups);
                }
            }
            else if (userChoice == "7")
            {
                FirebaseUtils.ClearSubmissions(setup);
            }

            Console.Clear();
            Main(args);
        }
    }
}