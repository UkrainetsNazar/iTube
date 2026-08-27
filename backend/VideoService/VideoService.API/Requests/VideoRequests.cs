namespace VideoService.API.Requests;

public sealed class UploadVideoRequest
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public List<string> Tags { get; set; } = [];
}

public sealed class PublishVideoRequest
{
    public string Visibility { get; set; } = "Public";
}

public sealed class ReactToVideoRequest
{
    public string Type { get; set; } = null!;
}

public sealed class AddCommentRequest
{
    public string Text { get; set; } = null!;
}