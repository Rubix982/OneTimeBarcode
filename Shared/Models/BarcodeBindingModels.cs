using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class RequestSharedKeyBindingModel
    {
        [Required]
        [Display(Name = "Key")]
        public string Key { get; set; }
    }
}