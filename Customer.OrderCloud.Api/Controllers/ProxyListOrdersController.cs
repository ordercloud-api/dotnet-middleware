using Microsoft.AspNetCore.Mvc;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System.Threading.Tasks;

namespace Catalyst.Api.Controllers
{
	// This code snippet shows how to proxy an Ordercloud list route. 
	// Imagine a franchise business with multiple locations. Owners and Employees at a location both place orders. Only Owners should see all orders for their location.
	// As the developer, you'd want to give buyer users with a specific flag read access to specific orders.
	// You may need to proxy the Ordercloud List Orders endpoint server-side to achieve the correct permissions behavior.
	[Route("api/proxy")]
	public class ProxyListOrdersController : CatalystController
	{
		private readonly IOrderCloudClient _oc;
		private readonly DecodedToken _user;

		public ProxyListOrdersController(IOrderCloudClient oc, DecodedToken user)
		{
			_oc = oc;
			_user = user;
		}

		// Users with the Shopper role can access this route. Typically, anyone purchasing products has this permission.
		[HttpGet("orders/for/my/location"), OrderCloudUserAuth(ApiRole.Shopper)]
		// The IListArgs model describes list arguments that let api users query data expressively with query params. 
		public async Task<ListPage<Order>> ListOrdersForBillingAddress(IListArgs args)
		{
			var me = await _user.BuildClient().Me.GetAsync();
			if (me.xp.FranchiseRole != "Owner")
			{
				throw new UnAuthorizedException();
			}
			var locationID = me.xp.BillingAddressID;
			var billingAddressFilter = new ListFilter("BillingAddress.ID", locationID);
			// Add a filter on top of any user-defined filters. Only return orders where Order.BillingAddress.ID equals the user's.   
			args.Filters.Add(billingAddressFilter);
			// Request orders from Ordercloud from the admin endpoint with elevated access.
			var orders = await _oc.Orders.ListAsync(OrderDirection.Incoming,
				// Apply all the list arguments to the request
				page: args.Page,
				pageSize: args.PageSize,
				sortBy: string.Join(',', args.SortBy),
				search: args.Search,
				searchOn: args.SearchOn,
				filters: args.ToFilterString());
			return orders;
		}
	}
}
