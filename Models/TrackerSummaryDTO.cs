namespace PrestamosCreciendo.Models
{
    public class TrackerSummaryDTO
    {
        public string? user_name { get; set; }
        public string? users_lastName { get; set; }
        public int payment_number { get; set; }
        public float amount_neto { get; set; }
        public int id_credit { get; set; }
        public int number_index { get; set; }
        public float amount {  get; set; }
        public DateTime? created_at { get; set; }

    }
}
