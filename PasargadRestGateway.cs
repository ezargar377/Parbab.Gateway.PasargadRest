


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Parbad.Abstraction;
using Parbad.Gateway.PasargadRest.Internal;
using Parbad.GatewayBuilders;
using Parbad.Internal;
using Parbad.Net;
using Parbad.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Parbad.Gateway.PasargadRest
{
    [Gateway(Name)]
    public class PasargadRestGateway : GatewayBase<PasargadRestGatewayAccount>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasargadIdentityService identityService;
        private readonly HttpClient _httpClient;
        private readonly PasargadRestGatewayOptions _gatewayOptions;
        private readonly IOptions<MessagesOptions> _messageOptions;
        public const string Name = "PasargadRest";

        public PasargadRestGateway(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory, IPasargadIdentityService identityService,
            IGatewayAccountProvider<PasargadRestGatewayAccount> accountProvider,
            IOptions<PasargadRestGatewayOptions> gatewayOptions,
            IOptions<MessagesOptions> messageOptions) : base(accountProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            this.identityService = identityService;
            _httpClient = httpClientFactory.CreateClient(this);
            _gatewayOptions = gatewayOptions.Value;
            _messageOptions = messageOptions;
        }

        /// <inheritdoc />
        public override async Task<IPaymentRequestResult> RequestAsync(Invoice invoice, CancellationToken cancellationToken = default)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));
            var account = await GetAccountAsync(invoice).ConfigureAwaitFalse();
            return await PasargadRestHelper.CreateRequestResult(invoice, _httpContextAccessor.HttpContext, identityService, account, _gatewayOptions, _httpClient);
        }

        /// <inheritdoc />
        public override async Task<IPaymentFetchResult> FetchAsync(InvoiceContext context, CancellationToken cancellationToken = default)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var callbackResult = await PasargadRestHelper.CreateCallbackResult(
                    _httpContextAccessor.HttpContext.Request,
                    _messageOptions.Value,
                    cancellationToken)
                .ConfigureAwaitFalse();

            if (callbackResult.IsSucceed)
            {
                return PaymentFetchResult.ReadyForVerifying();
            }

            return PaymentFetchResult.Failed(callbackResult.Message);
        }

        /// <inheritdoc />
        public override async Task<IPaymentVerifyResult> VerifyAsync(InvoiceContext context, CancellationToken cancellationToken = default)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var callbackResult = await PasargadRestHelper.CreateCallbackResult(
                _httpContextAccessor.HttpContext.Request,
                _messageOptions.Value,
                cancellationToken)
                .ConfigureAwaitFalse();

            if (!callbackResult.IsSucceed)
            {
                return PaymentVerifyResult.Failed(callbackResult.Message);
            }
            var account = await GetAccountAsync(context.Payment).ConfigureAwaitFalse();
            
            
            var confirmResult = await PasargadRestHelper.CreateCheckResult(_httpClient, identityService, callbackResult, account, _gatewayOptions);

            if (!confirmResult.IsSucceed)
            {
                return PaymentVerifyResult.Failed(confirmResult.Message);
            }
            return await PasargadRestHelper.CreateVerifyResult(_httpClient, identityService, callbackResult, account, _gatewayOptions);
        }

        /// <inheritdoc />
        public override async Task<IPaymentRefundResult> RefundAsync(InvoiceContext context, Money amount, CancellationToken cancellationToken = default)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var callbackResult = await PasargadRestHelper.CreateCallbackResult(
                _httpContextAccessor.HttpContext.Request,
                _messageOptions.Value,
                cancellationToken)
                .ConfigureAwaitFalse();

            if (!callbackResult.IsSucceed)
            {
                return PaymentRefundResult.Failed(callbackResult.Message);
            }
            var account = await GetAccountAsync(context.Payment).ConfigureAwaitFalse();
            var data = await PasargadRestHelper.CreateRefundResult(_httpClient, identityService, callbackResult, account, _gatewayOptions);
            return data;
        }
    }
}
