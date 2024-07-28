namespace PrestamosCreciendo.Models
{
    public class NewPaymentRequest
    {
        public int credit_id {  get; set; }
        public float amount { get; set; }
        public string format { get; set; }
        public bool rev { get; set; }
    }
}
