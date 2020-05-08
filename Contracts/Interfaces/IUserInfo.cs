namespace Contracts.Interfaces
{
    public interface IUserInfo
    {
        string Email { get; }
        int AdultLevel { get; }
        bool IsSupervisor { get; }
    }
}
