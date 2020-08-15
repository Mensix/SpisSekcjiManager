using NUnit.Framework;

namespace SpisSekcjiManager.Tests
{
    [TestFixture]
    public class CompareGroupsTest
    {
        [Test]
        public void IsComparingGroups()
        {
            var oldGroups = Dataset.FromJson("{\"name\":\"testing\",\"lastUpdateDate\":\"15/03/2020\",\"groups\":[{\"category\":\"Humor\",\"link\":\"https://facebook.com/groups/kopypasty\",\"members\":74990,\"name\":\"Sekcja past\"},{\"link\":\"https://facebook.com/groups/postnostalgawka\",\"members\":68372,\"name\":\"Jak będzie w 2030?\"}]}");
            var newGroups = Dataset.FromJson("{\"name\":\"testing\",\"lastUpdateDate\":\"17/03/2020\",\"groups\":[{\"category\":\"Humor\",\"link\":\"https://facebook.com/groups/kopypasty\",\"members\":74987,\"name\":\"Sekcja past\"},{\"link\":\"https://facebook.com/groups/postnostalgawka\",\"members\":68915,\"name\":\"Jak będzie w 2030?\"}]}");

            var comparedGroups = Utils.GroupUtils.CompareGroups(oldGroups, newGroups);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(comparedGroups.Groups.Find(x => x.Name == "Sekcja past").MembersGrowth, -3);
                Assert.AreEqual(comparedGroups.Groups.Find(x => x.Name == "Jak będzie w 2030?").MembersGrowth, 543);
            });
        }
    }
}