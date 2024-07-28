namespace PrestamosCreciendo.Models
{
    public class HistoryAgentDTO
    {
        public float Base {  get; set; }
        public float today_amount { get; set; }
        public float today_sell {  get; set; }
        public float bills { get; set; }
        public float total {  get; set; }
        public float average { get; set; }
        public Users? user { get; set; }
        public ErrorViewModel? error { get; set; }
    }
}
