using System.Net.Http;
using System.Threading.Tasks;
using Tavis.Http;

namespace Tavis.HttpResponseMachine
{
    /// <summary>
    /// HttpResponseHandler that can be chained into a response pipeline
    /// </summary>
    public abstract class DelegatingResponseHandler : IResponseHandler
    {
        public DelegatingResponseHandler InnerResponseHandler { get; set; }

        protected DelegatingResponseHandler()
        {
            
        }

        protected DelegatingResponseHandler(DelegatingResponseHandler innerResponseHandler)
        {
            InnerResponseHandler = innerResponseHandler;
        }

        public virtual Task<HttpResponseMessage> HandleResponseAsync(IRequestFactory requestFactory, HttpResponseMessage responseMessage)
        {
            if (InnerResponseHandler != null)
            {
                return InnerResponseHandler.HandleResponseAsync(requestFactory, responseMessage);
            }

            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(responseMessage);
            return tcs.Task;
        }
    }


 
}