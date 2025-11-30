using AwesomeAssertions;
using WuzApiClient.Utilities;

namespace WuzApiClient.UnitTests.Utilities;

[Trait("Category", "Unit")]
public sealed class FileEncoderTests
{
    [Fact]
    public async Task EncodeFileAsync_ValidImageFile_ReturnsDataUrl()
    {
        var tempFile = Path.GetTempFileName();
        var testData = new byte[] { 1, 2, 3, 4, 5 };
        await File.WriteAllBytesAsync(tempFile, testData);

        try
        {
            var result = await FileEncoder.EncodeFileAsync(tempFile, "image/png");

            result.IsSuccess.Should().BeTrue();
            result.EncodedData.Should().NotBeNull();
            result.EncodedData.Should().StartWith("data:image/png;base64,");
            var base64Part = result.EncodedData!.Substring("data:image/png;base64,".Length);
            var decodedBytes = Convert.FromBase64String(base64Part);
            decodedBytes.Should().Equal(testData);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task EncodeFileAsync_NonExistentFile_ReturnsFailure()
    {
        var nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent-file-" + Guid.NewGuid() + ".png");

        var result = await FileEncoder.EncodeFileAsync(nonExistentFile, "image/png");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
        result.Error.Should().Contain("File not found");
    }

    [Fact]
    public async Task EncodeFileAsync_EmptyFile_ReturnsEmptyDataUrl()
    {
        var tempFile = Path.GetTempFileName();

        try
        {
            var result = await FileEncoder.EncodeFileAsync(tempFile, "text/plain");

            result.IsSuccess.Should().BeTrue();
            result.EncodedData.Should().Be("data:text/plain;base64,");
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Theory]
    [InlineData("application/pdf")]
    [InlineData("image/jpeg")]
    [InlineData("video/mp4")]
    [InlineData("audio/mpeg")]
    public async Task EncodeFileAsync_VariousMimeTypes_ReturnsCorrectDataUrl(string mimeType)
    {
        var tempFile = Path.GetTempFileName();
        var testData = new byte[] { 10, 20, 30 };
        await File.WriteAllBytesAsync(tempFile, testData);

        try
        {
            var result = await FileEncoder.EncodeFileAsync(tempFile, mimeType);

            result.IsSuccess.Should().BeTrue();
            result.EncodedData.Should().StartWith($"data:{mimeType};base64,");
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task EncodeStreamAsync_ValidStream_ReturnsDataUrl()
    {
        var testData = new byte[] { 10, 20, 30, 40, 50 };
        using var stream = new MemoryStream(testData);

        var result = await FileEncoder.EncodeStreamAsync(stream, "application/pdf");

        result.Should().StartWith("data:application/pdf;base64,");
        var base64Part = result.Substring("data:application/pdf;base64,".Length);
        var decodedBytes = Convert.FromBase64String(base64Part);
        decodedBytes.Should().Equal(testData);
    }

    [Fact]
    public async Task EncodeStreamAsync_EmptyStream_ReturnsEmptyDataUrl()
    {
        using var stream = new MemoryStream();

        var result = await FileEncoder.EncodeStreamAsync(stream, "text/plain");

        result.Should().Be("data:text/plain;base64,");
    }

    [Fact]
    public async Task EncodeStreamAsync_SeekableStream_RestoresPosition()
    {
        var testData = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(testData);
        stream.Position = 2;

        await FileEncoder.EncodeStreamAsync(stream, "image/jpeg");

        stream.Position.Should().Be(2);
    }

    [Fact]
    public async Task EncodeStreamAsync_NonSeekableStream_ReturnsDataUrl()
    {
        var testData = new byte[] { 5, 10, 15, 20 };
        using var baseStream = new MemoryStream(testData);
        using var nonSeekableStream = new NonSeekableStream(baseStream);

        var result = await FileEncoder.EncodeStreamAsync(nonSeekableStream, "image/png");

        result.Should().StartWith("data:image/png;base64,");
        var base64Part = result.Substring("data:image/png;base64,".Length);
        var decodedBytes = Convert.FromBase64String(base64Part);
        decodedBytes.Should().Equal(testData);
    }

    [Theory]
    [InlineData(new byte[] { 0 })]
    [InlineData(new byte[] { 255 })]
    [InlineData(new byte[] { 0, 127, 255 })]
    public async Task EncodeStreamAsync_VariousByteValues_EncodesCorrectly(byte[] testData)
    {
        using var stream = new MemoryStream(testData);

        var result = await FileEncoder.EncodeStreamAsync(stream, "application/octet-stream");

        var base64Part = result.Substring("data:application/octet-stream;base64,".Length);
        var decodedBytes = Convert.FromBase64String(base64Part);
        decodedBytes.Should().Equal(testData);
    }

    [Fact]
    public async Task EncodeFileAsync_WithCancellationToken_CancelsOperation()
    {
        var tempFile = Path.GetTempFileName();
        var testData = new byte[1024];
        await File.WriteAllBytesAsync(tempFile, testData);

        try
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            var act = async () => await FileEncoder.EncodeFileAsync(tempFile, "image/png", cts.Token);

            await act.Should().ThrowAsync<OperationCanceledException>();
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task EncodeStreamAsync_WithCancellationToken_CancelsOperation()
    {
        var testData = new byte[1024];
        using var stream = new MemoryStream(testData);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await FileEncoder.EncodeStreamAsync(stream, "image/png", cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task EncodeStreamAsync_LargeStream_EncodesSuccessfully()
    {
        var testData = new byte[1024 * 100];
        new Random(42).NextBytes(testData);
        using var stream = new MemoryStream(testData);

        var result = await FileEncoder.EncodeStreamAsync(stream, "application/octet-stream");

        result.Should().StartWith("data:application/octet-stream;base64,");
        var base64Part = result.Substring("data:application/octet-stream;base64,".Length);
        var decodedBytes = Convert.FromBase64String(base64Part);
        decodedBytes.Should().Equal(testData);
    }

    /// <summary>
    /// Helper class to simulate a non-seekable stream for testing.
    /// </summary>
    private sealed class NonSeekableStream : Stream
    {
        private readonly Stream innerStream;

        public NonSeekableStream(Stream innerStream)
        {
            this.innerStream = innerStream;
        }

        public override bool CanRead => this.innerStream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => this.innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) =>
            this.innerStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) =>
            throw new NotSupportedException();

        public override void SetLength(long value) =>
            throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            await this.innerStream.ReadAsync(buffer, offset, count, cancellationToken);

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            await this.innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }
}
