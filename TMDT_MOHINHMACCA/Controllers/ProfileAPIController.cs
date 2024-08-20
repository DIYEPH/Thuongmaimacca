using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.Services;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileAPIController : Controller
    {
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ShopmaccaContext _db;
        public ProfileAPIController(IFirebaseStorageService firebaseStorageService, ShopmaccaContext db)
        {
            _firebaseStorageService = firebaseStorageService;
            _db = db;
        }

        [HttpPut("updateAvatar")]
        public async Task<IActionResult> UploadAvatarToFirebase([FromForm] UpdateAvatarVM model)
        {
            if (model == null || string.IsNullOrEmpty(model.Username) || model.Avatar == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            var user = _db.Accounts.FirstOrDefault(u => u.Username == model.Username);
            if (user == null)
            {
                return NotFound("Không tìm thấy người dùng");
            }
            try
            {
                // Xử lý tải ảnh lên Firebase và nhận đường dẫn imageUrl
                var imageUrl = await _firebaseStorageService.UploadImageAsync(model.Avatar, "Avatar");
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    user.Avatar = imageUrl;
                    _db.SaveChanges();
                }
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }

        }
    }
}
