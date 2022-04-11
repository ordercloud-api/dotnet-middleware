```c#	
public class CheckoutIntegrationController
{
	private readonly IShippingEstimator _shippingEstimator;
	private readonly ITaxCalculator _taxCalculator;
	private readonly ICreditCardProcessor _creditCardProcessor;

	public CheckoutIntegrationController(IShippingEstimator shippingEstimator, ITaxCalculator taxCalculator, ICreditCardProcessor creditCardProcessor)
	{
		// assign here
	}

	[HttpPost, Route("shippingrates"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<ShipEstimateResponse> GetShippingRates([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		// ...
		var rates = _shippingEstimator.CalculateShipMethodsAsync(packageList);
		// ...
	}

	[HttpPost, Route("ordercalculate"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		...
		var taxCalculation = _taxCalculator.CalculateEstimateAsync(orderSummary);
		// ...
	}

	[HttpPost, Route("ordersubmit"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<OrderSubmitResponse> HandleOrderSubmit([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		// ...
		var authorizeResult = _creditCardProcessor.AuthorizeOnlyAsync(transaction);
		// ...
	}
}
```
