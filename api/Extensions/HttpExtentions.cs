using System.Text.Json;
using api.Helpers;
using Microsoft.AspNetCore.Http;

namespace api.Extensions
{
    public static class HttpExtentions
    {
        public static void AddPaginationHeader(this HttpResponse response,int currentPage, int totalPages, int itemsPerPage, int totalItems){

            var paginatioinHeader = new PaginationHeader(currentPage, totalPages, itemsPerPage, totalItems);

            var options = new JsonSerializerOptions{
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginatioinHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");

        }
    }
}