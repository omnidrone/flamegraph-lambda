using Amazon.Lambda.Core;
using System;
using Amazon.EC2;
using Amazon.EC2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace FlamegraphLambda
{
    public class StartPerfCheck
    {
        public async Task<string> Start(ILambdaContext context)
        {
            List<string> dnsNames = await GetPicassoApiPublicDns();
            if (dnsNames != null && dnsNames.Count > 0)
            {
                await ChainedRequestHelper.Trigger(dnsNames, 0, context);
            }

            return $"Starting performance checks on {String.Join(",",dnsNames)}";
        }

        public async Task<List<string>> GetPicassoApiPublicDns()
        {
            AmazonEC2Client client = new AmazonEC2Client();
            var response = await client.DescribeInstancesAsync(new DescribeInstancesRequest
            {
                Filters = new List<Filter>{
                    new Filter{
                        Name = "tag:GameRole",
                        Values = new List<string>{"PICASSOAPI"}
                    },
                    new Filter{
                        Name = "tag:Name",
                        Values = new List<string>{"Prod Picasso API"}
                    },
                    new Filter{
                        Name = "instance-state-name",
                        Values = new List<string>{"running"}
                    }
                }
            });

            return response.ResponseMetadata.Metadata.Where(kp => kp.Key == "dnsName").Select(kp => kp.Value).ToList();
        }

    }

}
