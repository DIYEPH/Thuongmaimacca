using AutoMapper;
using Firebase.Auth;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using TMDT_MOHINHMACCA.Models;
using TMDT_MOHINHMACCA.Services;
using TMDT_MOHINHMACCA.ViewModels;

namespace TMDT_MOHINHMACCA.Hubs
{
    public class ChatHub :Hub
    {
        ShopmaccaContext _db;
        IMapper _mapper;
        public  static List<UserMessagerVM> _Connections = new List<UserMessagerVM>();
        private readonly static Dictionary<string, string> _ConnectionsMap = new Dictionary<string, string>();
        public ChatHub(ShopmaccaContext db,IMapper mapper)
        {

            _db = db;
            _mapper= mapper;
        }
        private string Username
        {
            get
            {
                return Context.User.Identity.Name;
            }
        }
        public async Task SendPrivate(string receiverId, string content)
        {
            if (string.IsNullOrEmpty(content)) {
                var message = new Message();
                message.SenderId=Username;
                message.Content= Regex.Replace(content, @"<.*?>", string.Empty);
                message.ReceiverId=receiverId;
                message.Senttime= DateTime.Now;
                message.Status = 0;

                //Send
                await Clients.Client(Username).SendAsync("newMessage", message);
                await Clients.Caller.SendAsync("newMessage", message);
            }           
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                var user = _db.Accounts.FirstOrDefault(p => p.Username == Username);
                if (user != null)
                {
                    var userViewModel = _mapper.Map<Account, UserMessagerVM>(user);
                    userViewModel.Device = GetDevice();
                    if (!_Connections.Any(u => u.Username == Username))
                    {
                        _Connections.Add(userViewModel);
                        _ConnectionsMap.Add(Username, Context.ConnectionId);
                        await Clients.All.SendAsync("UserConnected", userViewModel);
                    }
                    await Clients.Caller.SendAsync("getProfileInfo", userViewModel);
                }
            }
            catch(Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var user = _Connections.Where(u => u.Username == Username).First();
                _Connections.Remove(user);
                _ConnectionsMap.Remove(user.Username);
                await Clients.All.SendAsync("UserDisconnected", user);
            }
            catch(Exception ex)
            {
                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public static List<UserMessagerVM> GetConnections()
        {
            return _Connections;
        }
        private string? GetDevice()
        {
            var device= Context.GetHttpContext().Request.Headers["Device"].ToString();
            if (!string.IsNullOrEmpty(device) && (device.Equals("Desktop")|| device.Equals("Mobile")))
            {
                return device;
            }
            return "Web";
        }
    }
}
