namespace ParbadGateway.PasargadRest.Models
{

    internal class PasargadRestPaymentRequest
    {
        public string invoice { get; set; }
        public string invoiceDate { get; set; }
        public long amount { get; set; }
        public string callbackApi { get; set; }
        public string mobileNumber { get; set; }
        public string serviceCode { get; set; }
        public string serviceType { get; set; }
        public int terminalNumber { get; set; }
        public string description { get; set; }
        public string payerMail { get; set; }
        public string payerName { get; set; }
        public string pans { get; set; }
        public string nationalCode { get; set; }
    }

}
