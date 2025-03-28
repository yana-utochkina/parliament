using ParliamentDomain.Model;

namespace ParliamentInfrastructure.ViewModels
{
    public class ProfileViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string University { get; set; }
        public string Faculty { get; set; }

        public ProfileViewModel(User user)
        {
            Id = user.Id;
            Email = user.Email;
            FullName = user.FullName;
            University = user.University;
            Faculty = user.Faculty;
        }
    }
}
