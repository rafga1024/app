using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WeatherApi.Services
{
    public class BasicAuthenticationAttribute: AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
            }
            else
            {
                string authToken = actionContext.Request.Headers.Authorization.Parameter;
                string decodeAuthToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                string[] loginpassword = decodeAuthToken.Split(':');
                string login = loginpassword[0];
                string password = loginpassword[1];
                if (UserSecurity.Login(login, password) && UserSecurity.IsAdmin(login, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(login), new string[] { "admin" });
                }
                else if (UserSecurity.Login(login, password) && !UserSecurity.IsAdmin(login, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(login), new string[] { "user" });
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}