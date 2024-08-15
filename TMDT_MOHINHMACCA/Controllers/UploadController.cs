using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Chat.Web.Helpers;
using Chat.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol.Plugins;
using TMDT_MOHINHMACCA.Hubs;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;
using Message = TMDT_MOHINHMACCA.Models.Message;

namespace Chat.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly int FileSizeLimit;
        private readonly string[] AllowedExtensions;
        private readonly ShopmaccaContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileValidator _fileValidator;

        public UploadController(ShopmaccaContext context,
            IMapper mapper,
            IWebHostEnvironment environment,
            IHubContext<ChatHub> hubContext,
            IConfiguration configruation,
            IFileValidator fileValidator)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
            _hubContext = hubContext;
            _fileValidator = fileValidator;

            FileSizeLimit = configruation.GetSection("FileUpload").GetValue<int>("FileSizeLimit");
            AllowedExtensions = configruation.GetSection("FileUpload").GetValue<string>("AllowedExtensions").Split(",");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload([FromForm] UploadViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!_fileValidator.IsValid(viewModel.File))
                    return BadRequest("Validation failed!");

                var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(viewModel.File.FileName);
                var folderPath = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.File.CopyToAsync(fileStream);
                }

                var user = _context.Accounts.Where(u => u.Username == User.Identity.Name).FirstOrDefault();
                var room = _context.Accounts.Where(r => r.Username == viewModel.UserId).FirstOrDefault();
                if (user == null || room == null)
                    return NotFound();

                string htmlImage = string.Format(
                    "<a href=\"/uploads/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/{0}\" class=\"post-image\">" +
                    "</a>", fileName);

                var message = new MessageVM()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    Senttime = DateTime.Now,
                    SenderId = user.Username,
                    ReceiverId = room.Username,
                    Avatarsender = user.Avatar,
                    Status = 0
                };
                var mess = new Message()
                {
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Status = message.Status,
                    Content = message.Content,
                    Senttime = DateTime.Now

                };
                await _context.Messages.AddAsync(mess);
                await _context.SaveChangesAsync();
                var receiverIds = new List<string>
                 {
            user.Username,
            room.Username
                };
                await _hubContext.Clients.Users(receiverIds).SendAsync("newMessage", message);

                return Ok();
            }

            return BadRequest();
        }
    }
}
