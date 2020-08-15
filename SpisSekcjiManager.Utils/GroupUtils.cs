using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ShellProgressBar;

namespace SpisSekcjiManager.Utils
{
    public static class GroupUtils
    {
        public static Dataset ParseGroups(ChromeDriver chromeDriver, Setup setup, Dataset groups)
        {
            int oldGroupsCount = groups.Groups.Count;
            List<Group> newGroups = new List<Group>();

            int totalTicks = groups.Groups.Count;
            var options = new ProgressBarOptions
            {
                DisplayTimeInRealTime = false,
                ForegroundColor = ConsoleColor.White,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };

            using (var progressBar = new ProgressBar(totalTicks, "Parsing groups...", options))
            {
                for (int i = 0; i < groups.Groups.Count; i++)
                {
                    try
                    {
                        chromeDriver.Navigate().GoToUrl($"https://mbasic.facebook.com/groups/{groups.Groups[i].Link}");
                        bool isGroupPresent = chromeDriver.FindElements(By.CssSelector("h1 > div")).Count > 0;
                        if (isGroupPresent)
                        {
                            newGroups.Add(new Group
                            {
                                Category = groups.Groups[i].Category,
                                Link = groups.Groups[i].Link,
                                Members = ParseMembers(chromeDriver),
                                Name = HttpUtility.HtmlDecode(chromeDriver.FindElement(By.CssSelector("h1 > div")).GetAttribute("innerText")),
                                IsSection = groups.Groups[i].IsSection,
                                Keywords = groups.Groups[i].Keywords,
                                IsOpen = chromeDriver.FindElement(By.CssSelector("h1 + p")).GetAttribute("innerText") == "Grupa Publiczna"
                            });
                        }
                        progressBar.Tick($"{i + 1}/{oldGroupsCount}");
                        if (setup.Settings.ShouldUpdateStatus == true) FirebaseUtils.UpdateStatus(setup, groups, i, oldGroupsCount);
                    }
                    catch (Exception)
                    {
                        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + $"/data/{groups.Name}_ERR"), new Dataset
                        {
                            LastUpdateDate = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"),
                            Name = groups.Name,
                            Groups = newGroups
                        }.ToJson());
                    }
                }
            }

            return new Dataset
            {
                LastUpdateDate = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"),
                Name = groups.Name,
                Groups = newGroups
            };
        }

        public static Dataset FixGroups(Dataset groups)
        {
            Random random = new Random();
            return new Dataset()
            {
                LastUpdateDate = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"),
                Name = groups.Name,
                Groups = groups.Groups
                                .Select(x =>
                                {
                                    if (x.Name == null)
                                    {
                                        x.Name = random.Next().ToString();
                                        return x;
                                    }
                                    return x;
                                })
                                .GroupBy(x => x.Name)
                                .Select(x => x.OrderByDescending(i => i.MembersGrowth).First())
                                .GroupBy(x => x.Link)
                                .Select(x => x.OrderByDescending(i => i.Members).First())
                                .OrderByDescending(x => x.Members)
                                .Select(x =>
                                {
                                    if (Regex.IsMatch(x.Name, "^[0-9]*$"))
                                    {
                                        x.Name = null;
                                        return x;
                                    }
                                    if (x.IsOpen == false)
                                    {
                                        x.IsOpen = null;
                                        return x;
                                    }
                                    return x;
                                })
                                .ToList()
            };
        }

        public static Dataset CompareGroups(Dataset oldGroups, Dataset newGroups)
        {
            foreach (var n in newGroups.Groups)
            {
                n.MembersGrowth = n.Members - oldGroups.Groups.Find(x => x.Link == n.Link).Members != 0
                    ? n.Members - oldGroups.Groups.Find(x => x.Link == n.Link).Members
                    : null;
            }

            return new Dataset()
            {
                LastUpdateDate = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"),
                Name = newGroups.Name,
                Groups = newGroups.Groups
            };
        }

        private static int ParseMembers(ChromeDriver chromeDriver) => chromeDriver.FindElements(By.CssSelector("td > span")).Count > 0 ? Convert.ToInt32(chromeDriver.FindElement(By.CssSelector("td + td > span")).GetAttribute("innerHTML")) : 0;
    }
}