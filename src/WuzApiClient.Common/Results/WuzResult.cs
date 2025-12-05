using System;

namespace WuzApiClient.Results;

/// <summary>
/// Represents the result of an operation without a return value.
/// </summary>
public readonly struct WuzResult : IEquatable<WuzResult>
{
    private readonly WuzApiError? error;

    private WuzResult(WuzApiError? error)
    {
        this.error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess => this.error == null;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !this.IsSuccess;

    /// <summary>
    /// Gets the error if the operation failed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if result is successful.</exception>
    public WuzApiError Error => this.error ?? throw new InvalidOperationException("WuzResult is successful; no error available.");

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="WuzResult"/>.</returns>
    public static WuzResult Success() => new(null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed <see cref="WuzResult"/>.</returns>
    public static WuzResult Failure(WuzApiError error) => new(error ?? throw new ArgumentNullException(nameof(error)));

    /// <summary>
    /// Matches the result to either success or failure handler.
    /// </summary>
    /// <typeparam name="T">The return type of the handlers.</typeparam>
    /// <param name="onSuccess">Handler to execute if result is successful.</param>
    /// <param name="onFailure">Handler to execute if result is failed.</param>
    /// <returns>The result of the executed handler.</returns>
    public T Match<T>(Func<T> onSuccess, Func<WuzApiError, T> onFailure)
    {
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        return this.IsSuccess ? onSuccess() : onFailure(this.Error);
    }

    /// <summary>
    /// Executes the appropriate action based on whether the result is successful or failed.
    /// </summary>
    /// <param name="onSuccess">Action to execute if result is successful.</param>
    /// <param name="onFailure">Action to execute if result is failed.</param>
    public void Match(Action onSuccess, Action<WuzApiError> onFailure)
    {
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        if (this.IsSuccess)
            onSuccess();
        else
            onFailure(this.Error);
    }

    /// <summary>
    /// Implicitly converts an error to a failed result.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator WuzResult(WuzApiError error) => Failure(error);

    /// <inheritdoc/>
    public bool Equals(WuzResult other) =>
        this.IsSuccess == other.IsSuccess &&
        (this.IsSuccess || this.error!.Equals(other.error));

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is WuzResult other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => this.IsSuccess ? 0 : this.error!.GetHashCode();

    /// <summary>
    /// Determines whether two results are equal.
    /// </summary>
    public static bool operator ==(WuzResult left, WuzResult right) => left.Equals(right);

    /// <summary>
    /// Determines whether two results are not equal.
    /// </summary>
    public static bool operator !=(WuzResult left, WuzResult right) => !left.Equals(right);

    /// <inheritdoc/>
    public override string ToString() => this.IsSuccess ? "Success" : $"Failure: {this.Error}";
}
