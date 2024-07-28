namespace PrestamosCreciendo.Models
{
    public class TrackerCreditDTO
    {
        public int credit_id {  get; set; }
        public int user_id { get; set; }
        public string? user_name { get; set; }
        public string? user_lastName { get; set; }
        public string? user_province { get; set; }
        public DateTime? credit_created_at { get; set; }
        public float credit_utility {  get; set; }
        public int credit_paymentNumber { get; set; }
        public float credit_amountNeto { get; set; }
    }
}
