namespace AIChat1.IService
{
    public interface IJwtIssuer
    {
        string Issue(int userId, string username, IEnumerable<string>? roles = null);
    }
}
