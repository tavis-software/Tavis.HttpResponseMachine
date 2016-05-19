using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tavis.Http;
using Tavis.HttpResponseMachine;

namespace LinkTests
{

    public class FakeHandler : DelegatingHandler
    {
        public HttpResponseMessage Response { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            if (Response == null)
            {
                return base.SendAsync(request, cancellationToken);
            }
            Response.RequestMessage = request;
            return Task.Factory.StartNew(() => Response);
        }
    }

    public class NotFoundHandler : DelegatingResponseHandler
    {
        public bool NotFound = false;
        public NotFoundHandler(DelegatingResponseHandler innerHandler) : base(innerHandler)
        {
            
        }

        public override Task<HttpResponseMessage> HandleResponseAsync(IRequestFactory requestFactory, HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine("Not found");
                NotFound = true;
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(responseMessage);
                return tcs.Task;
            }
            else
            {
                return base.HandleResponseAsync(requestFactory, responseMessage);
            }
        }
    }

    public class OkHandler : DelegatingResponseHandler
    {
        public OkHandler(DelegatingResponseHandler innerHandler) : base(innerHandler)
        {

        }

        public override Task<HttpResponseMessage> HandleResponseAsync(IRequestFactory requestFactory, HttpResponseMessage responseMessage)
        {
            if (responseMessage.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine("OK!");
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(responseMessage);
                return tcs.Task;
            }
            else
            {
                return base.HandleResponseAsync(requestFactory, responseMessage);
            }
        }
    }
}