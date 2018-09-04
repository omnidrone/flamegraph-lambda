using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Newtonsoft.Json;

namespace FlamegraphLambda
{
    public class FunctionHelper
    {
        public static async Task Trigger(List<string> dnsNames, int index, string function)
        {
            IAmazonLambda lambda = new AmazonLambdaClient(new AmazonLambdaConfig
            {
                RegionEndpoint = RegionEndpoint.USEast1,
                Timeout = TimeSpan.FromMinutes(8)
            });
            await lambda.InvokeAsync(new InvokeRequest
            {
                FunctionName = function,
                InvocationType = InvocationType.Event,
                Payload = JsonConvert.SerializeObject(new ActionRequest(dnsNames, index))
            });
        }
    }
}
