using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace TestIdentityServer
{
    public static class QvaCarClaims
    {
        public const string Province = "province";
    }

    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var provinceIdentityRes = new IdentityResource(
                        name: "province",
                        displayName: "Province",
                        userClaims: new[] { QvaCarClaims.Province });

            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                provinceIdentityRes,
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            var coreApiScopes = new ApiScope("qvacar.api.core", "Qva Car Main Backend");
            //var province = new ApiScope(QvaCarClaims.Province, "Province Api Scope");
            return new[] { coreApiScopes };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            var qvaCarCoreApiResouce = new ApiResource(
                    "qvacar.api.core",
                    "Qva Car Backend Api",
                    new string[] { QvaCarClaims.Province })
            {
                Scopes = { "qvacar.api.core", },
                ApiSecrets = { new Secret("apisecret".Sha256()) }
            };
            return new[] { qvaCarCoreApiResouce };
        }

        public static IEnumerable<Client> GetClients()
        {
            var mobileAppClient = new Client
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
                        IdentityServerConstants.StandardScopes.Address,
                        QvaCarClaims.Province,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "qvacar.api.core",
                    },
                RequireConsent = false,
                RedirectUris = { "qvacarmobilexamarin://callback" },
                UpdateAccessTokenClaimsOnRefresh = true,
                AllowAccessTokensViaBrowser = true,
                PostLogoutRedirectUris = { "qvacarmobilexamarin://logout" },
            };
            return new[] { mobileAppClient };
        }
    }
}