using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CKEditor5
{
    [IgnoreAntiforgeryToken]
    public class CKEditor5InlineModel : PageModel
    {

        public string SetPath { get; private set; } = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Path.Combine("UzivatelskeSoubory")));

        public CKEditor5InlineModel() 
        { 
 
        }


        public void OnGet()
        {
        }

    }

    public class JsonContent
    {
        public string html { get; set; }
        public int id { get; set; }
    }
}
