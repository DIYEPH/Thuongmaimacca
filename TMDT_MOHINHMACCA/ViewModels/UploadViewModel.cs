using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Chat.Web.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
