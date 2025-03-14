namespace ParliamentInfrastructure.Models
{
    public class NewsCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public DateTime PublicationDate { get; set; }

        public NewsCardViewModel(int Id, string Title,  string Department, DateTime PublicationDate)
        {
            this.Id = Id;
            this.Title = Title;
            this.Department = Department;
            this.PublicationDate = PublicationDate;
        }
    }
}
