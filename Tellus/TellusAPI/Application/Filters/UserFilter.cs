namespace TellusAPI.Application.Filters
{
    public class UserFilter
    {
        public string? Name { get; set; }         // Filtro por nome (contains)
        public string? Email { get; set; }        // Filtro por email (contains)
        public int? ProfileId { get; set; }       // Filtro por ProfileReference.Id
        public int Page { get; set; } = 1;        // Página 1-based
        public int PageSize { get; set; } = 20;   // Itens por página
    }
}
