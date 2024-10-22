namespace Parbad.Gateway.PasargadRest.Models
{

    internal class PasargadRestCallbackResult
    {
        public bool IsSucceed { get; set; }

        /// <summary>
        /// Equals to ReferenceID in Parbad system.
        /// </summary>
        public string InvoiceNumber { get; set; }


        public string UrlId { get; set; }

        public PasargadRestPurchaseResultData Data { get; set; }


        public string Message { get; set; }
    }
}
