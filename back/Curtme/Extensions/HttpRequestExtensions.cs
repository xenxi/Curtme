using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Curtme.Extensions
{
    public static class HttpRequestExtensions
    {
        public static RequestInfo GetRequestInfo(this HttpContext context)
        {
            var requestInfo = new RequestInfo();

            requestInfo.Language = context.Request.GetTypedHeaders()
                                          .AcceptLanguage?
                                          .OrderByDescending(acceptedLanguage => acceptedLanguage.Quality ?? 1)
                                          .Select(language => language.ToString().Split('-')[0])
                                          .FirstOrDefault() ?? String.Empty;

            requestInfo.IpAddress = context.Connection.RemoteIpAddress;

            requestInfo.ReferrerDomain = context.GetDomainReferer();

            return requestInfo;
        }

        private static String GetDomainReferer(this HttpContext context)
        {
            var refererUrl = context.Request.Headers["Referer"].FirstOrDefault();

            return refererUrl.GetDomainName() ?? "Unknown";
        }

        public static Boolean IsRecursiveURL(this String url, HttpContext context)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                   uriResult.Authority == context.GetAbsoluteUri().Authority;
        }

        private static Uri GetAbsoluteUri(this HttpContext context)
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = context.Request.Scheme;
            uriBuilder.Host = context.Request.Host.Host;
            uriBuilder.Path = context.Request.Path.ToString();
            uriBuilder.Query = context.Request.QueryString.ToString();

            return uriBuilder.Uri;
        }
    }

    public class RequestInfo
    {
        public String Language { get; set; }

        public IPAddress IpAddress { get; set; }

        public String ReferrerDomain { get; set; }
    }
}