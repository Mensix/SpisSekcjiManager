using System;
using System.Collections.Generic;
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
            Dataset oldGroups, newGroups, deadGroups;

            using IPlaywright playwright = await Playwright.CreateAsync().ConfigureAwait(false);
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
                    oldGroups = Dataset.FromJson($"{setup.Files[i].Input}");
                    (newGroups, deadGroups) = await GroupUtils.ParseGroups(page, oldGroups).ConfigureAwait(false);

                    if (setup.Settings.AutoFix) newGroups = GroupUtils.FixGroups(newGroups);
                    if (setup.Settings.AutoCompare) newGroups = GroupUtils.CompareGroups(oldGroups, newGroups);
                    if (setup.Settings.AutoUpdate) await FirebaseUtils.PostGroups(newGroups, setup, i).ConfigureAwait(false);
                    if (setup.Settings.ShouldParseHades && oldGroups.Name == "sections") await FirebaseUtils.PostHades(deadGroups, setup).ConfigureAwait(false);
                    if (setup.Settings.ShouldUpdateArchive && oldGroups.Name == "sections")
                    {
                        List<Archive> archive = Archive.FromJson("archive");
                        GroupUtils.GenerateArchive(archive, newGroups).ToJson("archive-o.json");
                        await FirebaseUtils.PostArchive(archive, setup).ConfigureAwait(false);
                    }

                    newGroups.ToJson($"{setup.Files[i].Output}");
                }
            }
            else if (userChoice == "2")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    oldGroups = Dataset.FromJson($"{setup.Files[i].Input}");
                    GroupUtils.FixGroups(oldGroups).ToJson($"{setup.Files[i].Output}.json");
                }
            }
            else if (userChoice == "3")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    newGroups = Dataset.FromJson($"{setup.Files[i].Input}");
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