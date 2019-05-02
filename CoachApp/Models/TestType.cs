using System.ComponentModel.DataAnnotations;

namespace CoachApp.Models
{
    public enum TestType
    {
        [Display(Name = "Cooper Test")]
        CooperTest,
        [Display(Name = "Sprint Test")]
        SprintTest
    }
}
