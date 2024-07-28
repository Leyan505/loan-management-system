namespace PrestamosCreciendo.Models
{
    public class TrackerDTO
    {
        public List<TrackerSummaryDTO> Summary { get; set; }
        public List<TrackerCreditDTO> Credit { get; set; }
        public List<Bills> Bills { get; set; }
        public float total_summary { get; set; }
        public float total_credit { get; set; }
    }
}
