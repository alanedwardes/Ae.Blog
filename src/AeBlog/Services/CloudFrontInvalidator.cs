using AeBlog.Models;
using Amazon.CloudFront;
using Amazon.CloudFront.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AeBlog.Services
{
    public sealed class CloudFrontInvalidator : ICloudFrontInvalidator
    {
        private readonly string distributionId;
        private readonly IAmazonCloudFront cloudFront;

        public CloudFrontInvalidator(string distributionId, IAmazonCloudFront cloudFront)
        {
            this.distributionId = distributionId;
            this.cloudFront = cloudFront;
        }

        public async Task InvalidatePost(PostSummary postSummary)
        {
            await cloudFront.CreateInvalidationAsync(new CreateInvalidationRequest
            {
                DistributionId = distributionId,
                InvalidationBatch = new InvalidationBatch
                {
                    CallerReference = Guid.NewGuid().ToString(),
                    Paths = new Paths
                    {
                        Items = new List<string> { postSummary.Url },
                        Quantity = 1
                    }
                }
            });
        }
    }
}
