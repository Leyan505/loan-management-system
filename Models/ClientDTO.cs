namespace PrestamosCreciendo.Models
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string? Nit {  get; set; }
        public string? address { get; set; }
        public int? phone { get; set; }
        public string? name { get; set; }
        public string? last_name { get; set; }
        public string? province { get; set; }
        public int credit_count {  get; set; }
        public int closed {  get; set; }
        public int inprogress { get; set; }
        public Credit? amount_net { get; set; }
        public float gap_credit { get; set; }
        public float summary_net { get; set; }
        public string status { get; set; } = "good";
        public string? lat {  get; set; }
        public string? lng { get; set; }
        public float total_credit { get; set; }
        
    }
}
