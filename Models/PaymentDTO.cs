namespace PrestamosCreciendo.Models
{
    public class PaymentDTO
    {
        public int id_user {  get; set; }
        public string? name { get; set; }
        public string? last_name { get; set; }
        public int credit_id { get; set; }
        public float amount_neto { get; set; }
        public float positive {  get; set; }
        public float rest { get; set; }
        public int payment_current { get; set; }
        public int payment_number { get; set; }
        public int payment_done { get; set; }
        public float payment_quote { get; set; }
        public float utility { get; set; }
        public bool rev {  get; set; }
    }
}
