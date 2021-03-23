public class PreWebhookResponse
{
    /// <summary>
    /// Returns a webhook response indicating that OrderCloud.io should proceed with normal processing of the request.
    /// </summary>
    public static PreWebhookResponse Proceed() => new PreWebhookResponse { proceed = true };

    /// <summary>
    /// Returns a webhook response indicating that OrderCloud.io should NOT proceed with normal processing of the request.
    /// </summary>
    /// <returns></returns>
    public static PreWebhookResponse Block() => new PreWebhookResponse { proceed = false, body = "Access Denied" };

    /// <summary>
    /// Returns a webhook response indicating that OrderCloud.io should NOT proceed with normal processing of the request.
    /// </summary>
    /// <param name="response">
    /// A response to pass along to the client when blocking a request
    /// </param>
    /// <returns></returns>
  
    public static PreWebhookResponse Block(object response) => new PreWebhookResponse { proceed = false, body = response };

    /// <summary>
    /// A value indicating whether OrderCloud.io should proceed with normal processing of the request.
    /// </summary>
    public bool proceed { get; set; }

    /// <summary>
    /// The body that the client will recieve if a request is blocked
    /// </summary>
    public object body { get; set; } 
}