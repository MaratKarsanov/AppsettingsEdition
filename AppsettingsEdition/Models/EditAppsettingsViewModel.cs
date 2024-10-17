using System.ComponentModel.DataAnnotations;

namespace AppsettingsEdition.Models
{
    public class EditAppsettingsViewModel
    {
        public string OldValue { get; set; } = "";
        [Required]
        public string NewValue { get; set; } = "";
    }
}
