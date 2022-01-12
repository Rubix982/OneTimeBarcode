using System.Collections.Generic;

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
