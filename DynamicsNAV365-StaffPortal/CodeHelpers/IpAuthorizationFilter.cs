using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace DynamicsNAV365_StaffPortal.CodeHelpers
{
    public class IpAuthorizationFilter : AuthorizationFilterAttribute
    {
        private readonly List<string> _allowedIpAddresses;

        public IpAuthorizationFilter(params string[] allowedIpAddresses)
        {
            _allowedIpAddresses = allowedIpAddresses.ToList();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var remoteIpAddress = actionContext.Request.RequestUri.Host;

            if (!_allowedIpAddresses.Contains(remoteIpAddress))
            {
                var dnsHostEntry = Dns.GetHostEntry(remoteIpAddress);
                if (dnsHostEntry.HostName == null || !_allowedIpAddresses.Contains(dnsHostEntry.HostName))
                {
                    HandleUnauthorizedRequest(actionContext);
                }
            }

            base.OnAuthorization(actionContext);
        }

        private void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                ReasonPhrase = "Access denied. You are not authorized to access this resource."
            };
        }
    }
    public class ApiKeyAuthorizeAttribute : AuthorizeAttribute
    {
        private const string ApiKey = "integration";

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            // Retrieve the API key from the request header
            if (!actionContext.Request.Headers.Contains("ApiKey"))
            {
                return false;
            }

            var apiKey = actionContext.Request.Headers.GetValues("ApiKey").FirstOrDefault();

            // Validate the API key
            return IsValidApiKey(apiKey);
        }

        private bool IsValidApiKey(string apiKey)
        {
            // Compare the provided API key with the expected API key
            return string.Equals(apiKey, ApiKey, StringComparison.OrdinalIgnoreCase);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            // Send unauthorized response with appropriate status code and message
            actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, "Invalid API key.");
        }
    }
}