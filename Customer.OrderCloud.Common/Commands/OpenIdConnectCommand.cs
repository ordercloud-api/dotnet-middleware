using Customer.OrderCloud.Common.Models;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.OrderCloud.Common.Commands
{
	public interface IOpenIdConnectCommand
	{
		Task<OpenIdConnectCreateUserResponse> CreateUserFromSSOAsync(OpenIDConnectIEPayloadWithXp payload);
		Task<OpenIdConnectSyncUserResponse> UpdateUserFromSSOAsync(OpenIDConnectIEPayloadWithXp payload);
	}

	public class OpenIdConnectCommand : IOpenIdConnectCommand
	{
		private readonly IOrderCloudClient _oc;

		public OpenIdConnectCommand(IOrderCloudClient oc)
		{
			_oc = oc;
		}

		public async Task<OpenIdConnectCreateUserResponse> CreateUserFromSSOAsync(OpenIDConnectIEPayloadWithXp payload)
		{
			// Get User details from external identity providing system and create user in OrderCloud.

			return new OpenIdConnectCreateUserResponse();
		}

		public async Task<OpenIdConnectSyncUserResponse> UpdateUserFromSSOAsync(OpenIDConnectIEPayloadWithXp payload)
		{
			// Get User details from external identity providing system and patch user in OrderCloud if necesary.


			return new OpenIdConnectSyncUserResponse();
		}
	}
}
