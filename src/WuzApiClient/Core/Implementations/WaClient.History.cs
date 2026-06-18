using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WuzApiClient.Common.Results;
using WuzApiClient.Models.Requests.Session;
using WuzApiClient.Models.Responses.Chat;
using WuzApiClient.Models.Responses.Session;

namespace WuzApiClient.Core.Implementations;

// Message/chat history operations.
public sealed partial class WaClient
{
    /// <inheritdoc/>
    public async Task<WuzResult<HistoryMessageResponse[]>> GetChatHistoryAsync(
        string chatJid,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<HistoryMessageResponse[]>(
            $"/chat/history?chat_jid={Uri.EscapeDataString(chatJid)}&limit={limit}",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<Dictionary<string, ChatHistoryIndexEntry[]>>> GetChatHistoryIndexAsync(
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.GetAsync<Dictionary<string, ChatHistoryIndexEntry[]>>(
            "/chat/history?chat_jid=index",
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<RequestHistorySyncResponse>> RequestHistorySyncAsync(
        int count = 50,
        string? chatJid = null,
        CancellationToken cancellationToken = default)
    {
        var path = $"/session/history?count={count}";
        if (!string.IsNullOrWhiteSpace(chatJid))
            path += $"&chat_jid={Uri.EscapeDataString(chatJid)}";

        return await this.httpClient.GetAsync<RequestHistorySyncResponse>(
            path,
            "Token",
            this.UserToken,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<WuzResult<SetHistoryResponse>> SetHistoryAsync(
        int history,
        CancellationToken cancellationToken = default)
    {
        return await this.httpClient.PostAsync<SetHistoryResponse>(
            "/session/history",
            "Token",
            this.UserToken,
            new SetHistoryRequest { History = history },
            cancellationToken).ConfigureAwait(false);
    }
}
