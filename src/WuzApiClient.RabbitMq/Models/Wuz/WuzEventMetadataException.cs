using System;

namespace WuzApiClient.RabbitMq.Models.Wuz;

public sealed class WuzEventMetadataException : Exception
{
    public WuzEventMetadataException(string message) : base(message)
    {
    }
}