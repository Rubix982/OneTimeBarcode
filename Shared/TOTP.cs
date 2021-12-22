using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yort.Otp;

namespace Shared
{
    public class TOTP
    {
        public const int digits = 8; // digits of code
        public const int period = 10; // interval for new TOTP in seconds
        public const int tolerance = 1; // how many cycles either back or ahead to tolerate
        private byte[] secret;
        private TimeBasedPasswordGenerator totp;

        public TOTP(byte[] secret)
        {
            this.totp = new TimeBasedPasswordGenerator(true, secret);
            this.totp.TimeInterval = TimeSpan.FromSeconds(period);
            this.totp.PasswordLength = digits;
            this.secret = secret;
        }

        public string GetCode()
        {
            return totp.GeneratedPassword;
        }

        public bool ConfirmCode(string code)
        {
            var time = DateTime.UtcNow;
            Debug.WriteLine(time);
            if (this.totp.GeneratedPassword.Equals(code))
            {
                return true;
            }
            for (var i = 0; i < tolerance; i++)
            {
                var tmpTotp = new TimeBasedPasswordGenerator(true, this.secret);
                tmpTotp.TimeInterval = TimeSpan.FromSeconds(period);
                tmpTotp.PasswordLength = digits;
                time = time.AddSeconds(-period);
                tmpTotp.Timestamp = time;
                var checkcode = tmpTotp.GeneratedPassword;
                if (checkcode.Equals(code))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
