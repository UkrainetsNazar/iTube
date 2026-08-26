namespace MediaService.API.Requests;

public sealed class UploadMediaRequest
{
    public IFormFile File { get; set; } = null!;
    public string MediaType { get; set; } = null!;
    public Guid? VideoId { get; set; }
    public Guid? ChannelId { get; set; }
}