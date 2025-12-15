using System;
using System.Diagnostics.CodeAnalysis;

namespace WuzApiClient.Extensions.Utilities
{
    /// <summary>
    /// Result of file encoding operation.
    /// </summary>
    internal readonly record struct FileEncodeResult
    {
        /// <summary>
        /// Gets a value indicating whether the encoding was successful.
        /// </summary>
        [MemberNotNullWhen(true, nameof(EncodedData))]
        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Gets the encoded data URL on success.
        /// Throws <see cref="InvalidOperationException"/> if accessed when <see cref="IsSuccess"/> is false.
        /// </summary>
        public string? EncodedData { get; init; }

        /// <summary>
        /// Gets the error message on failure.
        /// Throws <see cref="InvalidOperationException"/> if accessed when <see cref="IsSuccess"/> is true.
        /// </summary>
        public string? Error { get; init; }

        /// <summary>
        /// Creates a success result.
        /// </summary>
        /// <param name="encodedData">The encoded data URL.</param>
        /// <exception cref="ArgumentNullException">Thrown when encodedData is null or empty.</exception>
        public static FileEncodeResult Success(string encodedData)
        {
            if (string.IsNullOrEmpty(encodedData))
            {
                throw new ArgumentNullException(nameof(encodedData), "Encoded data cannot be null or empty.");
            }

            return new FileEncodeResult { IsSuccess = true, EncodedData = encodedData };
        }

        /// <summary>
        /// Creates a failure result.
        /// </summary>
        /// <param name="error">The error message.</param>
        /// <exception cref="ArgumentNullException">Thrown when error is null or empty.</exception>
        public static FileEncodeResult Failure(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                throw new ArgumentNullException(nameof(error), "Error message cannot be null or empty.");
            }

            return new FileEncodeResult { IsSuccess = false, Error = error };
        }
    }
}
