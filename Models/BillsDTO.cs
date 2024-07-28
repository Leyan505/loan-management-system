namespace PrestamosCreciendo.Models
{
    public class BillsDTO
    {
        public Bills bill { get; set; }
        public string wallet_name { get; set; }
        public string category_name { get; set; }
        public string user_name { get; set; }
        public List<BillsDTO> clients { get; set; }
        public float total {  get; set; }
        public List<ListBill> list_categories { get; set; }
    }
}
