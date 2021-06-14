namespace Identity.Api.ViewModels.Account.Registration
{
    public class RegisterVm
    {
        public string Id { get; set; }
        public bool EmailConfirmed { get; set; }
        public string SecurityToken { get; set; }
    }
}
