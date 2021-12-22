using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExpiringBarcodeDemo.Handler;

namespace ExpiringBarcodeDemo
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        private Api api;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.api = new Api(new User());
            // Create your application here
            SetContentView(Resource.Layout.Register);
            
            EditText username = FindViewById<EditText>(Resource.Id.username);
            EditText password = FindViewById<EditText>(Resource.Id.password);
            EditText confirmpassword = FindViewById<EditText>(Resource.Id.confirmpassword);
            Button btnRegister = FindViewById<Button>(Resource.Id.btnRegister);

            btnRegister.Click += async (object sender, EventArgs e) =>
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
            };
        }
    }
}