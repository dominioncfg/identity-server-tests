// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace TestIdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("qvacarapibackend","Qva Car Main Backend")
            };
        public static IEnumerable<ApiResource> ApiResources =>
           new ApiResource[] {
                new ApiResource(
                    "qvacarapibackend",
                    "Qva Car Backend Api",
                    new string[] {  })
                    {
                        Scopes = { "qvacarapibackend"},
                        ApiSecrets = { new Secret("apisecret".Sha256())}
                    }
               };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "Qva Car Mobile",
                    RequireClientSecret = false,
                    ClientId = "mobileqvacarbackendapiclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "qvacarapibackend", IdentityServerConstants.StandardScopes.OfflineAccess },
                    RequireConsent = false,
                    RedirectUris = { "xamarinformsclients://callback" },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowAccessTokensViaBrowser = true,
                    PostLogoutRedirectUris = { "xamarinformsclients://logout" },
                },
            };
    }
}