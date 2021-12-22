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

namespace ExpiringBarcodeDemo.Handler
{
    public static class UserConstants
    {
        public static readonly string Email = "Email";
        public static readonly string Token = "Token";
        public static readonly string MemberID = "MemberID";
        public static readonly string SECRET = "BarcodeSharedSecret";
    }
}