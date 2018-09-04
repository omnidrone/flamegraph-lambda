using Amazon.Lambda.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace FlamegraphLambda
{
    public class ActionLambda
    {
        
        public async Task Action(ActionRequest request, ILambdaContext context)
        {
            string path = System.Environment.GetEnvironmentVariable("path");
            string s3url = System.Environment.GetEnvironmentVariable("s3url");

            using (HttpClient client = new HttpClient())
            {
                string url = $"{request.URLs[request.Index]}/{path}?s3url={s3url}";
                context.Logger.LogLine(url);
                var response = await client.GetAsync(url);
                context.Logger.LogLine(response.ToString());
            }
            request.Index++;
            if (request.URLs.Count > request.Index)
                await FunctionHelper.Trigger(request.URLs, request.Index, context.InvokedFunctionArn);
        }

    }

    public class ActionRequest
    {
        public List<string> URLs { get; set; }
        public int Index { get; set; }

        public ActionRequest(List<string> urls, int index)
        {
            URLs = urls;
            Index = index;
        }
    }
}
