using CKEditorB.DB_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Pages;

[IgnoreAntiforgeryToken]
public class InlineModel : PageModel
{
    private readonly AdamekContext _db;

    public InlineModel(AdamekContext db)
    {
        _db = db;
    }

    public List<Content> Obsah { get; private set; } = new();

    public async Task OnGetAsync()
    {
        Obsah = await _db.Contents.AsNoTracking().OrderBy(x => x.Id).ToListAsync();

        if (Obsah.Count == 0)
        {
            var first = new Content
            {
                Html = "<h2>Ukazka CKEditor Inline</h2><p>Toto je editovatelny obsah.</p>"
            };
            _db.Contents.Add(first);
            await _db.SaveChangesAsync();
            Obsah.Add(first);
        }
    }

    public async Task<IActionResult> OnPostContentAsync([FromBody] CKEditor5.JsonContent request)
    {
        if (request == null)
        {
            return BadRequest();
        }

        var content = await _db.Contents.FirstOrDefaultAsync(x => x.Id == request.id);
        if (content == null)
        {
            content = new Content { Id = request.id };
            _db.Contents.Add(content);
        }

        content.Html = request.html ?? string.Empty;
        await _db.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }
}
