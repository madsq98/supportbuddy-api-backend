using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace SB.WebAPI.Utilities
{
    public class BasicAuthorizationReader
    {
        private const string StartOfHeader = "Basic";
        private const char Separator = ':';

        private string _username = "";
        private string _password = "";

        private void ReadAuthorizationHeader(HttpContext ctx)
        {
            string authorizationHeader = ctx.Request.Headers["Authorization"];

            if (authorizationHeader != null && authorizationHeader.StartsWith(StartOfHeader))
            {
                var base64EncodedUsernameAndPassword = authorizationHeader.Substring(StartOfHeader.Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");

                var decodedUsernameAndPassword =
                    encoding.GetString(Convert.FromBase64String(base64EncodedUsernameAndPassword));

                var separatorIndex = decodedUsernameAndPassword.IndexOf(Separator);

                _username = decodedUsernameAndPassword.Substring(0, separatorIndex);
                _password = decodedUsernameAndPassword.Substring(separatorIndex + 1);
            }

            //throw new InvalidDataException("Authorization header not found or not Basic Authorization");
        }

        public string GetUsername(HttpContext ctx)
        {
            ReadAuthorizationHeader(ctx);
            return _username;
        }

        public string GetPassword(HttpContext ctx)
        {
            ReadAuthorizationHeader(ctx);
            return _password;
        }
    }
}