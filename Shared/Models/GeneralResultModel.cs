using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace Shared.Models
{
    public class GeneralResultModel
    {
        public string Message { get; set; }
        public IDictionary<string, IEnumerable<string>> ModelState { get; set; }

        public bool Error
        {
            get { return !string.IsNullOrEmpty(Message); }
        }
    }
}
