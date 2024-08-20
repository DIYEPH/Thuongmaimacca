using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.Services;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostAPIController : ControllerBase
    {
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ShopmaccaContext _db;
        public PostAPIController(IFirebaseStorageService firebaseStorageService, ShopmaccaContext db)
        {
            _firebaseStorageService = firebaseStorageService;
            _db = db;
        }
        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImageToFirebase([FromForm] IFormFile coverImage)
        {
            try
            {
                // Xử lý tải ảnh lên Firebase và nhận đường dẫn imageUrl
                var imageUrl = await _firebaseStorageService.UploadImageAsync(coverImage, "CoverPost");
                return Ok(imageUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }
        [HttpPost("createPost")]
        public async Task<IActionResult> CreatePost([FromBody] Post model)
        {
            try
            {
                string? username = User.Identity?.Name;
                model.Username = username;
                model.Status = "c";
                model.PostTime = DateTime.Now;
                _db.Posts.Add(model);
                await _db.SaveChangesAsync();
                int postId = await _db.Posts.OrderByDescending(p => p.PostId).Select(p => p.PostId).FirstOrDefaultAsync();
                return Ok(postId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating post: {ex.Message}");
            }
        }

    }
}
