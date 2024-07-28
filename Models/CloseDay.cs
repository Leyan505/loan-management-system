namespace PrestamosCreciendo.Models
{
    public class CloseDay
    {
        public int Id { get; set; }
        public int Id_agent { get; set; }
        public int Id_supervisor { get; set; }
        public DateTime Created_at { get; set; }
        public float Total {  get; set; }
        public float Base_before { get; set; }
        public float From_number { get; set; }
    }
}
