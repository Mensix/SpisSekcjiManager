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

        public async static void UpdateStatus(Setup setup, Dataset dataset, int current, int total)
        {
            var firebase = new FirebaseClient(setup.Settings.FirebaseLink);
            await firebase.Child("update").Child(dataset.Name).PutAsync(new { current, total }).ConfigureAwait(false);
        }

        public async static void InitStatus(Setup setup, Dataset dataset) {
            var firebase = new FirebaseClient(setup.Settings.FirebaseLink);
            await firebase.Child("update").Child("wasDone").PutAsync(true).ConfigureAwait(false);
            await firebase.Child("update").Child(dataset.Name).PutAsync(new { current = 0, total = 0 }).ConfigureAwait(false);
        }

        public async static void ClearSubmissions(Setup setup) {
            var firebase = new FirebaseClient(setup.Settings.FirebaseLink);
            await firebase.Child("submissions").DeleteAsync().ConfigureAwait(false);
        }
    }
}