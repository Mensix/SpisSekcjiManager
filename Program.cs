using System;
using System.IO;
using System.Threading.Tasks;
using PlaywrightSharp;
using PlaywrightSharp.Chromium;
using SpisSekcjiManager.Utils;

namespace SpisSekcjiManager
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            string dataDirectory = $"{Directory.GetCurrentDirectory()}/data";
            Setup setup = Setup.FromJson();
            Dataset oldGroups, newGroups;

            using var playwright = await Playwright.CreateAsync().ConfigureAwait(false);
            await using IChromiumBrowser browser = await playwright.Chromium.LaunchAsync().ConfigureAwait(false);
            IPage page = await browser.NewPageAsync().ConfigureAwait(false);

            Console.WriteLine("=======================");
            Console.WriteLine("Spis Sekcji Manager v2");
            Console.WriteLine("=======================");

            Console.WriteLine("[1] - Parsowanie grup");
            Console.WriteLine("[2] - Naprawa grup");
            Console.WriteLine("[3] - Zaaktualizuj dane");
            Console.WriteLine("[4] - Odswiez ustawienia");
            Console.WriteLine("[5] - Wyczyść prośby o dodanie grup");
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

            if (userChoice == "1")
            {
                await page.Login(setup.Login, setup.Password).ConfigureAwait(false);

                for (int i = 0; i < setup.Files.Count; i++)
                {
                    oldGroups = Dataset.FromJson($"{setup.Files[i]}");
                    newGroups = await GroupUtils.ParseGroups(page, oldGroups).ConfigureAwait(false);

                    if (setup.Settings.AutoFix == true) newGroups = GroupUtils.FixGroups(newGroups);
                    if (setup.Settings.AutoCompare == true) newGroups = GroupUtils.CompareGroups(oldGroups, newGroups);
                    if (setup.Settings.AutoUpdate == true) await FirebaseUtils.PostGroups(newGroups, setup, i).ConfigureAwait(false);

                    newGroups.ToJson($"{setup.Files[i]}-o.json");
                }
            }
            else if (userChoice == "2")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    oldGroups = Dataset.FromJson($"{setup.Files[i]}");
                    GroupUtils.FixGroups(oldGroups).ToJson($"{setup.Files[i]}-o.json");
                }
            }
            else if (userChoice == "3")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    newGroups = Dataset.FromJson($"{setup.Files[i]}");
                    await FirebaseUtils.PostGroups(newGroups, setup, i).ConfigureAwait(false);
                }
            }
            else if (userChoice == "4")
            {
                Setup.FromJson();
            }
            else if (userChoice == "5")
            {
                await FirebaseUtils.ClearSubmissions(setup).ConfigureAwait(false);
            }

        }
    }
}