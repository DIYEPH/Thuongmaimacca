using System.Text;

namespace TMDT_MOHINHMACCA.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile file, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, file.FileName);
                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                    file.CopyTo(myfile);
                return file.FileName;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qazwsxedcrfvtgbyhn ujmikolpQAZWSXEDCRFVTGBYHNUJMIKOLP!";
            var random = new Random();
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
                sb.Append(pattern[random.Next(0, pattern.Length)]);
            return sb.ToString();
        }
    }
}
