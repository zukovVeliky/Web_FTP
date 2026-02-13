using Microsoft.EntityFrameworkCore;

namespace CKEditorB.DB_Context;

public class AdamekContext : DbContext
{
    public AdamekContext(DbContextOptions<AdamekContext> options)
        : base(options)
    {
    }

    public DbSet<Content> Contents => Set<Content>();
}
