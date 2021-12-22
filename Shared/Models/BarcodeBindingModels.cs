using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class requestSharedKeyBindingModel
    {
        [Required]
        [Display(Name = "Key")]
        public string Key { get; set; }
    }
}