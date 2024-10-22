


namespace Parbad.Gateway.PasargadRest.Models
{
    public class PasargadRestTokenResponse
    {
        public string resultMsg { get; set; }
        public int resultCode { get; set; }
        public string token { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string userId { get; set; }
        public PasargadRestRole[] roles { get; set; }
    }


}
