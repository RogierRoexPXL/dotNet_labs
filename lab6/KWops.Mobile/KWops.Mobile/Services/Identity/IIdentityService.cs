namespace KWops.Mobile.Services.Identity
{
    public interface IIdentityService
    {
        Task<ILoginResult> LoginAsync();
    }
}
