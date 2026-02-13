using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.DB_Context;

namespace Code
{
    public class Role
    {
        private IdentityContext _DB;

        private  List<AspNetRole> Roles = new List<AspNetRole>();

        public Role(IdentityContext DB)
        {
            _DB = DB;
            var xxx = Roles;
        }


        public bool ExistRole (string Name)
        {
            List<AspNetRole> Rol = new List<AspNetRole>();
            var ex = from k in _DB.AspNetRoles where(k.Name == Name) select k;
            Rol = ex.ToList();
            if(Rol.Count >0)
            {
                return true;
            }
            return false;
        }

        public async Task DeleteRoleAsync (string id)
        {
            /// delete zaznamy users in role

            var userRoles = from k in _DB.AspNetUserRoles where (k.RoleId == id) select k;
            foreach (var UR in userRoles.ToList())
            {
                _DB.AspNetUserRoles.Remove(UR);
            }
            /////////////////////////////////



            _DB.AspNetRoles.Remove(_DB.AspNetRoles.Find(id));
            await _DB.SaveChangesAsync();
        }
        public void CreateRole (string Name)
        {


            if (!this.ExistRole(Name))
            {
                IdentityRole IR = new IdentityRole(Name);

                var xxx = new AspNetRole();
                xxx.Name = IR.Name;
                xxx.Id = IR.Id;
                xxx.NormalizedName = IR.NormalizedName;
                xxx.ConcurrencyStamp = IR.ConcurrencyStamp;
                _DB.AspNetRoles.Add(xxx);
                _DB.SaveChanges();
            }
        }

        public bool Is_User_in_Role(string id_User, string id_Role)
        {

            AspNetUserRole UR = _DB.AspNetUserRoles.Find(id_User, id_Role);
            
            if (UR == null)
            {
                UR = null;
                return false;
            }
            return true;
            
            
        }

        public void AddUser_in_Role(string id_User, string id_Role)
        {
            if (!this.Is_User_in_Role(id_User, id_Role))
            {
                AspNetUserRole UR = new AspNetUserRole();
                UR.Role = this._DB.AspNetRoles.Find(id_Role);
                UR.User = this._DB.AspNetUsers.Find(id_User);
                UR.RoleId = UR.Role.Id;
                UR.UserId = UR.User.Id;

                _DB.AspNetUserRoles.Add(UR);
                _DB.SaveChanges();
            }
        }

        public void Remove_User_in_Role(string id_User, string id_Role)
        {
            AspNetUserRole UR = _DB.AspNetUserRoles.Find(id_User, id_Role);
            if (UR != null)
            {
                _DB.AspNetUserRoles.Remove(UR);
                _DB.SaveChanges();
            }
        }


    }




}

