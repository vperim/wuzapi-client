namespace WuzApiClient.RabbitMq.Models.Wuz;

/// <summary>
/// Transport metadata from wuzapi RabbitMQ messages.
/// Contains user/instance identification added by wuzapi to the flattened event payload.
/// </summary>
public sealed record WuzEventEnvelopeMetadata(string InstanceName, string UserId);
