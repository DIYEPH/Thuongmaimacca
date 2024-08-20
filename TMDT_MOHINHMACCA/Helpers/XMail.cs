using System.Net;
using System.Net.Mail;

namespace TMDT_MOHINHMACCA.Helpers
{
    public class XMail
    {
        /// <summary>
        /// Gửi email từ hệ thống thông qua tài khoản gmail
        /// </summary>
        /// <param name="to">Email người nhận</param>
        /// <param name="subject">Tiêu đề mail</param>
        /// <param name="body">Nội dung mail</param>
        public static void Send(String to, String subject, String body)
        {
            var from = "Web Master <nolyeoghada10@gmail.com>";
            Send(from, to, subject, body);
        }

        /// <summary>
        /// Gửi email đơn giản thông qua tài khoản gmail
        /// </summary>
        /// <param name="from">Email người gửi</param>
        /// <param name="to">Email người nhận</param>
        /// <param name="subject">Tiêu đề mail</param>
        /// <param name="body">Nội dung mail</param>
        public static void Send(String from, String to, String subject, String body)
        {
            String cc = "";
            String bcc = "";
            String attachments = "";
            Send(from, to, cc, bcc, subject, body, attachments);
        }

        /// <summary>
        /// Gửi email thông qua tài khoản gmail
        /// </summary>
        /// <param name="from">Email người gửi</param>
        /// <param name="to">Email người nhận</param>
        /// <param name="cc">Danh sách email những người cùng nhận phân cách bởi dấu phẩy</param>
        /// <param name="bcc">Danh sách email những người cùng nhận phân cách bởi dấu phẩy</param>
        /// <param name="subject">Tiêu đề mail</param>
        /// <param name="body">Nội dung mail</param>
        /// <param name="attachments">Danh sách file định kèm phân cách bởi phẩy hoặc chấm phẩy</param>
        public static void Send(String from, String to, String cc, String bcc, String subject, String body, String attachments)
        {
            var message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(from);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.ReplyToList.Add(from);
            if (cc.Length > 0)
            {
                message.CC.Add(cc);
            }
            if (bcc.Length > 0)
            {
                message.Bcc.Add(bcc);
            }
            if (attachments.Length > 0)
            {
                String[] fileNames = attachments.Split(';', ',');
                foreach (var fileName in fileNames)
                {
                    message.Attachments.Add(new Attachment(fileName));
                }
            }

            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true; // Kích hoạt kết nối an toàn
                    client.UseDefaultCredentials = false; // Không sử dụng thông tin xác thực mặc định
                    client.Credentials = new NetworkCredential("nolyeoghada10@gmail.com", "mcub rhcc ggvj mufy");

                    client.Send(message);
                }
            }
            catch (SmtpException ex)
            {
                // Xử lý ngoại lệ SmtpException
                Console.WriteLine($"SmtpException: {ex.Message}");
                // Thêm các xử lý bổ sung tùy thuộc vào nhu cầu của bạn
            }
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác (nếu có)
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
