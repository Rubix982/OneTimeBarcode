using System;
using Yort.Otp;
using System.Diagnostics;

namespace Shared
{
    public class TOTP
    {
        public const int digits = 8; // digits of code
        public const int period = 10; // interval for new TOTP in seconds
        public const int tolerance = 1; // how many cycles either back or ahead to tolerate
        private readonly byte[] secret;
        private readonly TimeBasedPasswordGenerator totp;

        public TOTP(byte[] secret)
        {
            this.totp = new TimeBasedPasswordGenerator(true, secret)
            {
                TimeInterval = TimeSpan.FromSeconds(period),
                PasswordLength = digits
            };
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
                var tmpTotp = new TimeBasedPasswordGenerator(true, this.secret)
                {
                    TimeInterval = TimeSpan.FromSeconds(period),
                    PasswordLength = digits
                };
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
