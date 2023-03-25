namespace Assignment.Services;

public class UserPostService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly string UsersKey = "users";
    private readonly string PostsKey = "posts";
    private readonly TimeSpan CacheExpiration;

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public UserPostService(HttpClient httpClient, IMemoryCache cache, IConfiguration config, ILogger<UserPostService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _configuration = config;
        _logger = logger;

        CacheExpiration = new TimeSpan(_configuration.GetValue<int>("CacheExpiration:Hours"),
                                       _configuration.GetValue<int>("CacheExpiration:Minutes"),
                                       _configuration.GetValue<int>("CacheExpiration:Seconds"));
    }

    public async Task<List<UserPostsResponse>> GetUserPostsAsync()
    {
        var usersCache = new List<User>();
        var postsCache = new List<Post>();

        if (_cache.TryGetValue<List<User>>(UsersKey, out var users))
        {
            usersCache = _cache.Get<List<User>>(UsersKey);
        }
        else
        {
            usersCache = await GetUsersAsync();
            _cache.Set(UsersKey, usersCache, CacheExpiration);
        }

        if (_cache.TryGetValue<List<Post>>(PostsKey, out var posts))
        {
            postsCache = _cache.Get<List<Post>>(PostsKey);
        }
        else
        {
            postsCache = await GetPostsAsync();
            _cache.Set(PostsKey, postsCache, CacheExpiration);
        }

        var usersPosts = from user in usersCache
                         join post in postsCache
                         on user.Id equals post.UserId into userPosts
                         select new UserPostsResponse
                         {
                             Id = user.Id,
                             Name = user.Name,
                             Username = user.Username,
                             Email = user.Email,
                             Address = string.Format("{0}, {1} - {2} {3}", user.Address.Street,
                                                                           user.Address.Suite,
                                                                           user.Address.Zipcode,
                                                                           user.Address.City),
                             Phone = user.Phone,
                             Website = user.Website,
                             Company = user.Company.Name,
                             Posts = userPosts.Select(x => new PostResponse() { Id = x.Id,
                                                                                Body = x.Body,
                                                                                Title = x.Title })
                                              .ToList()
                         };

        return usersPosts.ToList();
    }

    private async Task<List<User>> GetUsersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_configuration.GetValue<string>("Endpoint") + UsersKey);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<List<User>>(json, jsonOptions); ;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users");
            throw;
        }

    }

    private async Task<List<Post>> GetPostsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(_configuration.GetValue<string>("Endpoint") + PostsKey);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<List<Post>>(json, jsonOptions); ;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts");
            throw;
        }

    }
}
