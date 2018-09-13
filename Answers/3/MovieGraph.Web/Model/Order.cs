using System.ComponentModel.DataAnnotations;

namespace MovieGraph.Web.Model
{
    public enum Order
    {
        [Display(Name = "most recent first")] MostRecentFirst,
        [Display(Name = "most popular first")] MostPopularFirst,
        [Display(Name = "alphabetically")] Alphabetically
    }
}