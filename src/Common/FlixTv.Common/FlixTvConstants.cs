using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlixTv.Common
{
    public class FlixTvConstants
    {
        // RabbitMQ constants
        public const string RabbitMqHost = "localhost";
        public const string DefaultExchangeType = "direct";
        public const string MovieExchangeName = "movieExchange";
        public const string UploadImageQueueName = "movieImageQueue";
        public const string UploadVideoQueueName = "movieSourceVideoQueue";
        public const string SendEmailQueueName = "movieEmailMessageQueue";


        // AWS VOD constants
        public const string Region = "us-east-1";
        public const string CdnName = "dlg82512ftdc3";
        public const string AccessKey = "AKIAYMMUDGSVJRK6HU7B";
        public const string SecretKey = "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";
        public const string InputBucket = "flix-vod-solution-source71e471f1-ite6qfaomflb";
        public const string OutputBucket = "flix-vod-solution-destination920a3c57-b6uptcsoegsj";
    }
}
