using ParliamentDomain.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParliamentInfrastructure.ViewModels;

public class ProfileViewModel
{
    public int Id { get; set; }

    [DisplayName("Електронна пошта")]
    public string Email { get; set; }
    [DisplayName("Прізвище Ім'я")]
    public string FullName { get; set; }
    [DisplayName("Університет")]
    public string University { get; set; }
    [DisplayName("Факультет")]
    public string Faculty { get; set; }

    public ProfileViewModel() { }

    public ProfileViewModel(int Id, string Email, string FullName, string University, string Faculty)
    {
        this.Id = Id;
        this.Email = Email;
        this.FullName = FullName;
        this.University = University;
        this.Faculty = Faculty;
    }

    public ProfileViewModel(User user)
    {
        Id = user.Id;
        Email = user.Email;
        FullName = user.FullName;
        University = user.University;
        Faculty = user.Faculty;
    }
}
