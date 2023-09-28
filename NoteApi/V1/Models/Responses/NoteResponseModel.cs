using System.ComponentModel.DataAnnotations;

namespace NoteApi.V1.Models.Responses
{
    public class NoteResponseModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
