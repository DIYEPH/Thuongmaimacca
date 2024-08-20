namespace TMDT_MOHINHMACCA.Services
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string firebase_filename);
    }
}
