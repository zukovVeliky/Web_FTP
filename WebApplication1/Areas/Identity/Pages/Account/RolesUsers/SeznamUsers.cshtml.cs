using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account.RolesUsers
{
    public class SeznamUsersModel : PageModel
    {

        private DB_Context.IdentityContext _DB;

        public List<DB_Context.AspNetUser>  Uzivatele = new List<DB_Context.AspNetUser>();


        public SeznamUsersModel(DB_Context.IdentityContext context)
        {


            _DB = context;

            var Users = from k in  _DB.AspNetUsers  select k;


            this.Uzivatele = Users.ToList();

            int? pageIndex = null;
            int px = 1;
            /*
            if (int.TryParse(Request.Query["pageIndex"], out px))
            {
                pageIndex = int.Parse(Request.Query["pageIndex"]);
            }
            */












        }


        public void OnGet()
        {

        }

        public string CodeIdUser(string id ) {return CodeDecode.Base64Encode(id); }


    }
}
