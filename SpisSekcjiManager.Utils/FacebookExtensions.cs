using System.Threading.Tasks;
using PlaywrightSharp;

namespace SpisSekcjiManager.Utils
{
    public static class FacebookExtensions
    {
        public static async Task Login(this IPage page, string email, string password)
        {
            await page.GoToAsync("https://mbasic.facebook.com/login.php/").ConfigureAwait(false);
            await page.ClickAsync("button.t").ConfigureAwait(false);
            await page.FillAsync("input[name='email']", email).ConfigureAwait(false);
            await page.FillAsync("input[name='pass']", password).ConfigureAwait(false);
            await page.ClickAsync("input[name='login']").ConfigureAwait(false);
        }
    }
}