namespace ParliamentInfrastructure.Models
{
    public class DepartmentCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DepartmentCardViewModel(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }
}
