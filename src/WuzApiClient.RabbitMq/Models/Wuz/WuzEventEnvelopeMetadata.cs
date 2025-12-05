namespace WuzApiClient.RabbitMq.Models.Wuz;

public sealed record WuzEventEnvelopeMetadata(string InstanceName, string JsonData, string UserId);