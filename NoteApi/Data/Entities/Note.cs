using System.ComponentModel.DataAnnotations;

namespace NoteApi.Data.Entities
{
    public class Note
    {
        [Key]
        public int Id { get; set; }

        [Required()]
        public int UserId { get; set; }

        [Required()]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
