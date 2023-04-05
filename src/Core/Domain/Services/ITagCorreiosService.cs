namespace Domain.Services;

public interface ITagCorreiosService
{
    Task<(string sro, string contract)> GetSro(string method);
}