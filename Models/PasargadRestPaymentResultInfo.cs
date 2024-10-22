


namespace Parbad.Gateway.PasargadRest.Models
{
    public class PasargadRestPaymentResultInfo
    {
        public string invoice { get; set; }
        public string referenceNumber { get; set; }
        public string trackId { get; set; }
        public string maskedCardNumber { get; set; }
        public string hashedCardNumber { get; set; }
        public string requestDate { get; set; }
        public int amount { get; set; }
    }
}
