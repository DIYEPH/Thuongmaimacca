using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TMDT_MOHINHMACCA.Helpers;
using TMDT_MOHINHMACCA.Hubs;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageAPIController : Controller
    {
        private readonly ShopmaccaContext _db;
        private readonly IMapper _mapper;
        private readonly IHubContext<ChatHub> _hubContext;
        public MessageAPIController(IHubContext<ChatHub> hubContext, ShopmaccaContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _hubContext = hubContext;
        }
        private string Username
        {
            get
            {
                return User.Identity.Name;
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var message = await _db.Messages.Include(p => p.Sender).FirstOrDefaultAsync(m => m.MessageId == id);
            if (message == null)
                return NotFound();
            var messageDto = new MessageVM
            {
                MessageId = message.MessageId,
                Content = message.Content,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Senttime = message.Senttime,
                Status = message.Status,
                Avatarsender = message.Sender?.Avatar
            };
            return Ok(messageDto);
        }
        [HttpGet("getUsers/{fullname}")]
        public async Task<IActionResult> GetUsers(string fullname)
        {
            var users = await _db.Accounts.Where(p => EF.Functions.Like(p.Fullname, $"%{fullname}%")).ToListAsync();
            var userDtos = users.Select(user => new UserMessagerVM
            {
                Username = user.Username,
                Fullname = user.Fullname,
                Avatar = user.Avatar,
            }).ToList();

            return Ok(userDtos);
        }
        [HttpGet("chatUser/{userid}")]
        public async Task<IActionResult> GetMessages(string userid)
        {
            await _db.Messages
    .Where(m =>
        (m.ReceiverId == Username && m.SenderId == userid))
    .Where(m => m.Status != 1) // Chỉ cập nhật các tin nhắn chưa được đánh dấu là đã đọc
    .ForEachAsync(m => m.Status = 1);
            await _db.SaveChangesAsync();
            List<MessageVM> messagesDto = await _db.Messages
                               .Where(m =>
                                   (m.ReceiverId == userid && m.SenderId == Username) ||
                                   (m.ReceiverId == Username && m.SenderId == userid))
                               .Include(m => m.Sender)
                               .OrderBy(m => m.Senttime)

                               .Select(message => new MessageVM
                               {
                                   MessageId = message.MessageId,
                                   Content = message.Content,
                                   SenderId = message.SenderId,
                                   ReceiverId = message.ReceiverId,
                                   Senttime = message.Senttime,
                                   Status = message.Status,
                                   Avatarsender = message.Sender.Avatar
                               })
                               .ToListAsync();

            return Ok(messagesDto);
        }
        [HttpGet("chatRoom")]
        public IActionResult GetChatedUser()
        {
            var allUsernames = _db.Messages
       .Where(m => m.SenderId == Username || m.ReceiverId == Username)
       .Select(m => m.SenderId == Username ? m.ReceiverId : m.SenderId)
       .Distinct()
       .ToList();
            var usersWithMessages = _db.Accounts
                .Where(u => allUsernames.Contains(u.Username))
                .ToList();
            var messages = _db.Messages
                .Where(m => (m.SenderId == Username && allUsernames.Contains(m.ReceiverId)) ||
                            (m.ReceiverId == Username && allUsernames.Contains(m.SenderId)))
                .GroupBy(m => m.SenderId == Username ? m.ReceiverId : m.SenderId)
                .Select(g => new
                {
                    Username = g.Key,
                    LastMessage = g.OrderByDescending(m => m.Senttime).FirstOrDefault(),
                    UnreadCount = g.Count(m => m.ReceiverId == Username && m.Status == 0)
                })
                .ToList();
            var sortedUsers = usersWithMessages
                .Join(messages,
                      user => user.Username,
                      message => message.Username,
                      (user, message) => new UserMessagerVM
                      {
                          Avatar = user.Avatar,
                          Username = user.Username,
                          Fullname = user.Fullname,
                          Lastmess = message.LastMessage?.Content,
                          Lastdate = message.LastMessage?.Senttime,
                          Unread = message.UnreadCount,
                          Online = ChatHub._Connections.FirstOrDefault(c => c.Username == user.Username) != null ? true : false,
                          Device = ChatHub._Connections.FirstOrDefault(c => c.Username == user.Username)?.Device
                      })
                .OrderByDescending(userWithMessage => userWithMessage.Lastdate ?? DateTime.MinValue)
                .ToList();

            return Ok(sortedUsers);
        }
        [HttpGet("getUnread")]
        public IActionResult GetUnread()
        {

            var unreadUsers = _db.Messages
        .Where(m => m.ReceiverId == Username && m.Status == 0)
        .GroupBy(m => m.SenderId)  // Nhóm các tin nhắn theo người gửi
        .Select(g => g.Key)         // Chọn ra người gửi (SenderId)
        .Count();                   // Đếm số lượng người gửi

            return Ok(unreadUsers);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<MessageVM>> Create(MessageVM message)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            message.SenderId = Username;
            message.Status = 0;
            message.Senttime = vietnamTime;


            var messnew = new Message();
            messnew.SenderId = Username;
            messnew.Status = 0;
            messnew.Senttime = vietnamTime;
            messnew.Content = message.Content;
            messnew.ReceiverId = message.ReceiverId;
            _db.Messages.Add(messnew);
            await _db.SaveChangesAsync();
            var receiverIds = new List<string>
        {
            Username,
            message.ReceiverId
        };
            //Send mess
            await _hubContext.Clients.Users(receiverIds).SendAsync("newMessage", message);
            var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            var chatUrl = $"{baseUrl}/chat";

            await Task.Run(() =>
            {
                var receive = _db.Accounts.Find(message.ReceiverId);
                XMail.Send(receive.Email, "Bạn có tin nhắn mới!", $"Có người nhắn tin cho bạn. <a href='{chatUrl}'>Nhấn vào đây</a> để xem chi tiết.");
            });
            return CreatedAtAction(nameof(Get), new { id = message.MessageId }, message);
        }
    }
}
