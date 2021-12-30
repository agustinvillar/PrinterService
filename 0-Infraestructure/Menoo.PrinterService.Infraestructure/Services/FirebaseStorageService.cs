using Firebase.Auth;
using Firebase.Storage;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Services
{
    public class FirebaseStorageService : IFirebaseStorage
    {
        private readonly string _apiKey;

        private readonly string _user;

        private readonly string _password;

        private readonly string _bucket;

        public FirebaseStorageService() 
        {
            _apiKey = GlobalConfig.ConfigurationManager.GetSetting("keyFirebaseStorage");
            _user = GlobalConfig.ConfigurationManager.GetSetting("userFirebaseStorage");
            _password = ConfigurationManager.AppSettings["passwordFirebaseStorage"];
            _bucket = ConfigurationManager.AppSettings["fireBaseBucket"];
        }

        public async Task<string> UploadAsync(byte[] fileBytes, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
            var tokenData = await auth.SignInWithEmailAndPasswordAsync(_user, _password);
            Stream stream = new MemoryStream(fileBytes);
            var uploadAction = new FirebaseStorage(
                     _bucket,
                     new FirebaseStorageOptions
                     {
                         AuthTokenAsyncFactory = () => Task.FromResult(tokenData.FirebaseToken),
                         ThrowOnCancel = true
                     })
                    .Child("tickets")
                    .Child($"{fileName}.png");
            var downloadUrl = await uploadAction.PutAsync(stream);
            return downloadUrl;
        }
    }
}
