using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SpisSekcjiManager.Utils;

namespace SpisSekcjiManager
{
    internal static class Program
    {
        private static async Task Main()
        {
            Setup setup = Setup.FromJson();
            Dataset oldGroups, newGroups;

            Console.WriteLine("=======================");
            Console.WriteLine("Spis Sekcji Manager v2");
            Console.WriteLine("=======================");

            Console.WriteLine("[1] - Naprawa grup");
            Console.WriteLine("[2] - Zaaktualizuj dane");
            Console.WriteLine("[3] - Odswiez ustawienia");
            Console.WriteLine("[4] - Wyczyść prośby o dodanie grup");
            Console.WriteLine();

            Console.Write("Podaj swoj wybor: ");
            string userChoice = Console.ReadLine();
            while (userChoice != "1" && userChoice != "2" && userChoice != "3" && userChoice != "4")
            {
                Console.WriteLine("Niewlasciwy wybor!");
                Console.WriteLine();
                Console.Write("Podaj swoj wybor: ");
                userChoice = Console.ReadLine();
            }

            Console.Clear();
            if (userChoice == "1")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    oldGroups = Dataset.FromJson($"{setup.Files[i].Input}");
                    GroupUtils.FixGroups(oldGroups).ToJson($"{setup.Files[i].Output}.json");
                }
            }
            else if (userChoice == "2")
            {
                for (int i = 0; i < setup.Files.Count; i++)
                {
                    newGroups = Dataset.FromJson($"{setup.Files[i].Input}");
                    await FirebaseUtils.PostGroups(newGroups, setup, i).ConfigureAwait(false);
                }
            }
            else if (userChoice == "3")
            {
                Setup.FromJson();
            }
            else if (userChoice == "4")
            {
                await FirebaseUtils.ClearSubmissions(setup).ConfigureAwait(false);
            }

            await Main().ConfigureAwait(false);
        }
    }
}