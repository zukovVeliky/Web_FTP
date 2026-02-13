using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account.RolesUsers
{
    public class UserEditRoleModel : PageModel
    {
        public string CIP { get; set; }
        private DB_Context.IdentityContext _DB;
        public List<DB_Context.AspNetRole> Roles = new List<DB_Context.AspNetRole>();
        public DB_Context.AspNetUser Uzivatel = new DB_Context.AspNetUser();
        public UserEditRoleModel(DB_Context.IdentityContext context)
        {
            this._DB = context;
            var Role = from k in _DB.AspNetRoles select k;
            this.Roles = Role.ToList();
        }
        public void OnGet(string cip)
        {
            if (cip != "" && cip != null)
            {
                this.CIP = cip;
                Uzivatel = _DB.AspNetUsers.Find(CodeDecode.Base64Decode(cip));
            }

        }

        /// <summary>
        /// aktivace uzivatele v roli
        /// </summary>
        /// <param name="id"></param>
        public void OnGetA (string id,string cip)
        {
            this.CIP = cip;
            this.Uzivatel = this._DB.AspNetUsers.Find(CodeDecode.Base64Decode(cip));

            Code.Role R = new Code.Role(this._DB);
            R.AddUser_in_Role(this.Uzivatel.Id, CodeDecode.Base64Decode(id));

        }
        /// <summary>
        /// deaktivace uzivatele v roli
        /// </summary>
        /// <param name="id"></param>
        public void OnGetD(string id, string cip)
        {
            this.CIP = cip;
            this.Uzivatel = this._DB.AspNetUsers.Find(CodeDecode.Base64Decode(cip));

            Code.Role R = new Code.Role(this._DB);
            R.Remove_User_in_Role(this.Uzivatel.Id, CodeDecode.Base64Decode(id));

        }

        public string Zakodovani(string arg)
        {
            return CodeDecode.Base64Encode(arg);
        }





        public bool UserInRole(string role)
        {
            var UR = from k in _DB.AspNetUserRoles where(k.RoleId == role && k.UserId == this.Uzivatel.Id ) select k;

            var vystup = UR.ToList();
          if(vystup.Count > 0) { return true; }
          return false;
        }
    }
}
