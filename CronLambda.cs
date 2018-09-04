using Amazon.Lambda.Core;
using System;
using Amazon.EC2;
using Amazon.EC2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace FlamegraphLambda
{
    public class CronLambda
    {
        public async Task<string> Cron(ILambdaContext context)
        {
            string invokeFunction = context.InvokedFunctionArn.Replace("cron", "action");
            context.Logger.LogLine($"function to invoke {invokeFunction}");
            List<string> dnsNames = await GetPicassoApiPublicDns(context);
            context.Logger.LogLine(String.Join(",", dnsNames));
            if (dnsNames != null && dnsNames.Count > 0)
            {
                await FunctionHelper.Trigger(dnsNames, 0, invokeFunction);
            }

            return $"Starting performance checks on {String.Join(",",dnsNames)}";
        }

        public async Task<List<string>> GetPicassoApiPublicDns(ILambdaContext context)
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
            return response.Reservations.SelectMany(x => x.Instances).Select(i => i.PublicDnsName).ToList();
        }

    }

}
