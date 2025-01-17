using Moq;
using MyORM.interfaces;
using Persistence.Entities;
using Xunit;

namespace OrmTests;

public class OrmTests
{
    private readonly Mock<IQueryMapper> _mockMapper;

    public OrmTests()
    {
        _mockMapper = new Mock<IQueryMapper>();
    }
    
   public static IEnumerable<object[]> GetCrudQueryData()
    {
        
        FormattableString insertQuery = $"""
                                          INSERT INTO users (login, password, role, profile_name, avatar_path, country)
                                         VALUES ('user123', 'secure_password', 'user', 'John Doe', '/images/avatar123.png', 'USA')
                                         RETURNING login;
                                         """;
        
        yield return new object[] {insertQuery, "user123"};
        
        FormattableString selectQuery = $"""
                                         SELECT login 
                                         FROM users
                                         WHERE profile_name = 'John Doe';
                                         """;
        yield return new object[] {selectQuery, "user123"};
        
        FormattableString updateQuery = $"""
                                         UPDATE users
                                         SET profile_name = 'Jane Doe'
                                         WHERE login= 'user123'
                                         RETURNING profile_name;
                                         """;
        yield return new object[] {updateQuery, "Jane Doe"};
        
        FormattableString selectAggregateQuery = $"""
                                                  SELECT COUNT(*)
                                                  FROM users
                                                  WHERE login= 'user123';
                                                  """;
        yield return new object[] {selectAggregateQuery, 1};
        
        FormattableString deleteQuery = $"""
                                         DELETE from users
                                         WHERE login= 'user123'
                                         RETURNING profile_name;
                                         """;
        yield return new object[] {deleteQuery, "Jane Doe"};
    }
    
    public static IEnumerable<object[]> GetJoinQueryData()
    {
        FormattableString joinSqlQuery = $"""
                                      SELECT artwork_id, title, description, artwork_path, category,users.user_id, users.profile_name 
                                      FROM artworks
                                      INNER JOIN users ON artworks.user_id = users.user_id
                                      WHERE artworks.user_id = 1;
                                      """;

        var artwork = new UserArtwork
        {
            ArtworkId = 1,
            Title = "title",
            Description = "description",
            Category = "anime",
            ArtworkPath = "path",
            ProfileName = "name",
            UserId = 1,
        };

        yield return new object[] {joinSqlQuery, artwork};
    }
    
    public static IEnumerable<object[]> GetListQueryData()
    {
        FormattableString selectListSqlQuery = $"""
                                          SELECT * 
                                          FROM users
                                          """;
        var userList = new List<User>();
        
        var user1 = new User
        {
            UserId = 1,
            Login = "login",
            Password = "password",
            ProfileName = "name",
            Country = "Russia",
            AvatarPath = "path",
            Role = "user"
        };

        var user2 = new User
        {
            UserId = 2,
            Login = "login2",
            Password = "password",
            ProfileName = "name",
            Country = "Russia",
            AvatarPath = "path",
            Role = "user"
        };
        userList.Add(user1);
        userList.Add(user2);
        
        yield return new object[] {selectListSqlQuery, userList};
    }


    [Theory]
    [MemberData(nameof(GetCrudQueryData))]
    public async void QueryMapper_Should_ExecuteAndReturnAsync(FormattableString input, string expected)
    {
        _mockMapper
            .Setup(m => m.ExecuteAndReturnAsync<string>(It.IsAny<FormattableString>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var mapper = _mockMapper.Object;

        var ctx = new CancellationTokenSource();
        var result = await mapper.ExecuteAndReturnAsync<string>(input, ctx.Token);

        Assert.Equal(expected, result);

        _mockMapper.Verify(m => m.ExecuteAndReturnAsync<string>(input, ctx.Token), Times.Once);
    }
    
    [Theory]
    [MemberData(nameof(GetJoinQueryData))]
    public async void QueryMapper_Should_ExecuteAndReturnUserArtworkAsync(FormattableString input, UserArtwork expected)
    {
        _mockMapper
            .Setup(m => m.ExecuteAndReturnAsync<UserArtwork>(It.IsAny<FormattableString>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var mapper = _mockMapper.Object;

        var ctx = new CancellationTokenSource();
        var result = await mapper.ExecuteAndReturnAsync<UserArtwork>(input, ctx.Token);

        Assert.Equivalent(expected, result);

        _mockMapper.Verify(m => m.ExecuteAndReturnAsync<UserArtwork>(input, ctx.Token), Times.Once);
    }
    
    [Theory]
    [MemberData(nameof(GetListQueryData))]
    public async void QueryMapper_Should_ExecuteAndReturnUserListAsync(FormattableString input, List<User> expected)
    {
        _mockMapper
            .Setup(m => m.ExecuteAndReturnAsync<List<User>>(It.IsAny<FormattableString>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var mapper = _mockMapper.Object;

        var ctx = new CancellationTokenSource();
        var result = await mapper.ExecuteAndReturnAsync<List<User>>(input, ctx.Token);

        Assert.Equivalent(expected, result);

        _mockMapper.Verify(m => m.ExecuteAndReturnAsync<List<User>>(input, ctx.Token), Times.Once);
    }
}
