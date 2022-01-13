using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Android.Util;

using ExpiringBarcodesAndroidAppNew.Handler;

using Shared;

namespace ExpiringBarcodesAndroidAppNew
{
    [Activity(Label = "Main Activity", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int LOGIN_ACTIVITY = 0;
        private User user;
        private Barcode barcodeGenerator;
        private System.Timers.Timer timer;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            
            SetContentView(Resource.Layout.activity_main);

            user = new User();

            Button btnLogOut = FindViewById<Button>(Resource.Id.btnLogOut);

            btnLogOut.Click += async (object sender, EventArgs e) =>
            {
                await LogOut();
            };

            Log.Debug("MK", "Logout Click listener initialized.");

            if (!user.IsLoggedIn)
            {
                RequireLogin();
            }
            else
            {
                try
                {
                    if (string.IsNullOrEmpty(user.GetProperty(UserConstants.MemberID)))
                    {
                        var api = new Api(user);
                        await api.GetMembershipId();
                    }
                    if (string.IsNullOrEmpty(user.GetProperty(UserConstants.SECRET)))
                    {
                        var api = new Api(user);
                        await api.RequestSharedKey();
                    }

                    Initialize();
                }
                catch (UnauthorizedAccessException)
                {
                    await LogOut();
                }
            }
        }

        private void RequireLogin()
        {
            var intent = new Intent(this, typeof(LoginActivity));
            StartActivityForResult(intent, LOGIN_ACTIVITY);
        }
        private async Task LogOut()
        {
            if (timer.Enabled)
            {
                timer.Stop();
            }
            var api = new Api(user);
            await api.LogOut();
            user.RequestLogout();
            Toast.MakeText(this, "You have ben logged out.", ToastLength.Short).Show();
            RequireLogin();
        }

        private void Initialize()
        {
            TextView memberIdField = FindViewById<TextView>(Resource.Id.memberID);
            TextView secretField = FindViewById<TextView>(Resource.Id.secret);
            TextView loggedInAsField = FindViewById<TextView>(Resource.Id.loggedInAs);

            var secret = Convert.FromBase64String(user.GetProperty(UserConstants.SECRET));
            var secretHumanString = secret.ByteArrToStr();

            RunOnUiThread(() => loggedInAsField.Text = user.GetProperty(UserConstants.Email));
            RunOnUiThread(() => memberIdField.Text = user.GetProperty(UserConstants.MemberID));
            RunOnUiThread(() => secretField.Text = secretHumanString);

            this.barcodeGenerator = new Barcode(this.user);
            this.timer = new System.Timers.Timer
            {
                Interval = 1000
            };
            this.timer.Elapsed += OnTimedEvent;
            this.timer.Enabled = true;
            RefreshBarcode();
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RefreshBarcode();
        }

        private void RefreshBarcode()
        {
            var barcodeImage = FindViewById<ImageView>(Resource.Id.barcodeImage);
            TextView codeField = FindViewById<TextView>(Resource.Id.TOTP);

            var barcode = barcodeGenerator.CurrentBarCode();
            var code = barcodeGenerator.CurrentCode();
            RunOnUiThread(() => barcodeImage.SetImageBitmap(barcode));
            RunOnUiThread(() => codeField.Text = code);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == LOGIN_ACTIVITY)
            {
                if (resultCode == Result.Ok)
                {
                    //reset user
                    user = new User();
                    Initialize();
                }
                else
                {
                    RequireLogin();
                }
            }
        }
    }
}