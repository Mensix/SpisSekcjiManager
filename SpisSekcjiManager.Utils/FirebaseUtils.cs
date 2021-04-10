using Firebase.Database;
using Firebase.Database.Query;

namespace SpisSekcjiManager.Utils
{
    public static class FirebaseUtils
    {
        public async static void PostGroups(Dataset dataset, Setup setup, int index)
        {
            var firebase = new FirebaseClient(setup.Settings.FirebaseLink);
            string newGroupsStr = dataset.ToJson();
            await firebase.Child(setup.Files[index].Path).PutAsync(newGroupsStr).ConfigureAwait(false);
        }

        public async static void ClearSubmissions(Setup setup) {
            var firebase = new FirebaseClient(setup.Settings.FirebaseLink);
            await firebase.Child("submissions").DeleteAsync().ConfigureAwait(false);
        }
    }
}