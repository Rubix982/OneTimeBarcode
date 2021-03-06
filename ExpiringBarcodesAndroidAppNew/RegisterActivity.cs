using System;
using System.Net;
using Android.OS;
using System.Linq;
using Android.App;
using Android.Util;
using Android.Widget;
using Android.Content;
using ExpiringBarcodesAndroidAppNew.Handler;

namespace ExpiringBarcodesAndroidAppNew
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        private Api api;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Log.Info("MK", "OnCreate: Register Activity");

            this.api = new Api(new User());
            // Create your application here
            SetContentView(Resource.Layout.Register);
            
            EditText username = FindViewById<EditText>(Resource.Id.username);
            EditText password = FindViewById<EditText>(Resource.Id.password);
            EditText confirmpassword = FindViewById<EditText>(Resource.Id.confirmpassword);
            Button btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);

            btnRegister.Click += async (object sender, EventArgs e) =>
            {
                try
                {
                    var res = await api.Register(username.Text, password.Text, confirmpassword.Text);
                    if (res.Error && res.ModelState.Any())
                    {
                        Toast.MakeText(this, res.ModelState.First().Value.First(), ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "Registration Complete", ToastLength.Short).Show();
                        this.Finish();
                    }
                }
                catch (WebException)
                {
                    Toast.MakeText(this, "Unstable server connection", ToastLength.Short).Show();
                }
            };
            btnLogin.Click += async (object sender, EventArgs e) =>
            {
                var intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);
            };
        }
    }
}