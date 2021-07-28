using System;

namespace api.DTOs
{

    // Dtos não tem outros modelos, apena so id deles
    public class MessageDto
    {
        public int Id { get; set; }

        // Sender
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }
        public string SenderKnownAs { get; set; }

        // Recipient
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string RecipientKnownAs { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}