using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using WeatherApi.Services;
using WeatherDataAccess;

namespace WeatherApi.Controllers
{
    public class LoginController : ApiController
    {
        [BasicAuthentication]
        public IEnumerable<UserLogin> Get()
        {
            using (DBweatherEntities _ctx = new DBweatherEntities())
            {
                return _ctx.UserLogins.ToList();
            }
        }
        [BasicAuthentication]
        public HttpResponseMessage Get(int id)
        {
            var login = Thread.CurrentPrincipal.Identity.Name;
            using (DBweatherEntities _ctx = new DBweatherEntities())
            {
                var user= _ctx.UserLogins.Where(x=>x.login==login).FirstOrDefault();
                if (user != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Brak uzytkownika o nr " + login);
                }
            }
        }

        public HttpResponseMessage Post([FromBody] UserLogin user)
        {
            try
            {
                using (DBweatherEntities _ctx = new DBweatherEntities())
                {
                    if (!_ctx.UserLogins.Where(x => x.email.ToUpper() == user.email.ToUpper() || x.login==user.login).Any())
                    {
                        _ctx.UserLogins.Add(user);
                        _ctx.SaveChanges();
                        var msg = Request.CreateResponse(HttpStatusCode.Created,user);
                        msg.Headers.Location = new Uri(Request.RequestUri +"/"+ user.id.ToString());
                        return msg;
                    }
                    var msgbad=Request.CreateResponse(HttpStatusCode.NotAcceptable, "Jest juz uzytkownik z taki e-mail");
                     
                    return msgbad;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,ex);
            }
        }

        [BasicAuthentication]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                using (DBweatherEntities _ctx = new DBweatherEntities())
                {
                    var user = _ctx.UserLogins.Where(x => x.id == id).FirstOrDefault();
                    if (user != null)
                    {
                        _ctx.UserLogins.Remove(user);
                        _ctx.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, user);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Brak uzytkownika o nr " + id);
                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [BasicAuthentication]
        public HttpResponseMessage Put(int id, [FromBody] UserLogin user)
        {
            try
            {
                using (DBweatherEntities _ctx = new DBweatherEntities())
                {
                    var tmpuser = _ctx.UserLogins.Where(x => x.id == id).FirstOrDefault();
                    if (user != null)
                    {

                        tmpuser.email = user.email;
                        tmpuser.isAdmin = user.isAdmin;
                        tmpuser.login = user.login;
                        tmpuser.password = user.password;
                        _ctx.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, user);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Brak uzytkownika o nr " + id);
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
