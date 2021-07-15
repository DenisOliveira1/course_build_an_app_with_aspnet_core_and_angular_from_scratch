namespace api.Helpers
{
    public class LikesParams : PaginationParams
    {
        public int userId { get; set; }
        public string predicate { get; set; }
    }
}