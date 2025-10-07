namespace TellusAPI.Domain.Entities
{
    public class Function
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool IsActive { get; set; }
    }
}
