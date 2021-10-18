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

        public async Task<string> Upload(IFormFile file, AssetType asset)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = _options.Url
            };
            try
            {
                var extension = file.FileName.Split('.').Last();
                using var client = new AmazonS3Client(new BasicAWSCredentials(_options.Access, _options.Token), config);
                await using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _options.Bucket + @"/" + asset,
                    InputStream = stream,
                    StorageClass = S3StorageClass.StandardInfrequentAccess,
                    PartSize = 6291456, // 6 MB
                    Key = $"{Guid.NewGuid()}.{extension}",
                    CannedACL = S3CannedACL.PublicRead
                };
                var fileTransferUtility = new TransferUtility(client);
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                return $"{fileTransferUtilityRequest.BucketName}/{fileTransferUtilityRequest.Key}";
            }
            catch (AmazonS3Exception e)
            {
                _logger.LogError("Error encountered ***. Message:'{0}' when writing an object", e.Message);
                throw new FileLoadException("The file could not be uploaded", file.FileName);
            }
        }
    }
}