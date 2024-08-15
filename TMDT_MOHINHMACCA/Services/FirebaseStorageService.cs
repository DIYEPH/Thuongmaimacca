using Firebase.Auth;
using Firebase.Storage;
using System.Net.Sockets;
using TESTSTORAGE.Helpers;
using TESTSTORAGE.ViewModels;

namespace TMDT_MOHINHMACCA.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly string _apiKey;
        private readonly string _bucketName;
        private readonly string _authEmail;
        private readonly string _authPassword;
        public FirebaseStorageService()
        {
            _apiKey= StorageDataAccess.GetAPIKey();
            _bucketName = StorageDataAccess.GetBucket();
            _authEmail = StorageDataAccess.GetAuthEmail();
            _authPassword = StorageDataAccess.GetAuthPassword();
        }
        public async Task<string> UploadImageAsync(IFormFile file,string firebase_filename)
        {
            string imageUrl = null;

            if (file != null && file.Length > 0)
            {
                // Xác định tên thư mục trên Firebase
                string firebaseFolderName = firebase_filename;

                // Tạo MemoryStream để lưu trữ dữ liệu của file
                using (var ms = new MemoryStream())
                {
                    // Đọc dữ liệu từ file vào MemoryStream
                    await file.CopyToAsync(ms);
                    ms.Position = 0; // Đặt vị trí của MemoryStream về đầu để đọc từ đầu

                    // Tạo FirebaseProvider để xác thực với Firebase
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
                    var auth = await authProvider.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);

                    // Tải hình ảnh lên Firebase và lấy link của hình ảnh
                    imageUrl = await new FirebaseStorage(
                        _bucketName,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                        })
                        .Child(firebaseFolderName)
                        .Child($"{Guid.NewGuid()}_{file.FileName}")
                        .PutAsync(ms);
                }
            }

            return imageUrl ?? ""; // Trả về imageUrl nếu có, ngược lại trả về chuỗi rỗng
        }

    }
}
