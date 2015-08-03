﻿namespace NancyAspNetOwin.WebApp.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using JWT;
    using Nancy;
    using Nancy.ModelBinding;

    public class LoginModule : NancyModule
    {
        const string SecretKey = "30ea254132194990837862e7d9a644c1";

        public LoginModule(UserData userData)
        {
            Get["/login"] = o => View["Authentication/login"];

            Post["/login"] = p =>
            {
                var credentials = this.Bind<Credentials>();

                var user = userData.ValidateUser(credentials.UserName, credentials.Password);

                if (user is ValidatedUser)
                {
                    var claims = new List<Claim>(user.Claims.Select(c => new Claim(ClaimTypes.Role, c)))
                    {
                        new Claim(ClaimTypes.Name, user.UserName)
                    };

                    var token = new JwtToken
                    {
                        Issuer = "https://localhost",
                        Audience = "https://localhost",
                        Claims = claims,
                        Expiry = DateTime.UtcNow.AddDays(1),
                    };

                    var encodedToken = JsonWebToken.Encode(token, SecretKey, JwtHashAlgorithm.HS256);

                    return Response.AsJson(encodedToken);
                }

                return HttpStatusCode.Unauthorized;
            };
        }
    }
}