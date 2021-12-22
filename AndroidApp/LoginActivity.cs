using System;
using System.Net;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using ExpiringBarcodeDemo.Handler;
using ExpiringBarcodeDemo;
using Xamarin.Auth;


namespace ExpiringBarcodeDemo
{
    [Activity(Label = "Please Log In")]
    public class LoginActivity : Activity
    {
        private User user;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            user = new User();

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Login);

            EditText username = FindViewById<EditText>(Resource.Id.username);
            EditText password = FindViewById<EditText>(Resource.Id.password);
            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            Button btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            
            btnLogin.Click += async (object sender, EventArgs e) =>
            {
                try
                {
                    var api = new Api(user);
                    if (await api.Login(username.Text, password.Text))
                    {
                        await api.GetMembershipId();
                        await api.RequestSharedKey();

                        Toast.MakeText(this, "You have successfully signed in.", ToastLength.Short).Show();

                        this.SetResult(Result.Ok);
                        this.Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, "Username or Password is Incorrect", ToastLength.Short).Show();
                    }
                }
                catch (WebException)
                {
                    Toast.MakeText(this, "Unstable server connection", ToastLength.Short).Show();
                }
            };
            btnRegister.Click += async (object sender, EventArgs e) =>
            {
                var intent = new Intent(this, typeof(RegisterActivity));
                StartActivity(intent);
            };
        }
        public override void OnBackPressed()
        {
            //prevent back button
        }

    }
}

