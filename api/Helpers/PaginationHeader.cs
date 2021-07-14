namespace api.Helpers
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set;} // Página atual
        public int TotalPages { get; set;} // Total de páginas
        public int ItemsPerPage { get; set;} // Itens por pagina
        public int TotalItems { get; set;} // Total de itens
        public PaginationHeader(int currentPage, int totalPages, int itemsPerPage, int totalItems)
        {
            CurrentPage = currentPage;
            TotalPages = totalPages;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
        }
    }
}