using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Shared;
using ZXing;
using ZXing.Common;

namespace ExpiringBarcodeDemo.Handler
{
    public class Barcode
    {
        private User user;
        private string memberid;
        private TOTP totp;
        private Bitmap cacheBitmap;
        private string cacheData;

        public Barcode(User _user)
        {
            if (!_user.IsLoggedIn || string.IsNullOrEmpty(_user.GetProperty(UserConstants.SECRET)))
            {
                throw new UnauthorizedAccessException();
            }
            this.user = _user;
            this.memberid = user.GetProperty(UserConstants.MemberID);
            var secret = user.GetProperty(UserConstants.SECRET);
            var secretBytes = Convert.FromBase64String(secret);
            this.totp = new TOTP(secretBytes);
            this.cacheBitmap = null;
            this.cacheData = "";
        }

        public Bitmap CurrentBarCode()
        {
            var data = memberid + totp.GetCode();
            if (this.cacheData.Equals(data) && this.cacheBitmap != null)
            {
                return this.cacheBitmap;
            }
            var barcode = this.makeBarcode(data);
            this.cacheData = data;
            this.cacheBitmap = barcode;
            return barcode;
        }

        public string CurrentCode()
        {
            return this.totp.GetCode();
        }

        private Bitmap makeBarcode(string data)
        {
            var writer = new ZXing.Mobile.BarcodeWriter
            {
                Format = BarcodeFormat.PDF_417,
                Options = new EncodingOptions { Width = 1000, Height = 400 }
            };
            return writer.Write(data);
        }
    }
}