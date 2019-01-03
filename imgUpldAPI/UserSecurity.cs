using imgUpldAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace imgUpldAPI
{
    public class UserSecurity
    {

        public static bool Login(string username, string password)
        {
            using (WebAPIEntities entities = new WebAPIEntities())
            {
                return entities.Users.Any(user => user.Username.Equals(username, 
                    StringComparison.OrdinalIgnoreCase) && user.Password == password);
                
            }
        }
    }
}