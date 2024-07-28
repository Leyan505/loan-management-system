namespace PrestamosCreciendo.Models
{
    public class DataRouteDTO
    {
        public Users user {  get; set; }
        public int Id { get; set; }
        public float amount_total { get; set; }
        public int days_rest { get; set; }
        public float saldo {  get; set; }
        public float quote {  get; set; }
        public Summary? last_pay { get; set; }
        public string? request {  get; set; }
        public int order_list {  get; set; }
    }
}
