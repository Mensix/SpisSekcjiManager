using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace SpisSekcjiManager.Utils
{
    public static class FirebaseUtils
    {
        public async static Task PostGroups(Dataset dataset, Setup setup, int index)
        {
            FirebaseClient firebase = new(setup.Settings.FirebaseLink);
            await firebase.Child(setup.Files[index].Input).PutAsync(SerializeDataset.ToString(dataset)).ConfigureAwait(false);
        }

        public async static Task ClearSubmissions(Setup setup)
        {
            FirebaseClient firebase = new(setup.Settings.FirebaseLink);
            await firebase.Child("submissions").DeleteAsync().ConfigureAwait(false);
        }
    }
}