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
		var rates = await _shippingEstimatorEstimateShippingAsync(packageList);
		// ...
	}

	[HttpPost, Route("ordercalculate"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<OrderCalculateResponse> CalculateOrder([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		...
		var taxCalculation = await _taxCalculator.CalculateEstimateAsync(orderSummary);
		// ...
	}

	[HttpPost, Route("ordersubmit"), OrderCloudWebhookAuth] // route and method specified by OrderCloud platform
	public async Task<OrderSubmitResponse> HandleOrderSubmit([FromBody] OrderCalculatePayload<CheckoutConfig> payload)
	{
		// ...
		var authorizeResult = await _creditCardProcessor.AuthorizeOnlyAsync(transaction);
		// ...
		await _taxCalculator.CommitTransactionAsync(orderSummary)
	}
}
```


```c#
using OrderCloud.Catalyst.Shipping.EasyPost;
using OrderCloud.Catalyst.Tax.Avalara;
using OrderCloud.Catalyst.Tax.Stripe;

public class Startup
{
	public virtual void ConfigureServices(IServiceCollection services) {
		// ...
		services.AddSingleton<IShippingEstimator>(new EasyPostService() { ... });
		services.AddSingleton<ITaxCalculator>(new AvalaraService() { ... });
		services.AddSingleton<ICreditCardProcessor>(new StripeService() { ... });
		// ...
	}
}
