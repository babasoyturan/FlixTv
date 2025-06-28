using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FlixTv.Common;
using FlixTv.Common.Infrastructure;
using FlixTv.Common.Models.DTOs;

namespace FlixTv.Projections.UploadVideoService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IAmazonS3 _s3Client;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _s3Client = new AmazonS3Client(FlixTvConstants.AccessKey, FlixTvConstants.SecretKey, RegionEndpoint.USEast1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(FlixTvConstants.MovieExchangeName)
                .EnsureQueue(FlixTvConstants.UploadVideoQueueName, FlixTvConstants.MovieExchangeName)
                .Receive<FormFileDto>(async (message) =>
                {
                    await UploadToS3(message.Key, message.FileName, message.ContentType, message.FileData);
                })
                .StartConsuming(FlixTvConstants.UploadVideoQueueName);
        }

        private async Task UploadToS3(string key, string fileName, string contentType, byte[] data)
        {
            var uploadPath = $"{key}{Path.GetExtension(fileName)}";
            using var stream = new MemoryStream(data);
            var uploadRequest = new PutObjectRequest
            {
                InputStream = stream,
                Key = uploadPath,
                BucketName = FlixTvConstants.InputBucket,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(uploadRequest);
        }
    }
}