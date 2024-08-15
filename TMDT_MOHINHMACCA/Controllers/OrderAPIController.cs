using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using TMDT_MOHINHMACCA.Helpers;
using TMDT_MOHINHMACCA.Models;

namespace TMDT_MOHINHMACCA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderAPIController : Controller
    {
        private readonly ShopmaccaContext _db;
        public OrderAPIController(ShopmaccaContext db)
        {
            _db = db;
        }

        [HttpPost("Order")]
        public async Task<IActionResult> CreateOrder([FromBody] Order model)
        {
            try
            {
                string? username = User.Identity?.Name;
                model.Buyer = username;
                model.Status = "0";
                model.OrderTime = DateTime.Now;
                _db.Orders.Add(model);
                var post = _db.Posts.Include(p => p.UsernameNavigation).Where(p => p.PostId == model.PostId).FirstOrDefault();
                await _db.SaveChangesAsync();


                await Task.Run(() =>
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    var salesUrl = $"{baseUrl}/mysales";
                    var receive = _db.Accounts.Find(post.Username);
                    XMail.Send(receive.Email, "Bạn có đơn hàng mới!", $"Người đặt hàng {model.Buyer}. <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                });

                return Ok("Order created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating post: {ex.Message}");
            }
        }
        [HttpPost("checkoutAccept")]
        public async Task<IActionResult> checkoutAccept(int orderId)
        {
            try
            {
                var order = _db.Orders.Find(orderId);
                string username = User.Identity?.Name;
                var account = _db.Accounts.Find(username);

                if (account != null)
                {
                    if (order.Status == "0")
                        if (account.Money >= 2000)
                        {
                            var orderdetail = new OrderDetail();
                            orderdetail.OrderId = orderId;
                            orderdetail.Stamptime = DateTime.Now;
                            orderdetail.Person = username;
                            var history = new Transactionhistory();
                            history.Username = account.Username;
                            history.Amountmoney = 2000;
                            history.Initialbalance = account.Money;
                            history.Finalbalance = account.Money - history.Amountmoney;
                            DateTime utcNow = DateTime.UtcNow;
                            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, vietnamTimeZone);
                            history.TransactionDate = vietnamTime;
                            history.Content = "Thanh toán chấp nhận đơn hàng, Mã đơn hàng: " + orderId;
                            history.TransactionType = "3";
                            history.Payments = "0";
                            account.Money -= 2000;
                            _db.Transactionhistories.Add(history);
                            _db.SaveChanges();
                            await Task.Run(() =>
                            {
                                var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                                var salesUrl = $"{baseUrl}/mysales";
                                var receive = _db.Accounts.Find(username);
                                XMail.Send(receive.Email, $"Bạn đã chấp nhận đơn hàng!", $"Bạn đã thanh toán thành công 2000VNĐ để chấp nhận đơn hàng  {orderId}. <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                            });
                            return Ok(new { message = "Đơn hàng đã được tạo thành công" });
                        }
                        else
                        {
                            return BadRequest(new { insufficientBalance = true });
                        }
                    else
                        return BadRequest();
                }
                return NotFound("Không tìm thấy tài khoản.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating post: {ex.Message}");
            }
        }

        [HttpPut("cancelOrder")]
        public async Task<IActionResult> CancelOrderStatus(int orderId)
        {
            try
            {
                string username = User.Identity?.Name;
                var orderdetail = new OrderDetail();
                orderdetail.OrderId = orderId;
                orderdetail.Stamptime = DateTime.Now;
                orderdetail.Person = username;
                orderdetail.RequiType = "5";
                _db.Add(orderdetail);
                await _db.SaveChangesAsync();
                var order = _db.Orders
                    .Include(p => p.Post)
                    .FirstOrDefault(p => (p.Post.Username == username || p.Buyer == username) && p.Status == "0" && p.OrderId == orderId);
                if (order == null)
                {
                    return NotFound("Order not found or cannot be cancelled.");
                }

                order.Status = "5";
                await Task.Run(() =>
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    var salesUrl = $"{baseUrl}/mysales";
                    var receive = _db.Accounts.Find(order.Post.Username);
                    XMail.Send(receive.Email, "Đơn hàng đã bị hủy !", $"Người thực hiện {username}. <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                });
                await Task.Run(() =>
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    var salesUrl = $"{baseUrl}/myorders";
                    var receive = _db.Accounts.Find(order.Buyer);
                    XMail.Send(receive.Email, "Đơn hàng ...... đã bị hủy !", $"Người thực hiện {username}. <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                });
                await _db.SaveChangesAsync();

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error cancelling order: {ex.Message}");
            }
        }

        [HttpPut("acceptOrder")]
        public async Task<IActionResult> AcceptOrderStatus(int orderId)
        {
            try
            {
                string username = User.Identity?.Name;

                var order = _db.Orders
                    .Include(p => p.Post)
                    .FirstOrDefault(p => p.Post.Username == username && p.Status == "0" && p.OrderId == orderId);
                if (order == null)
                {
                    return NotFound("Order not found or cannot be cancelled.");
                }
                if (order.Status == "0")
                {
                    var orderdetail = new OrderDetail();
                    orderdetail.OrderId = orderId;
                    orderdetail.Stamptime = DateTime.Now;
                    orderdetail.Person = username;
                    orderdetail.RequiType = "1";
                    _db.Add(orderdetail);
                    order.Status = "1";
                    await _db.SaveChangesAsync();
                    await Task.Run(() =>
                    {
                        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                        var salesUrl = $"{baseUrl}/myorder";
                        var receive = _db.Accounts.Find(order.Buyer);
                        XMail.Send(receive.Email, $"Đơn hàng đã được chấp nhận và thực hiện!", $"Đơn hàng {orderId} đã được người bán chấp nhận và thực hiện . <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                    });
                }
                else
                    return BadRequest();
                return Ok(new { message = "Order accepted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error cancelling order: {ex.Message}");
            }
        }
        [HttpPut("finishedOrder")]
        public async Task<IActionResult> FinishedOrderStatus(int orderId)
        {
            try
            {
                string username = User.Identity?.Name;
                var orderdetail = new OrderDetail();
                orderdetail.OrderId = orderId;
                orderdetail.Stamptime = DateTime.Now;
                orderdetail.Person = username;
                orderdetail.RequiType = "2";
                _db.Add(orderdetail);
                var order = _db.Orders
                    .Include(p => p.Post)
                    .FirstOrDefault(p => p.Post.Username == username && p.Status == "1" && p.OrderId == orderId);

                if (order == null)
                {
                    return NotFound("Order not found or cannot be cancelled.");
                }
                order.Status = "2";
                await _db.SaveChangesAsync();
                await Task.Run(() =>
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    var salesUrl = $"{baseUrl}/myorder";
                    var receive = _db.Accounts.Find(order.Buyer);
                    XMail.Send(receive.Email, $"Đơn hàng đã thực hiện xong!", $"Đơn hàng {orderId} đã được người bán thực hiện xong, vui lòng liên hệ, thanh toán.... . <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                });
                return Ok(new { message = "Order accepted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error cancelling order: {ex.Message}");
            }
        }
        [HttpPut("completeOrder")]
        public async Task<IActionResult> CompleteOrderStatus([FromBody] Order request)
        {
            try
            {
                string username = User.Identity?.Name;
                var orderdetail = new OrderDetail();
                orderdetail.OrderId = request.OrderId;
                orderdetail.Stamptime = DateTime.Now;
                orderdetail.Person = username;
                orderdetail.RequiType = "3";
                _db.Add(orderdetail);
                var order = _db.Orders
                    .Include(p => p.Post)
                    .FirstOrDefault(p => p.Post.Username == username && p.Status == "2" && p.OrderId == request.OrderId);

                if (order == null)
                {
                    return NotFound("Order not found or cannot be cancelled.");
                }

                order.Status = "3";
                order.Link = request.Link;
                await _db.SaveChangesAsync();
                await Task.Run(() =>
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    var salesUrl = $"{baseUrl}/myorders";
                    var receive = _db.Accounts.Find(order.Buyer);
                    XMail.Send(receive.Email, $"Đơn hàng đã hoàn thành!", $"Chúc mừng, đơn hàng {order.OrderId} đã được hoàn thành, vui lòng đánh giá và phản hồi . <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                });
                return Ok(new { message = "Order accepted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error cancelling order: {ex.Message}");
            }
        }
        [HttpPut("reviewOrder")]
        public async Task<IActionResult> ReviewOrderStatus([FromBody] Order request)
        {
            try
            {
                string username = User.Identity?.Name;
                var orderdetail = new OrderDetail();
                orderdetail.OrderId = request.OrderId;
                orderdetail.Stamptime = DateTime.Now;
                orderdetail.Person = username;
                orderdetail.RequiType = "4";
                _db.Add(orderdetail);
                var order = _db.Orders
                    .Include(p => p.Post)
                    .FirstOrDefault(p => p.Buyer == username && p.Status == "3" && p.OrderId == request.OrderId);

                if (order == null)
                {
                    return NotFound("Order not found or cannot be cancelled.");
                }
                order.Star = request.Star;
                order.Review = request.Review;
                await _db.SaveChangesAsync();
                await Task.Run(() =>
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
                    var salesUrl = $"{baseUrl}/postdetail?id=" + order.PostId;
                    var receive = _db.Accounts.Find(order.Buyer);
                    XMail.Send(receive.Email, $"Bạn có đánh giá mới!", $"Người mua đã đánh giá bài đăng của bạn, vui lòng truy cập để xem chi tiết. <a href='{salesUrl}'>Nhấn vào đây</a> để xem chi tiết.");
                });

                return Ok(new { message = "Order reviewed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error cancelling order: {ex.Message}");
            }
        }
    }
}
