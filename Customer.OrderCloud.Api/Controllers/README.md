```c#	
public class CheckoutIntegrationController
{
	private readonly IShipMethodCalculator _shipMethodCalculator;
	private readonly ITaxCalculator _taxCalculator;
	private readonly ICreditCardProcessor _creditCardProcessor;

	public CheckoutIntegrationController(IShipMethodCalculator shipMethodCalculator, ITaxCalculator taxCalculator, ICreditCardProcessor creditCardProcessor)
	{
		// assign here
	}

	[HttpPost, Route("shippingrates"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<ShipEstimateResponse> GetShippingRates([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		...
		var rates = _shipMethodCalculator.CalculateShipMethodsAsync(packageList);
		...
	}

	[HttpPost, Route("ordercalculate"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		...
		var taxCalculation = _taxCalculator.CalculateEstimateAsync(orderSummary);
		...
	}

	[HttpPost, Route("ordersubmit"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<OrderSubmitResponse> HandleOrderSubmit([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		...
		var authorizeResult = _creditCardProcessor.AuthorizeOnlyAsync(transaction);
		...
	}
}
```
