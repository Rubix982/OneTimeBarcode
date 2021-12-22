using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class LoginResultModel
    {
        public string error { get; set; }
        public string error_description { get; set; }
        public string access_token { get; set; }
        public TokenType token_type { get; set; } 
        public long expires_in { get; set; }
    }

    public enum TokenType
    {
        bearer
    }
}
