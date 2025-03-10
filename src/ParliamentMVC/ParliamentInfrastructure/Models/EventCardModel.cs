namespace ParliamentInfrastructure.Models
{
    public class EventCardModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string AccessType { get; set; }
        public DateTime StartTime { get; set; }

        public EventCardModel(int Id, string Title, string Department, string AccessType, DateTime StartTime)
        {
            this.Id = Id;
            this.Title = Title;
            this.Department = Department;
            this.AccessType = AccessType;
            this.StartTime = StartTime;
        }
    }
}
