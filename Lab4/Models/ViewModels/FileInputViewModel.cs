namespace Lab4.Models.ViewModels
{

    public class FileInputViewModel
    {
        public string SportClubId { get; set; }

        public string SportClubTitle { get; set; }
        public IFormFile File { get; set; }
    }

}