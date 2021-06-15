namespace Identity.Api.Dtos.Account.Registration
{
    public class RegisterDto
    {
        public string Id { get; set; }
        public bool EmailConfirmed { get; set; }
        public string SecurityToken { get; set; }
    }
}
