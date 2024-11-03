using System;
using Microsoft.Extensions.DependencyInjection;
using ParbadGateway.PasargadRest.Internal;
using Parbad.GatewayBuilders;

namespace ParbadGateway.PasargadRest
{
    public static class PasargadRestGatewayBuilderExtensions
    {
        /// <summary>
        /// Adds PasargadRest gateway to Parbad services.
        /// </summary>
        /// <param name="builder"></param>
        public static IGatewayConfigurationBuilder<PasargadRestGateway> AddPasargadRest(this IGatewayBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IPasargadIdentityService, PasargadIdentityService>();

            return builder
                .AddGateway<PasargadRestGateway>()
                .WithHttpClient(clientBuilder => { })
                .WithOptions(options => { });
        }

        /// <summary>
        /// Configures the accounts for <see cref="PasargadRestGateway"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureAccounts">Configures the accounts.</param>
        public static IGatewayConfigurationBuilder<PasargadRestGateway> WithAccounts(
            this IGatewayConfigurationBuilder<PasargadRestGateway> builder,
            Action<IGatewayAccountBuilder<PasargadRestGatewayAccount>> configureAccounts)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.WithAccounts(configureAccounts);
        }

        /// <summary>
        /// Configures the options for PasargadRest Gateway.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions">Configuration</param>
        public static IGatewayConfigurationBuilder<PasargadRestGateway> WithOptions(
            this IGatewayConfigurationBuilder<PasargadRestGateway> builder,
            Action<PasargadRestGatewayOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);

            return builder;
        }
    }
}
