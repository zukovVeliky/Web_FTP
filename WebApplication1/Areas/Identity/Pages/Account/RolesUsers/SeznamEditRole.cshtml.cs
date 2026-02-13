using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account.RolesUsers
{
    public class SeznamEditRoleModel : PageModel
    {
        private DB_Context.IdentityContext _DB;
        public List<DB_Context.AspNetRole> Roles = new List<DB_Context.AspNetRole>();

        public SeznamEditRoleModel(Identity.DB_Context.IdentityContext context)
        {
            _DB = context;
        }
        public async Task OnGetAsync()
        {
            string deleteID = Request.Query["del"];
            /// odstraneni oddilu ////
            if (deleteID != null)
            {
                var OdstraneniRole = _DB.AspNetRoles.Find(deleteID);
                if (OdstraneniRole != null)
                {
                    /// odstraneni parametru  DEL z URL
                    string URLnew = Request.QueryString.ToString();
                    URLnew = URLnew.Replace("del=" + deleteID, "");
                    URLnew = URLnew.Replace("&&", "&");
                    URLnew = Request.Scheme + "://" + Request.Host + Request.Path + URLnew;
                    if (URLnew[URLnew.Length - 1] == '&') { URLnew = URLnew.Remove(URLnew.Length - 1); }
                    char c = URLnew[URLnew.Length - 1];
                    if (URLnew[URLnew.Length - 1] == '?') { URLnew = URLnew.Remove(URLnew.Length - 1); }
                    ///

                    Code.Role DelRole = new Code.Role(this._DB);
                    await DelRole.DeleteRoleAsync(deleteID);

                    //_DB.AspNetRoles.Remove(OdstraneniRole);
                    //await _DB.SaveChangesAsync();

                    Response.Redirect(URLnew);

                }
            }
            /// 





            this.Inicializace();
        }


        public void OnPostCreateRole()
        {
            string Name  = Request.Form["TextRole"].ToString();
            Code.Role CR = new Code.Role(this._DB);

            CR.CreateRole(Name);
            this.Inicializace();

            /*

            */
        }

        private void Inicializace()
        {
            var Users = from k in _DB.AspNetUsers select k;

            var Role = from k in _DB.AspNetRoles select k;
            this.Roles = Role.ToList();
        }

    }
}
