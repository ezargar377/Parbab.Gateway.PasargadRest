using Parbad.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parbad.Gateway.PasargadRest
{
    public interface IPasargadIdentityService
    {
        Task<string> GetAccessTokenAsync(PasargadRestGatewayAccount account);
    }
}
