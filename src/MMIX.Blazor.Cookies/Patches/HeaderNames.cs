// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace MMIX.Blazor.Cookies.Patches;

/// <summary>
/// Defines constants for well-known HTTP headers.
/// </summary>
// MODIFICATION POLICY: This list is not intended to be exhaustive, it primarily contains values used by the framework itself.
// Please do not open PRs without first opening an issue to discuss a specific item.
internal static class HeaderNames
{
    // Use readonly statics rather than constants so ReferenceEquals works

    /// <summary>Gets the <c>Accept</c> HTTP header name.</summary>
    public static readonly string Accept = "Accept";

    /// <summary>Gets the <c>Accept-Charset</c> HTTP header name.</summary>
    public static readonly string AcceptCharset = "Accept-Charset";

    /// <summary>Gets the <c>Accept-Encoding</c> HTTP header name.</summary>
    public static readonly string AcceptEncoding = "Accept-Encoding";

    /// <summary>Gets the <c>Accept-Language</c> HTTP header name.</summary>
    public static readonly string AcceptLanguage = "Accept-Language";

    /// <summary>Gets the <c>Accept-Ranges</c> HTTP header name.</summary>
    public static readonly string AcceptRanges = "Accept-Ranges";

    /// <summary>Gets the <c>Age</c> HTTP header name.</summary>
    public static readonly string Age = "Age";

    /// <summary>Gets the <c>Allow</c> HTTP header name.</summary>
    public static readonly string Allow = "Allow";

    /// <summary>Gets the <c>Authorization</c> HTTP header name.</summary>
    public static readonly string Authorization = "Authorization";

    /// <summary>Gets the <c>Cache-Control</c> HTTP header name.</summary>
    public static readonly string CacheControl = "Cache-Control";

    /// <summary>Gets the <c>Connection</c> HTTP header name.</summary>
    public static readonly string Connection = "Connection";

    /// <summary>Gets the <c>Content-Disposition</c> HTTP header name.</summary>
    public static readonly string ContentDisposition = "Content-Disposition";

    /// <summary>Gets the <c>Content-Encoding</c> HTTP header name.</summary>
    public static readonly string ContentEncoding = "Content-Encoding";

    /// <summary>Gets the <c>Content-Language</c> HTTP header name.</summary>
    public static readonly string ContentLanguage = "Content-Language";

    /// <summary>Gets the <c>Content-Length</c> HTTP header name.</summary>
    public static readonly string ContentLength = "Content-Length";

    /// <summary>Gets the <c>Content-Location</c> HTTP header name.</summary>
    public static readonly string ContentLocation = "Content-Location";

    /// <summary>Gets the <c>Content-MD5</c> HTTP header name.</summary>
    public static readonly string ContentMD5 = "Content-MD5";

    /// <summary>Gets the <c>Content-Range</c> HTTP header name.</summary>
    public static readonly string ContentRange = "Content-Range";

    /// <summary>Gets the <c>Content-Type</c> HTTP header name.</summary>
    public static readonly string ContentType = "Content-Type";

    /// <summary>Gets the <c>Cookie</c> HTTP header name.</summary>
    public static readonly string Cookie = "Cookie";

    /// <summary>Gets the <c>Date</c> HTTP header name.</summary>
    public static readonly string Date = "Date";

    /// <summary>Gets the <c>ETag</c> HTTP header name.</summary>
    public static readonly string ETag = "ETag";

    /// <summary>Gets the <c>Expires</c> HTTP header name.</summary>
    public static readonly string Expires = "Expires";

    /// <summary>Gets the <c>Expect</c> HTTP header name.</summary>
    public static readonly string Expect = "Expect";

    /// <summary>Gets the <c>From</c> HTTP header name.</summary>
    public static readonly string From = "From";

    /// <summary>Gets the <c>Host</c> HTTP header name.</summary>
    public static readonly string Host = "Host";

    /// <summary>Gets the <c>If-Match</c> HTTP header name.</summary>
    public static readonly string IfMatch = "If-Match";

    /// <summary>Gets the <c>If-Modified-Since</c> HTTP header name.</summary>
    public static readonly string IfModifiedSince = "If-Modified-Since";

    /// <summary>Gets the <c>If-None-Match</c> HTTP header name.</summary>
    public static readonly string IfNoneMatch = "If-None-Match";

    /// <summary>Gets the <c>If-Range</c> HTTP header name.</summary>
    public static readonly string IfRange = "If-Range";

    /// <summary>Gets the <c>If-Unmodified-Since</c> HTTP header name.</summary>
    public static readonly string IfUnmodifiedSince = "If-Unmodified-Since";

    /// <summary>Gets the <c>Last-Modified</c> HTTP header name.</summary>
    public static readonly string LastModified = "Last-Modified";

    /// <summary>Gets the <c>Location</c> HTTP header name.</summary>
    public static readonly string Location = "Location";

    /// <summary>Gets the <c>Max-Forwards</c> HTTP header name.</summary>
    public static readonly string MaxForwards = "Max-Forwards";

    /// <summary>Gets the <c>Pragma</c> HTTP header name.</summary>
    public static readonly string Pragma = "Pragma";

    /// <summary>Gets the <c>Proxy-Authenticate</c> HTTP header name.</summary>
    public static readonly string ProxyAuthenticate = "Proxy-Authenticate";

    /// <summary>Gets the <c>Proxy-Authorization</c> HTTP header name.</summary>
    public static readonly string ProxyAuthorization = "Proxy-Authorization";

    /// <summary>Gets the <c>Range</c> HTTP header name.</summary>
    public static readonly string Range = "Range";

    /// <summary>Gets the <c>Referer</c> HTTP header name.</summary>
    public static readonly string Referer = "Referer";

    /// <summary>Gets the <c>Retry-After</c> HTTP header name.</summary>
    public static readonly string RetryAfter = "Retry-After";

    /// <summary>Gets the <c>Server</c> HTTP header name.</summary>
    public static readonly string Server = "Server";

    /// <summary>Gets the <c>Set-Cookie</c> HTTP header name.</summary>
    public static readonly string SetCookie = "Set-Cookie";

    /// <summary>Gets the <c>TE</c> HTTP header name.</summary>
    public static readonly string TE = "TE";

    /// <summary>Gets the <c>Trailer</c> HTTP header name.</summary>
    public static readonly string Trailer = "Trailer";

    /// <summary>Gets the <c>Transfer-Encoding</c> HTTP header name.</summary>
    public static readonly string TransferEncoding = "Transfer-Encoding";

    /// <summary>Gets the <c>Upgrade</c> HTTP header name.</summary>
    public static readonly string Upgrade = "Upgrade";

    /// <summary>Gets the <c>User-Agent</c> HTTP header name.</summary>
    public static readonly string UserAgent = "User-Agent";

    /// <summary>Gets the <c>Vary</c> HTTP header name.</summary>
    public static readonly string Vary = "Vary";

    /// <summary>Gets the <c>Via</c> HTTP header name.</summary>
    public static readonly string Via = "Via";

    /// <summary>Gets the <c>Warning</c> HTTP header name.</summary>
    public static readonly string Warning = "Warning";

    /// <summary>Gets the <c>Sec-WebSocket-Protocol</c> HTTP header name.</summary>
    public static readonly string WebSocketSubProtocols = "Sec-WebSocket-Protocol";

    /// <summary>Gets the <c>WWW-Authenticate</c> HTTP header name.</summary>
    public static readonly string WWWAuthenticate = "WWW-Authenticate";
}