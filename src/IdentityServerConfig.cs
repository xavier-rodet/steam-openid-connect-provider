﻿using System;  
using System.Collections.Generic;
using System.Linq;  
using IdentityServer4;
using IdentityServer4.Models;
using IdentityModel;

namespace SteamOpenIdConnectProvider
{
    public class IdentityServerConfig
    {
        public static IEnumerable<Client> GetClients(string clientId, string secret, string redirectUri, string logoutRedirectUri, string allowedHost)
        {
            yield return new Client
            {
                ClientId = clientId,
                ClientName = "Proxy Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequireConsent = false,
                ClientSecrets =
                {
                    new Secret(secret.Sha256())
                },

                // where to redirect to after login
                RedirectUris = redirectUri.Split(",").Select(x => x.Trim()).ToArray(),

                // where to redirect to after logout
                PostLogoutRedirectUris = { logoutRedirectUri },
                RequirePkce = false,
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile/* ,
                    "api" */
                },

                // See: http://docs.identityserver.io/en/latest/quickstarts/6_aspnet_identity.html
                // http://docs.identityserver.io/en/latest/topics/cors.html
                AllowedCorsOrigins = new List<string>
                {
                    allowedHost
                },

                // IMPORTANT: Auth0 needs claims into Id Token so we can display name, picture directly (because it won't request /userinfo)
                // http://docs.identityserver.io/en/latest/reference/client.html#token
                // https://auth0.com/docs/connections/enterprise/oidc
                AlwaysIncludeUserClaimsInIdToken = true
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // scopes define the API resources in your system
        // See: https://stackoverflow.com/a/44932837/3254208
        // https://docs.identityserver.io/en/latest/reference/api_resource.html
        // https://github.com/IdentityModel/IdentityModel/blob/main/src/JwtClaimTypes.cs
        // https://blogs.infinitesquare.com/posts/web/implementer-identityserver-4-dans-une-application-apdotnet-core-2-2 
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api", "Api", new[] { JwtClaimTypes.Subject, JwtClaimTypes.Name , JwtClaimTypes.NickName, JwtClaimTypes.Picture  })
            };
        }
    }
}