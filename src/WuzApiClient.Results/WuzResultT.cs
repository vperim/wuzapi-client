using System;

namespace WuzApiClient.Results;

/// <summary>
/// Represents the result of an operation with a return value.
/// </summary>
/// <typeparam name="T">The type of the return value.</typeparam>
public readonly struct WuzResult<T> : IEquatable<WuzResult<T>>
{
    private readonly T? value;
    private readonly WuzApiError? error;

    private WuzResult(T? value, WuzApiError? error)
    {
        this.value = value;
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
    /// Gets the value if the operation succeeded.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if result is failed.</exception>
    public T Value => this.IsSuccess
        ? this.value!
        : throw new InvalidOperationException($"WuzResult is failed; no value available. Error: {this.error}");

    /// <summary>
    /// Gets the error if the operation failed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if result is successful.</exception>
    public WuzApiError Error => this.error ?? throw new InvalidOperationException("WuzResult is successful; no error available.");

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful <see cref="WuzResult"/>.</returns>
    public static WuzResult<T> Success(T value) => new(value, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed <see cref="WuzResult"/>.</returns>
    public static WuzResult<T> Failure(WuzApiError error) => new(default, error ?? throw new ArgumentNullException(nameof(error)));

    /// <summary>
    /// Matches the result to either success or failure handler.
    /// </summary>
    /// <typeparam name="TResult">The return type of the handlers.</typeparam>
    /// <param name="onSuccess">Handler to execute if result is successful.</param>
    /// <param name="onFailure">Handler to execute if result is failed.</param>
    /// <returns>The result of the executed handler.</returns>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<WuzApiError, TResult> onFailure)
    {
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        return this.IsSuccess ? onSuccess(this.Value) : onFailure(this.Error);
    }

    /// <summary>
    /// Executes the appropriate action based on whether the result is successful or failed.
    /// </summary>
    /// <param name="onSuccess">Action to execute if result is successful.</param>
    /// <param name="onFailure">Action to execute if result is failed.</param>
    public void Match(Action<T> onSuccess, Action<WuzApiError> onFailure)
    {
        if (onSuccess == null) throw new ArgumentNullException(nameof(onSuccess));
        if (onFailure == null) throw new ArgumentNullException(nameof(onFailure));

        if (this.IsSuccess)
            onSuccess(this.Value);
        else
            onFailure(this.Error);
    }

    /// <summary>
    /// Gets the value or a default value if the result is failed.
    /// </summary>
    /// <param name="defaultValue">The default value to return if failed.</param>
    /// <returns>The value if successful; otherwise, the default value.</returns>
    public T GetValueOrDefault(T defaultValue) => this.IsSuccess ? this.Value : defaultValue;

    /// <summary>
    /// Gets the value or the result of a factory function if the result is failed.
    /// </summary>
    /// <param name="defaultFactory">The factory function to create a default value.</param>
    /// <returns>The value if successful; otherwise, the result of the factory function.</returns>
    public T GetValueOrDefault(Func<T> defaultFactory)
    {
        if (defaultFactory == null) throw new ArgumentNullException(nameof(defaultFactory));
        return this.IsSuccess ? this.Value : defaultFactory();
    }

    /// <summary>
    /// Converts the result to a non-generic result, discarding the value.
    /// </summary>
    /// <returns>A non-generic <see cref="WuzResult"/>.</returns>
    public WuzResult ToResult() => this.IsSuccess ? WuzResult.Success() : WuzResult.Failure(this.Error);

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator WuzResult<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts an error to a failed result.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator WuzResult<T>(WuzApiError error) => Failure(error);

    /// <inheritdoc/>
    public bool Equals(WuzResult<T> other)
    {
        if (this.IsSuccess != other.IsSuccess)
            return false;

        if (this.IsSuccess)
        {
            if (this.value == null && other.value == null)
                return true;
            if (this.value == null || other.value == null)
                return false;
            return this.value.Equals(other.value);
        }

        return this.error!.Equals(other.error);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is WuzResult<T> other && this.Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (this.IsSuccess)
            return this.value?.GetHashCode() ?? 0;
        return this.error!.GetHashCode();
    }

    /// <summary>
    /// Determines whether two results are equal.
    /// </summary>
    public static bool operator ==(WuzResult<T> left, WuzResult<T> right) => left.Equals(right);

    /// <summary>
    /// Determines whether two results are not equal.
    /// </summary>
    public static bool operator !=(WuzResult<T> left, WuzResult<T> right) => !left.Equals(right);

    /// <inheritdoc/>
    public override string ToString() => this.IsSuccess ? $"Success: {this.Value}" : $"Failure: {this.Error}";
}
