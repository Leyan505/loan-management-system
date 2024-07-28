namespace PrestamosCreciendo.Models
{
    public class SummaryCredit
    {
        public int Id { get; set; }
        public float Amount_neto { get; set; }
        public int Order_list { get; set; }
        public int Id_user { get; set; }
        public int Id_agent { get; set; }
        public int Payment_number { get; set; }
        public float Utility { get; set; }
        public string Status { get; set; } = "inprogress";
        public DateTime Created_at { get; set; }
        public float utility_amount { get; set; }
        public float payment_amount { get; set; }
        public float total {  get; set; }
    }
}
