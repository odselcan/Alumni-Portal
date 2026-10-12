namespace Alumni_Portal.ViewModels
{
    public class BirthdayViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int DaysLeft { get; set; }
    }
}