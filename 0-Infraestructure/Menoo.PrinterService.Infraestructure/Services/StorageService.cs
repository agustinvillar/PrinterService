using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Menoo.PrinterService.Infraestructure.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Menoo.PrinterService.Infraestructure.Services
{
    public class StorageService : IStorage
    {
        private readonly string _awsAccessKeyId;

        private readonly string _awsSecretKey;

        private readonly string _awsBucket;

        public StorageService()
        {
            _awsAccessKeyId = GlobalConfig.ConfigurationManager.GetSetting("awsAccessKeyId");
            _awsSecretKey = GlobalConfig.ConfigurationManager.GetSetting("awsSecretKey");
            _awsBucket = GlobalConfig.ConfigurationManager.GetSetting("awsBucket");
        }

        public async Task<string> UploadAsync(byte[] fileBytes)
        {
            string fileName = $"ticket_{Guid.NewGuid().ToString()}";
            using (var s3 = new AmazonS3Client(_awsAccessKeyId, _awsSecretKey, RegionEndpoint.USEast1))
            {
                var stream = new MemoryStream(fileBytes);
                var utility = new TransferUtility(s3);
                var request = new TransferUtilityUploadRequest
                {
                    BucketName = _awsBucket,
                    Key = fileName,
                    InputStream = stream
                };
                utility.Upload(request);
            }
            return fileName;
        }
    }
}
