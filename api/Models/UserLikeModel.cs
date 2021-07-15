namespace api.Models
{
    public class UserLikeModel
    {
        public UserModel SourceUser { get; set; }
        public int SourceUserId { get; set; }
        
        public UserModel LikedUser { get; set; }
        public int LikedUserId { get; set; }

    }
}