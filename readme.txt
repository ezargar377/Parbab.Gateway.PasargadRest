---------------------------------------------------
Parbad - Online Payment Library for .NET developers
				PasargadRest Gateway
---------------------------------------------------

GitHub: https://github.com/ezargar377/Parbab.Gateway.PasargadRest
Tutorials: https://github.com/Sina-Soltani/Parbad/wiki

-------------
Configuration
-------------

.ConfigureGateways(gateways =>
{
    gateways
        .AddPasargadRest()
        .WithAccounts(accounts =>
        {
            accounts.AddInMemory(account =>
            {
                account.TerminalCode = <Your ID>;
                account.UserNamne = "<Your UserName>";
                account.Password = "<Your Password>";
            });
        });
})

-------------
Making a request
-------------

var result = _onlinePayment.RequestAsync(invoice => 
{
    invoice
	.UsePasargadRest()
    .SetPasargadCellNumber("<User Phone Number>")
})

-------------
Getting the original verification result
-------------
var result = _onlinePayment.VerifyAsync(invoice);


