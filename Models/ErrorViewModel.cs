using System.Security.Policy;

namespace PrestamosCreciendo.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? description { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public bool ShowDescription => !string.IsNullOrEmpty(description);
    }
}
