namespace APIContracts;

public class CreateCommentDto
{
    public required string Body { get; set; }
    public required int PostId { get; set; }
    public required int UserId { get; set; }
    public required string Author { get; set; }
}