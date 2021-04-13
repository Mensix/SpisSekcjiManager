using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlaywrightSharp;
using ShellProgressBar;

namespace SpisSekcjiManager.Utils
{
    public static class GroupUtils
    {
        public static async Task<Dataset> ParseGroups(IPage page, Dataset groups)
        {
            List<Group> newGroups = new();
            ProgressBarOptions options = new()
            {
                DisplayTimeInRealTime = false,
                ForegroundColor = ConsoleColor.White,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };

            using (var progressBar = new ProgressBar(groups.Groups.Count, "Parsing groups...", options))
            {
                for (int i = 0; i < groups.Groups.Count; i++)
                {
                    await page.GoToAsync($"https://mbasic.facebook.com/groups/{groups.Groups[i].Link}").ConfigureAwait(false);
                    bool groupExists = page.QuerySelectorAllAsync("h1 > div").Result.ToList().Count > 0;

                    if (groupExists)
                    {
                        newGroups.Add(new Group
                        {
                            Category = groups.Groups[i].Category,
                            Link = groups.Groups[i].Link,
                            Members = Convert.ToInt32(await page.QuerySelectorAsync("td + td > span").Result.GetInnerTextAsync().ConfigureAwait(false)),
                            Name = await page.QuerySelectorAsync("h1 > div").Result.GetInnerTextAsync().ConfigureAwait(false),
                            IsSection = groups.Groups[i].IsSection,
                            Keywords = groups.Groups[i].Keywords,
                            IsOpen = await page.QuerySelectorAsync("h1 + p").Result.GetInnerTextAsync().ConfigureAwait(false) == "Grupa Publiczna"
                        });
                    }
                    progressBar.Tick($"{i + 1}/{groups.Groups.Count}");
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
            Random random = new();
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

        public static Dataset CompareGroups(Dataset previousGroups, Dataset newGroups)
        {
            foreach (var n in newGroups.Groups)
            {
                n.MembersGrowth = n.Members - previousGroups.Groups.Find(x => x.Link == n.Link).Members != 0
                    ? n.Members - previousGroups.Groups.Find(x => x.Link == n.Link).Members
                    : null;
            }

            return new Dataset()
            {
                LastUpdateDate = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"),
                Name = newGroups.Name,
                Groups = newGroups.Groups
            };
        }
    }
}