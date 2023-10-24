namespace Lab4.Models.ViewModels
{
    public class NewsViewModel
    {
        public IEnumerable<Fan> Fans { get; set; }
        public IEnumerable<SportClub> SportClubs { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
