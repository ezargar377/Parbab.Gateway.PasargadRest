using System;
using Parbad.Abstraction;
using Parbad.Gateway.Parsian;
using Parbad.Gateway.PasargadRest.Internal;
using Parbad.Gateway.Saman.Internal;
using Parbad.InvoiceBuilder;

namespace Parbad.Gateway.PasargadRest
{
    public static class PasargadRestGatewayInvoiceBuilderExtensions
    {
        /// <summary>
        /// The invoice will be sent to Pasargad gateway.
        /// </summary>
        /// <param name="builder"></param>
        public static IInvoiceBuilder UsePasargadRest(this IInvoiceBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            return builder.SetGateway(PasargadRestGateway.Name);
        }



        /// <summary>
        /// Add Mobile Number.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="enable">If true, the invoice will be sent to pasargad  Mobile gateway. Otherwise it will be sent to pasargad Web gateway.</param>
        public static IInvoiceBuilder AddPasargadCellNumber(this IInvoiceBuilder builder, long mobileNumber)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.AddOrUpdateProperty(PasargadRestHelper.CellNumber, mobileNumber);

            return builder;
        }

        internal static long? CellNumber(this Invoice invoice)
        {
            if (invoice == null) throw new ArgumentNullException(nameof(invoice));

            return invoice.Properties.ContainsKey(PasargadRestHelper.CellNumber) ?
                   (long)invoice.Properties[PasargadRestHelper.CellNumber] : null;
        }
    }
}
