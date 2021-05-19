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
                new ApiScope("qvacar.api.core","Qva Car Main Backend")
            };
        public static IEnumerable<ApiResource> ApiResources =>
           new ApiResource[] {
                new ApiResource(
                    "qvacar.api.core",
                    "Qva Car Backend Api",
                    new string[] {  })
                    {
                        Scopes = { "qvacar.api.core"},
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
                    ClientId = "qvacar.mobile.xamarin",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "qvacar.api.core",                        
                    },
                    RequireConsent = false,
                    RedirectUris = { "qvacarmobilexamarin://callback" },
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AllowAccessTokensViaBrowser = true,
                    PostLogoutRedirectUris = { "qvacarmobilexamarin://logout" },
                },
            };
    }
}