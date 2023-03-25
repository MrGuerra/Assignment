namespace Assignment.Responses;

public class UserPostsResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Company { get; set; }
    public List<PostResponse> Posts { get; set; }
}

public class PostResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}
