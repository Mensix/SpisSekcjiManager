using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpisSekcjiManager.Utils
{
    public static class GroupUtils
    {
        private static readonly string todayDate = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/");
        public static Dataset FixGroups(Dataset groups)
        {
            Random random = new();
            return new Dataset()
            {
                LastUpdateDate = todayDate,
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
            foreach (Group g in newGroups.Groups)
            {
                g.MembersGrowth = g.Members - previousGroups.Groups.Find(x => x.Link == g.Link).Members;
            }

            return new Dataset()
            {
                LastUpdateDate = todayDate,
                Name = previousGroups.Name,
                Groups = newGroups.Groups
            };
        }
    }
}