namespace Assignment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserPostController : ODataController
{
    private readonly UserPostService _userPostService;

    public UserPostController(UserPostService userPostService)
    {
        _userPostService = userPostService;
    }

    /// <summary>
    /// Gets users and associated posts
    /// </summary>
    /// <typeparam name="User"></typeparam>
    /// <returns></returns>
    [HttpGet]
    [EnableQuery]
    [Route("/letter")]
    public async Task<ActionResult<List<UserPostsResponse>>> GetUserPostsAsync()
    {
        var result = await _userPostService.GetUserPostsAsync();

        return result;
    }
}
