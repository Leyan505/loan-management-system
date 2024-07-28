﻿namespace PrestamosCreciendo.Models
{
    public class Summary
    {
        public int Id { get; set; }
        public float Amount { get; set; }
        public int Id_agent { get; set; }
        public int Id_credit { get; set; }
        public int Number_index { get; set; }
        public DateTime Created_at { get; set; } = DateTime.UtcNow;
    }
}