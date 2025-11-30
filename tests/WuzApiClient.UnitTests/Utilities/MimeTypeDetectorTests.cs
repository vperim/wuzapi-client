using AwesomeAssertions;
using WuzApiClient.Utilities;

namespace WuzApiClient.UnitTests.Utilities;

[Trait("Category", "Unit")]
public sealed class MimeTypeDetectorTests
{
    [Theory]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("photo.jpeg", "image/jpeg")]
    [InlineData("picture.png", "image/png")]
    [InlineData("animation.gif", "image/gif")]
    [InlineData("graphic.webp", "image/webp")]
    public void DetectFromExtension_KnownImageExtension_ReturnsCorrectMimeType(string fileName, string expectedMimeType)
    {
        var result = MimeTypeDetector.DetectFromExtension(fileName);

        result.Should().Be(expectedMimeType);
    }

    [Theory]
    [InlineData("document.pdf", "application/pdf")]
    [InlineData("report.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("data.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("notes.txt", "text/plain")]
    [InlineData("legacy.doc", "application/msword")]
    [InlineData("spreadsheet.xls", "application/vnd.ms-excel")]
    public void DetectFromExtension_KnownDocumentExtension_ReturnsCorrectMimeType(string fileName, string expectedMimeType)
    {
        var result = MimeTypeDetector.DetectFromExtension(fileName);

        result.Should().Be(expectedMimeType);
    }

    [Theory]
    [InlineData("song.mp3", "audio/mpeg")]
    [InlineData("audio.ogg", "audio/ogg")]
    [InlineData("voice.oga", "audio/ogg")]
    [InlineData("music.m4a", "audio/mp4")]
    [InlineData("track.aac", "audio/aac")]
    [InlineData("recording.opus", "audio/ogg")]
    public void DetectFromExtension_KnownAudioExtension_ReturnsCorrectMimeType(string fileName, string expectedMimeType)
    {
        var result = MimeTypeDetector.DetectFromExtension(fileName);

        result.Should().Be(expectedMimeType);
    }

    [Theory]
    [InlineData("video.mp4", "video/mp4")]
    [InlineData("clip.3gp", "video/3gpp")]
    [InlineData("movie.3gpp", "video/3gpp")]
    public void DetectFromExtension_KnownVideoExtension_ReturnsCorrectMimeType(string fileName, string expectedMimeType)
    {
        var result = MimeTypeDetector.DetectFromExtension(fileName);

        result.Should().Be(expectedMimeType);
    }

    [Theory]
    [InlineData("file.xyz")]
    [InlineData("unknown.abc123")]
    [InlineData("weird.extension")]
    [InlineData("bitmap.bmp")]
    [InlineData("vector.svg")]
    [InlineData("sound.wav")]
    [InlineData("clip.webm")]
    [InlineData("movie.avi")]
    [InlineData("recording.mov")]
    [InlineData("video.mkv")]
    public void DetectFromExtension_UnknownExtension_ReturnsOctetStream(string fileName)
    {
        var result = MimeTypeDetector.DetectFromExtension(fileName);

        result.Should().Be("application/octet-stream");
    }

    [Fact]
    public void DetectFromExtension_NoExtension_ReturnsOctetStream()
    {
        var result = MimeTypeDetector.DetectFromExtension("fileWithoutExtension");

        result.Should().Be("application/octet-stream");
    }

    [Theory]
    [InlineData("IMAGE.JPG", "image/jpeg")]
    [InlineData("DOCUMENT.PDF", "application/pdf")]
    [InlineData("File.PNG", "image/png")]
    [InlineData("MiXeD.GiF", "image/gif")]
    public void DetectFromExtension_CaseInsensitive_ReturnsCorrectMimeType(string fileName, string expectedMimeType)
    {
        var result = MimeTypeDetector.DetectFromExtension(fileName);

        result.Should().Be(expectedMimeType);
    }

    [Theory]
    [InlineData(@"C:\path\to\file.jpg", "image/jpeg")]
    [InlineData(@"/unix/path/document.pdf", "application/pdf")]
    [InlineData(@"relative\path\image.png", "image/png")]
    public void DetectFromExtension_FullPath_ReturnsCorrectMimeType(string filePath, string expectedMimeType)
    {
        var result = MimeTypeDetector.DetectFromExtension(filePath);

        result.Should().Be(expectedMimeType);
    }

    [Fact]
    public void DetectFromExtension_EmptyString_ReturnsOctetStream()
    {
        var result = MimeTypeDetector.DetectFromExtension(string.Empty);

        result.Should().Be("application/octet-stream");
    }

    [Fact]
    public void DetectFromExtension_DotOnly_ReturnsOctetStream()
    {
        var result = MimeTypeDetector.DetectFromExtension(".");

        result.Should().Be("application/octet-stream");
    }
}
