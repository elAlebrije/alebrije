using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Alebrije.Storage.AmazonProvider;
using Alebrije.Storage.Contracts;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Alebrije.Storage.DigitalOceanProvider
{
    public class StorageManager : IStorage
    {
        private readonly ILogger<StorageManager> _logger;
        private readonly Options _options;

        public StorageManager(ILogger<StorageManager> logger, Options options)
        {
            _logger = logger;
            _options = options;
        }

        public async Task<string> Upload(IFormFile file, AssetType asset, string subFolder)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = _options.Endpoint,
            };
            try
            {
                var extension = file.FileName.Split('.').Last();
                var credentials = new BasicAWSCredentials(_options.Access, _options.Token);
                using var client = new AmazonS3Client(credentials, config);


                await using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var bucketName = $"{_options.Bucket.ToLower()}/{asset.ToString().ToLower()}/{subFolder}";
                var fileName = $"{Guid.NewGuid()}.{extension.ToLower()}";
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = stream,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6291456, // 6 MB
                    Key = fileName,
                    CannedACL = S3CannedACL.PublicRead
                };
                var fileTransferUtility = new TransferUtility(client);
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                var baseUri = new Uri(_options.PublicUrl);
                var relativePath = $"{asset.ToString().ToLower()}/{subFolder}/{fileName}";
                var returnUrl = new Uri(baseUri, relativePath);

                return returnUrl.ToString();
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                throw new FileLoadException("The file could not be uploaded", file.FileName);
            }
        }
    }
}