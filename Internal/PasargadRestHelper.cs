using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Parbad.Abstraction;
using ParbadGateway.PasargadRest.Models;
using Parbad.Http;
using Parbad.Internal;
using Parbad.Options;
using Parbad.Storage.Abstractions.Models;
using Parbad.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace ParbadGateway.PasargadRest.Internal
{
    internal static class PasargadRestHelper
    {
        internal const string CellNumber = "PasargadCellNumber";

        public static async Task<PaymentRequestResult> CreateRequestResult(
            Invoice invoice,
            HttpContext httpContext,
            IPasargadIdentityService identityService,
            PasargadRestGatewayAccount account,
            PasargadRestGatewayOptions gatewayOptions , HttpClient httpClient)
        {
            var invoiceDate = GetTimeStamp(DateTime.Now);

            var timeStamp = invoiceDate;

            var request = new PasargadRestPaymentRequest()
            {
                amount = (long)invoice.Amount,
                invoice = invoice.TrackingNumber.ToString(),
                invoiceDate = invoiceDate,
                terminalNumber = account.TerminalCode,
                callbackApi = invoice.CallbackUrl ,
                serviceType = "PURCHASE",
                serviceCode = "8",
                mobileNumber = invoice.CellNumber()?.ToString()
            };

            httpClient.DefaultRequestHeaders.Add("Authorization", await identityService.GetAccessTokenAsync(account));
            httpClient.BaseAddress = new Uri(gatewayOptions.BaseEndpoint);
            var resp = await httpClient.PostAsync(gatewayOptions.PaymentPurchaseEndpoint, new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                var resultString = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode) {

                var resultModel = JsonConvert.DeserializeObject<PasargadRestResponse<PasargadRestPaymentData>>(resultString);

                if (resultModel.resultCode != 0) {
                    return PaymentRequestResult.Failed(PasargadErrorHandler.GetErrorMessage(resultModel.resultCode), account.Name, resp.StatusCode.ToString());
                }
                httpContext.Response.Cookies.Append("urlId", resultModel.data.urlId);
                var result = PaymentRequestResult.SucceedWithRedirect(account.Name, httpContext, resultModel.data.url);
                result.DatabaseAdditionalData.Add("timeStamp", timeStamp);
                result.DatabaseAdditionalData.Add("urlId", resultModel.data.urlId);
                return result;
            }
            else
            {
                return PaymentRequestResult.Failed(resp.StatusCode.ToString(), account.Name, resp.StatusCode.ToString());
            }
        }

        public static async Task<PasargadRestCallbackResult> CreateCallbackResult(HttpRequest httpRequest,MessagesOptions messagesOptions,CancellationToken cancellationToken)
        {
            //  Reference ID
            var invoiceNumber = await httpRequest.TryGetParamAsync("invoiceId", cancellationToken).ConfigureAwaitFalse();
            var status = await httpRequest.TryGetParamAsync("status", cancellationToken).ConfigureAwaitFalse();
            httpRequest.Cookies.TryGetValue("urlId" , out var ulrId);

            var isSucceed = true;
            string message = null;

            if (string.IsNullOrWhiteSpace(invoiceNumber.Value) ||
                string.IsNullOrWhiteSpace(ulrId) || status.Value != "success")
            {
                isSucceed = false;
                message = messagesOptions.InvalidDataReceivedFromGateway;
            }

           
            return new PasargadRestCallbackResult
            {
                IsSucceed = isSucceed,
                InvoiceNumber = invoiceNumber.Value,
                UrlId = ulrId,
                Message = message , Data = new PasargadRestPurchaseResultData { invoice = invoiceNumber.ToString() , UrlId = ulrId }
            };
        }
        public static async Task<PaymentVerifyResult> CreateConfirmResult(HttpClient httpClient , IPasargadIdentityService identityService , PasargadRestCallbackResult callbackResult, PasargadRestGatewayAccount account , PasargadRestGatewayOptions  gatewayOptions, CancellationToken cancellationToken = default)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", await identityService.GetAccessTokenAsync(account));
            httpClient.BaseAddress = new Uri(gatewayOptions.BaseEndpoint);
            var responseMessage = await httpClient.PostAsync(
                    gatewayOptions.PaymentConfirmTransactionEndPoint, new StringContent(JsonConvert.SerializeObject(callbackResult.Data), Encoding.UTF8, "application.json"), cancellationToken)
                .ConfigureAwaitFalse();
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwaitFalse();
            if (responseMessage.IsSuccessStatusCode)
            {
                var resultModel = JsonConvert.DeserializeObject<PasargadRestResponse<PasargadRestPaymentResultInfo>>(response);

                if (resultModel.resultCode != 0)
                {
                    return PaymentVerifyResult.Failed(PasargadErrorHandler.GetErrorMessage(resultModel.resultCode));
                }
                var result = PaymentVerifyResult.Succeed(resultModel.data.referenceNumber, PasargadErrorHandler.GetErrorMessage(resultModel.resultCode));
                return result;
            }
            else
            {
                return PaymentVerifyResult.Failed("HttpStatusCode: " + responseMessage.StatusCode.ToString());
            }
        }
        public static async Task<PaymentVerifyResult> CreateVerifyResult(HttpClient httpClient , IPasargadIdentityService identityService , PasargadRestCallbackResult callbackResult, PasargadRestGatewayAccount account , PasargadRestGatewayOptions  gatewayOptions, CancellationToken cancellationToken = default)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", await identityService.GetAccessTokenAsync(account));
            httpClient.BaseAddress = new Uri(gatewayOptions.BaseEndpoint);
            var responseMessage = await httpClient.PostAsync(
                    gatewayOptions.PaymentConfirmTransactionEndPoint, new StringContent(JsonConvert.SerializeObject(callbackResult.Data), Encoding.UTF8, "application.json"), cancellationToken)
                .ConfigureAwaitFalse();
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwaitFalse();
            if (responseMessage.IsSuccessStatusCode)
            {
                var resultModel = JsonConvert.DeserializeObject<PasargadRestResponse<PasargadRestPaymentResultInfo>>(response);
                if (resultModel.resultCode != 0)
                {
                    return PaymentVerifyResult.Failed(PasargadErrorHandler.GetErrorMessage(resultModel.resultCode));
                }
                var result = PaymentVerifyResult.Succeed(resultModel.data.referenceNumber , PasargadErrorHandler.GetErrorMessage(resultModel.resultCode));
                return result;
            }
            else
            {
                return PaymentVerifyResult.Failed("HttpStatusCode: " + responseMessage.StatusCode.ToString());
            }
        }
        public static async Task<PasargadRestCheckCallbackResult> CreateCheckResult(HttpClient httpClient , IPasargadIdentityService identityService , PasargadRestCallbackResult callbackResult, PasargadRestGatewayAccount account , PasargadRestGatewayOptions  gatewayOptions, CancellationToken cancellationToken = default)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", await identityService.GetAccessTokenAsync(account));
            httpClient.BaseAddress = new Uri(gatewayOptions.BaseEndpoint);
            var responseMessage = await httpClient.PostAsync(
                    gatewayOptions.PaymentCheckTransactionEndPoint, new StringContent(JsonConvert.SerializeObject(new{ invoiceId = callbackResult.Data.invoice }), Encoding.UTF8, "application.json"), cancellationToken)
                .ConfigureAwaitFalse();
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwaitFalse();
            if (responseMessage.IsSuccessStatusCode)
            {
                var resultModel = JsonConvert.DeserializeObject<PasargadRestResponse<PasargadRestPaymentResultInfo>>(response);
                if (resultModel.resultCode != 0)
                {
                    return PasargadRestCheckCallbackResult.Failed(PasargadErrorHandler.GetErrorMessage(resultModel.resultCode));
                }
                var result = PasargadRestCheckCallbackResult.Succeed();
                return result;
            }
            else
            {
                return PasargadRestCheckCallbackResult.Failed(responseMessage.StatusCode.ToString());
            }
        }
        public static async Task<PaymentRefundResult> CreateRefundResult(HttpClient httpClient, IPasargadIdentityService identityService, PasargadRestCallbackResult callbackResult, PasargadRestGatewayAccount account, PasargadRestGatewayOptions gatewayOptions, CancellationToken cancellationToken = default)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", await identityService.GetAccessTokenAsync(account));
            httpClient.BaseAddress = new Uri(gatewayOptions.BaseEndpoint);
            var responseMessage = await httpClient.PostAsync(
                    gatewayOptions.PaymentReverseEndpoint, new StringContent(JsonConvert.SerializeObject(callbackResult.Data), Encoding.UTF8, "application.json"), cancellationToken)
                .ConfigureAwaitFalse();
            var response = await responseMessage.Content.ReadAsStringAsync().ConfigureAwaitFalse();
            if (responseMessage.IsSuccessStatusCode)
            {
                var resultModel = JsonConvert.DeserializeObject<PasargadRestResponse<PasargadRestPaymentResultInfo>>(response);

                if (resultModel.resultCode != 0)
                {
                    return PaymentRefundResult.Failed(PasargadErrorHandler.GetErrorMessage(resultModel.resultCode));
                }
                var result = PaymentRefundResult.Succeed();

                return result;
            }
            else
            {
                return PaymentRefundResult.Failed(responseMessage.StatusCode.ToString());
            }
        }
        private static string GetTimeStamp(DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
