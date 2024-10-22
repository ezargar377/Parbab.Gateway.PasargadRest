using Parbad.Internal;

namespace Parbad.Gateway.PasargadRest.Models
{
    internal class PasargadRestCheckCallbackResult
    {
        public bool IsSucceed { get; set; }
        public string Message { get; set; }


        public static PasargadRestCheckCallbackResult Succeed(string message = null)
        {
            return new PasargadRestCheckCallbackResult
            {
                IsSucceed = true,
                Message = (message ?? string.Empty)
            };
        }

        public static PasargadRestCheckCallbackResult Failed(string message)
        {
            return new PasargadRestCheckCallbackResult
            {
                IsSucceed = false,
                Message = (message ?? string.Empty)
            };
        }


    }
}
