using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OrderCloud.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.RegularExpressions;

namespace Customer.OrderCloud.Common.Commands
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public class OrderCloudIntegrationEventAttribute : Attribute
	{
		public ApiRole[] ElevatedRoles { get; }
		public object ConfigData { get; }
		public IntegrationEventType EventType { get;  }
		public OrderCloudIntegrationEventAttribute(IntegrationEventType eventType, ApiRole[] elevatedRoles = null, object configData = null) 
		{
			ElevatedRoles = elevatedRoles;
			ConfigData = configData;
			EventType = eventType;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class OrderCloudWebhookAttribute : Attribute
	{
		public string Description { get; }
		public ApiRole[] ElevatedRoles { get; }
		public object ConfigData { get; }
		public OrderCloudWebhookAttribute(string description = null, ApiRole[] elevatedRoles = null, object configData = null)
		{
			Description = description;
			ElevatedRoles = elevatedRoles;
			ConfigData = configData;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class OrderCloudCustomMessageSenderAttribute : Attribute
	{
		public MessageType[] MessageTypes { get; }
		public string Description { get; }
		public ApiRole[] ElevatedRoles { get; }
		public object xp { get; }
		public OrderCloudCustomMessageSenderAttribute(MessageType[] messageTypes, string description = null, ApiRole[] elevatedRoles = null, object xp = null)
		{
			Description = description;
			MessageTypes = messageTypes;
			ElevatedRoles = elevatedRoles;
			this.xp = xp;
		}
	}

	public class WebhookResources
	{
		public List<Webhook> Webhooks { get; set; } = new List<Webhook>();
		public List<IntegrationEvent> IntegrationEvents { get; set; } = new List<IntegrationEvent>();
		public List<MessageSender> CustomMessageSenders { get; set; } = new List<MessageSender>();
	}

	public class OrderCloudWebhookRouteAnalyser
	{
		public const string HASH_KEY_PLACEHOLDER = "<HASH_KEY_PLACEHOLDER>";
		public const string BASE_URL_PLACEHOLDER = "<BASE_URL_PLACEHOLDER>";

		private readonly IApiDescriptionGroupCollectionProvider _apiDescriptionGroupCollectionProvider;

		public OrderCloudWebhookRouteAnalyser(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
		{
			_apiDescriptionGroupCollectionProvider = apiDescriptionGroupCollectionProvider;
		}

		public WebhookResources AnalyzeProjectWebhookRoutes(string projectBaseUrl = BASE_URL_PLACEHOLDER, string hashKey = HASH_KEY_PLACEHOLDER)
		{
			var webhookResources = new WebhookResources();
			foreach (var group in _apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items)
			{
				foreach (var apiDescription in group.Items)
				{
					var action = apiDescription.ActionDescriptor as ControllerActionDescriptor;
					var webhook = GetAttribute<OrderCloudWebhookAttribute>(action.MethodInfo);
					if (webhook != null)
					{
						webhookResources.Webhooks.Add(new Webhook()
						{
							ID = MakeStringIDSafe($"{action.ControllerName}-{action.ActionName}"),
							Name = action.ActionName,
							ElevatedRoles = webhook.ElevatedRoles,
							ConfigData = webhook.ConfigData,
							Url = $"{projectBaseUrl}/{apiDescription.RelativePath}",
							// WebhookRoutes =
							// BeforeProcessRequest = 
							// API clients =
							Description = webhook.Description,
							HashKey = hashKey,
						});
						continue;
					}
					var integrationEvent = GetAttribute<OrderCloudIntegrationEventAttribute>(action.ControllerTypeInfo);
					if (integrationEvent != null)
					{
						var id = MakeStringIDSafe($"{integrationEvent.EventType}-{action.ControllerName}");
						if (!webhookResources.IntegrationEvents.Any(e => e.ID == id))
						{
							webhookResources.IntegrationEvents.Add(new IntegrationEvent()
							{
								ID = id,
								Name = action.ControllerName,
								ElevatedRoles = integrationEvent.ElevatedRoles,
								ConfigData = integrationEvent.ConfigData,
								CustomImplementationUrl = $"{projectBaseUrl}/{apiDescription.RelativePath}",
								EventType = integrationEvent.EventType,
								HashKey = hashKey,
							});
						}
					
						continue;
					}
					var messageSender = GetAttribute<OrderCloudCustomMessageSenderAttribute>(action.MethodInfo);
					if (messageSender != null)
					{
						webhookResources.CustomMessageSenders.Add(new MessageSender()
						{
							ID = MakeStringIDSafe($"generated-{action.ControllerName}-{action.ActionName}"),
							Name = action.ActionName,
							Description = messageSender.Description,
							MessageTypes = messageSender.MessageTypes,
							ElevatedRoles = messageSender.ElevatedRoles,
							xp = messageSender.xp,
							SharedKey = hashKey,
							URL = $"{projectBaseUrl}/{apiDescription.RelativePath}",
						});
						continue;
					}
				}
			}

			return webhookResources;
		}

		private T GetAttribute<T>(MemberInfo member) where T : Attribute
		{
			return member.GetCustomAttributes(typeof(T), false).FirstOrDefault() as T;
		}

		private string MakeStringIDSafe(string idCandidate)
		{
			return Regex.Replace(idCandidate, "^[a-zA-Z0-9_-]", "_");
		}
	}
}
