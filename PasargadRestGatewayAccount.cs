using Parbad.Abstraction;
namespace Parbad.Gateway.PasargadRest
{
    public class PasargadRestGatewayAccount : GatewayAccount
    {
        public string MerchantCode { get; set; }

        public int TerminalCode { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }


    }
}
