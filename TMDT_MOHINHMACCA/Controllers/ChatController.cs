using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TMDT_MOHINHMACCA.Hubs;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        [Route("/chat")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
