using System.Linq;
using Xamarin.Auth;

namespace ExpiringBarcodesAndroidAppNew.Handler
{
    public class User
    {
        private Account acc;

        public User()
        {
            this.acc =
                AccountStore.Create().FindAccountsForService("ExpiringBarcode").FirstOrDefault(a => a.Username.Equals("User"));
            if (this.acc == null)
            {
                this.acc = new Account("User"); //lets just use one user
            }
        }

        public string GetProperty(string prop)
        {
            if (this.acc.Properties.ContainsKey(prop))
            {
                return this.acc.Properties[prop];
            }
            else
            {
                return "";
            }
        }

        public void SetProperty(string prop, string value)
        {
            this.acc.Properties[prop] = value;
            AccountStore.Create().Save(acc, "ExpiringBarcode");
        }
        
        public bool IsLoggedIn
        {
            get { return this.acc != null && !string.IsNullOrEmpty(this.GetProperty(UserConstants.Token)); }
        }

        public void RequestLogout()
        {
            AccountStore.Create().Delete(this.acc, "ExpiringBarcode");
            this.acc = new Account("User");
        }
    }
}