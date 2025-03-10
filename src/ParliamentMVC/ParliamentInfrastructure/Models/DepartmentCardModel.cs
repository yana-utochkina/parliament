namespace ParliamentInfrastructure.Models
{
    public class DepartmentCardModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DepartmentCardModel(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }
}
