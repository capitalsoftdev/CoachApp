using System.ComponentModel.DataAnnotations;

namespace CoachApp.Models
{
    public class Participant
    {
        public string Id { get; set; }
        public string TestId { get; set; }
        public Test Test { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        [Range(0, int.MaxValue)]
        public int DistanceResult { get; set; }
        [Range(0, double.MaxValue)]
        public double TimeResult { get; set; }
    }
}
