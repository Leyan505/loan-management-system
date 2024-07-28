namespace PrestamosCreciendo.Models
{
    public class SummaryDTO
    {
        public List<SummaryClients>? clients {  get; set; }
        public Users? user {  get; set; }
        public SummaryCredit? credit_data { get; set; }
        public List<Credit>? other_credit { get; set; }
        public float recent { get; set; }
        public float rest { get; set; }
        public string? show {  get; set; }
        public ErrorViewModel error { get; set; }
        

    }
}
