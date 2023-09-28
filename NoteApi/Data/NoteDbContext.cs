using Microsoft.EntityFrameworkCore;
using NoteApi.Data.Entities;

namespace NoteApi.Data
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options)
        {

        }

        public DbSet<Note> Notes { get; set; }
    }
}
