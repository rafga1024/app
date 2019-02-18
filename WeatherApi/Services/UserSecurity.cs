using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeatherDataAccess;

namespace WeatherApi.Services
{
    public class UserSecurity
    {
        public static bool Login(string login, string password)
        {
            using (DBweatherEntities _ctx = new DBweatherEntities())
            {
                return _ctx.UserLogins.Any(u => u.login == login && u.password == password);
            }
        }
        public static bool IsAdmin(string login, string password)
        {
            using (DBweatherEntities _ctx = new DBweatherEntities())
            {
                return _ctx.UserLogins.Any(u => u.login == login && u.password == password && u.isAdmin==true);
            }
        }
    }
}