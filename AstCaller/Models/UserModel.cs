using System.Security.Principal;

namespace AstCaller.Models
{
    public class UserModel : IIdentity
    {
        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Name { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Fullname { get; set; }

        public int Id { get; set; }
    }
}
