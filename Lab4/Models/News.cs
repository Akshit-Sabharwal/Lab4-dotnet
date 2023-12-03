using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Lab4.Models
{
	public class News
	{
        public int NewsId { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "File Name")]
        public string FileName { get; set; }


        [Required]
        [Display(Name = "URL")]
        public string Url { get; set; }

        public string SportsClubId {  get; set; }

        public SportClub SportClub { get; set; }
    }
}