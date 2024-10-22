namespace Parbad.Gateway.PasargadRest
{
    public class PasargadRestGatewayOptions
    {
        public string BaseEndpoint { get; set; } = "https://pep.shaparak.ir/dorsa1";
        public string GetTokenEndpoint { get; set; } = "/token/getToken";
        public string PaymentPurchaseEndpoint { get; set; } = "/api/payment/purchase";
        public string PaymentConfirmTransactionEndPoint { get; set; } = "/api/payment/confirm-transactions";
        public string PaymentCheckTransactionEndPoint { get; set; } = "/api/payment/payment-inquiry";
        public string PaymentVerifyEndpoint { get; set; } = "/api/payment/verify-payment";
        public string PaymentReverseEndpoint { get; set; } = "/api/payment/reverse-transactions";
    }
}
