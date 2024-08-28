namespace DomainCore.Interfaces
{
    public interface ISecretRepository
    {
        Task<string> GetSecret();
    }
}
