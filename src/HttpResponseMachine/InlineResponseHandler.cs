using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tavis.Http;

namespace Tavis.HttpResponseMachine
{
    public class InlineResponseHandler : DelegatingResponseHandler, IResponseHandler
    {
        private readonly Action<IRequestFactory, HttpResponseMessage> _action;

        public InlineResponseHandler(Action<IRequestFactory, HttpResponseMessage> action, DelegatingResponseHandler innerHandler = null)
        {
            InnerResponseHandler = innerHandler;
            _action = action;
        }

        public override Task<HttpResponseMessage> HandleResponseAsync(IRequestFactory requestFactory, HttpResponseMessage responseMessage)
        {
            _action(requestFactory, responseMessage);

            return base.HandleResponseAsync(requestFactory, responseMessage);
            
        }
    }
}