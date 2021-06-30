using Catalyst.Common.Models;
using Catalyst.Common.Services;
using OrderCloud.Catalyst;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalyst.Common.Commands

{
	public interface ICheckoutIntegrationCommand
	{
		Task<ShipEstimateResponse> GetShippingRates(OrderCalculatePayload<CheckoutConfig> payload);
		Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload<CheckoutConfig> payload);
		Task<OrderSubmitResponse> HandleOrderReleased(OrderCalculatePayload<CheckoutConfig> payload);
	}

	public class CheckoutIntegrationCommand : ICheckoutIntegrationCommand
	{
		private readonly IAzureServiceBus _serviceBus;
		private readonly AppSettings _settings;
		public CheckoutIntegrationCommand(
			IAzureServiceBus serviceBus,
			AppSettings settings
		)
        {
			_serviceBus = serviceBus;
			_settings = settings;
        }
		public async Task<ShipEstimateResponse> GetShippingRates(OrderCalculatePayload<CheckoutConfig> payload)
		{
			var response = new ShipEstimateResponse();
			// Group lineItems into shipments 
			var shipments = payload.OrderWorksheet.LineItems.GroupBy(li => li.ShipFromAddress.ID);
			foreach (var shipment in shipments)
			{
				// Use a shipping estimator (EasyPost, Fedex, UPS, ect) to get shipMethods 
				var shipMethods = new List<ShipMethod>()
				{
					new ShipMethod()
					{
						Name = "Fedex 2 Day Priority",
						Cost = 12.45M,
						EstimatedTransitDays = 2
					}
				};
				response.ShipEstimates.Add(new ShipEstimate() { ShipMethods = shipMethods });
			}
			return response;
		}

		public async Task<OrderCalculateResponse> CalculateOrder(OrderCalculatePayload<CheckoutConfig> payload)
		{
			var response = new OrderCalculateResponse();
			// Get tax details from a provider like Avalara, Vertex, Tax Jar
			var taxDetails = new TaxDetails();
			response.TaxTotal = taxDetails.TotalTax; // Populate Total Tax field on the Order
			response.xp = new
			{
				TaxDetails = taxDetails // Save all other tax details in xp for auditing 
			};
			return response;
		}

		public async Task<OrderSubmitResponse> HandleOrderReleased(OrderCalculatePayload<CheckoutConfig> payload)
		{
			// Send email to the purchaser
			var toEmail = payload.OrderWorksheet.Order.FromUser.Email;

			// Forward order to an ERP or fullfilment system
			var message = new OrderQueueMessage
			{
				OrderID = payload.OrderWorksheet.Order.ID
			};
			await _serviceBus.SendMessage(_settings.ServiceBusSettings.OrderProcessingQueueName, message, afterMinutes: 1);

			// Return response
			return new OrderSubmitResponse()
			{
				HttpStatusCode = 200,
				xp = new 
				{
					NeedsAttention = false,
					OrderSubmitResponseCode = 200
				}
			};
		}
	}

	// Data configured in OrderCloud to be passed to all Checkout Integration Events as Payload.ConfigData
	public class CheckoutConfig
	{
		public string MyProperty { get; set; }
	}


	public class TaxDetails
	{
		public decimal TotalTax { get; set; }
		public string CurrencyCode { get; set; }
		public DateTime TaxDate { get; set; }
		public List<TaxForJurisdiction> Jurisdictions { get; set; }
	}

	public class TaxForJurisdiction
	{
		public string JurisdictionName { get; set; }
		public string TotalTaxable { get; set; }
		public string TaxRate { get; set; }
		public string TaxAmount { get; set; }

	}
}
