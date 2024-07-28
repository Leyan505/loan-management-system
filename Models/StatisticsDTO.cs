using Microsoft.EntityFrameworkCore.Storage;

namespace PrestamosCreciendo.Models
{
    public class StatisticsDTO
    {
        public float summary { get; set; }
        public float credit { get; set; }
        public float bills { get; set; }
        public int days { get; set; }
        public Wallet wallet { get; set; }
        public string range { get; set; }
    }
}
