using Microsoft.JSInterop;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MMIX.Blazor.Cookies.Tests.Patches;

internal class VirtualJSRuntime : IJSRuntime
{
    private const string cookieDateFormat = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";

    private Dictionary<string, JSCookie> _cookiesDictionary = new Dictionary<string, JSCookie>();
    private static class Keys {
        public const string Domain = "domain";
        public const string Expires = "expires";
        public const string MaxAge = "max-age";
        public const string Path = "path";
        public const string SameSite = "samesite";
        public const string Secure = "secure";
    };

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        if (Regex.IsMatch(identifier, "eval$") && args is { Length: 1 })
        {
            string command = args[0]?.ToString() ?? "";
            var commandResult = ParseEvalCommand(command);

            if (commandResult is TValue value)
            {
                return ValueTask.FromResult(value);
            }
        }

        return ValueTask.FromResult(default(TValue)!);
    }

    private object? ParseEvalCommand(string command)
    {
        if (Regex.IsMatch(command, @"document\.cookie$"))
        {
            RemoveExpiredCookies();
            return GetCookiesString();
        }
        if (Regex.IsMatch(command, @"document\.cookie\s*=\s*"))
        {
            var cookieString = ExtractCookieStringFromCommand(command);
            ParseCookieString(cookieString);

            return cookieString;
        }

        return null;
    }

    private string GetCookiesString()
    {
        var cookies = new List<string>();

        foreach (var cookie in _cookiesDictionary.Values)
        {
            if (!string.IsNullOrEmpty(cookie.Name))
            {
                cookies.Add($"{cookie.Name}={cookie.Value}");
            }
            else
            {
                cookies.Add(cookie.Value);
            }
        }

        return string.Join("; ", cookies);
    }

    private string ExtractCookieStringFromCommand(string command)
    {
        var singleQuoteMatch = Regex.Match(command, @"document\.cookie\s\s*=\s*\'(.*)\'");
        if (singleQuoteMatch.Success)
        {
            return singleQuoteMatch.Groups[1].Value;
        }

        var doubleQuoteMatch = Regex.Match(command, @"document\.cookie\s*=\s*""([^""]*)""");
        if (doubleQuoteMatch.Success)
        {
            return doubleQuoteMatch.Groups[1].Value;
        }

        return "";
    }

    private void ParseCookieString(string cookieString)
    {
        List <KeyValuePair<string, string>> cookieParts = GetCookiePartsFromCookieString(cookieString);
        cookieParts = RemoveDuplicateCookieParts(cookieParts);
        cookieParts = RemoveExtraCustomKeys(cookieParts);
        JSCookie cookie = CreateCookieFromCookieParts(cookieParts);

        AddOrUpdateCookie(cookie);
    }

    private List<KeyValuePair<string, string>> GetCookiePartsFromCookieString(string cookieString)
    {
        string[] stringParts = cookieString.Split(";");

        List<KeyValuePair<string, string>> parts = new List<KeyValuePair<string, string>>();
        foreach (string part in stringParts)
        {
            (string key, string value) = GetKeyValueFromCookiePart(part);

            bool isEmptyKeyValue = string.IsNullOrEmpty(key) && string.IsNullOrEmpty(value);
            if (isEmptyKeyValue) { continue; }

            parts.Add(new KeyValuePair<string, string>(key, value));
        }

        return parts;    
    }

    private List<KeyValuePair<string, string>> RemoveDuplicateCookieParts(List<KeyValuePair<string, string>> cookieParts)
    {
        List<string> existingKeys = new List<string>();
        List<KeyValuePair<string, string>> nonDuplicates = new List<KeyValuePair<string, string>>();
        foreach (KeyValuePair<string, string> part in cookieParts)
        {
            if (!existingKeys.Contains(part.Key.ToLower()))
            {
                nonDuplicates.Add(part);
                existingKeys.Add(part.Key.ToLower());
            }
        }

        return nonDuplicates;
    }

    private List<KeyValuePair<string, string>> RemoveExtraCustomKeys(List<KeyValuePair<string, string>> cookieParts) {
        bool customKeyExists = false;

        for (int i = 0; i < cookieParts.Count; i++)
        {
            KeyValuePair<string, string> cookiePart = cookieParts[i];

            if (IsCustomKey(cookiePart.Key))
            {
                if (customKeyExists)
                {
                    cookieParts.RemoveAt(i);
                }

                customKeyExists = true;
            }
        }

        return cookieParts;
    }

    private JSCookie CreateCookieFromCookieParts(List<KeyValuePair<string, string>> cookieParts)
    {
        JSCookie cookie = new JSCookie();
        foreach (KeyValuePair<string, string> cookiePart in cookieParts)
        {
            string lowerCaseKey = cookiePart.Key.ToLower();
            if (lowerCaseKey == Keys.Domain) { cookie.Domain = cookiePart.Value; }
            else if (lowerCaseKey == Keys.Expires)
            {
                cookie.Expires = TryParseExpiresCookiePart(cookiePart.Value);
            }
            else if (lowerCaseKey == Keys.Secure) { cookie.Secure = true; }
            else if (lowerCaseKey == Keys.MaxAge) { cookie.MaxAge = Convert.ToInt32(cookiePart.Value); }
            else if (lowerCaseKey == Keys.Path) { cookie.Path = cookiePart.Value; }
            else if (lowerCaseKey == Keys.SameSite) { continue; }
            else if (IsCustomKey(cookiePart.Key))
            {
                cookie.Name = cookiePart.Key;
                cookie.Value = cookiePart.Value;
            }
        }

        return cookie;
    }

    private DateTime? TryParseExpiresCookiePart(string partValue)
    {
        DateTime parsedDateTime;
        bool parseIsSuccessful = DateTime.TryParseExact(
            partValue,
            cookieDateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal,
            out parsedDateTime
        );
        if (parseIsSuccessful){ return parsedDateTime; }

        return null;
    }

    private void AddOrUpdateCookie(JSCookie cookie)
    {
        if (_cookiesDictionary.ContainsKey(cookie.Name))
        {
            JSCookie oldCookie = _cookiesDictionary[cookie.Name];
            _cookiesDictionary[cookie.Name] = UpdateCookie(oldCookie, cookie);

            RemoveCookieIfExpired(cookie);
        }
        else
        {
            _cookiesDictionary.Add(cookie.Name, cookie);
        }
    }

    private void RemoveCookieIfExpired(JSCookie cookie)
    {
        if (IsExpired(cookie)) {
            _cookiesDictionary.Remove(cookie.Name);
        }
    }

    private bool IsExpired(JSCookie cookie)
    {
        return DateTime.UtcNow > cookie.Expires;
    }

    private void RemoveExpiredCookies()
    {
        var expiredKeys = new List<string>();
        foreach (KeyValuePair<string, JSCookie> cookie in _cookiesDictionary)
        {
            if (IsExpired(cookie.Value)) { expiredKeys.Add(cookie.Key); }
        }

        foreach (string key in expiredKeys) { _cookiesDictionary.Remove(key); }
    }

    private JSCookie UpdateCookie(JSCookie oldCookie, JSCookie newCookie)
    {
        if (newCookie.Value != default) { oldCookie.Value = newCookie.Value; }
        if (newCookie.Domain != default) { oldCookie.Domain = newCookie.Domain; }
        if (newCookie.Expires != default) { oldCookie.Expires = newCookie.Expires; }
        if (newCookie.Path != default) { oldCookie.Path = newCookie.Path; }
        if (newCookie.MaxAge != default) { oldCookie.MaxAge = newCookie.MaxAge; }
        if (newCookie.Secure != default) { oldCookie.Secure = newCookie.Secure; }

        return oldCookie;
    }

    private bool IsCustomKey(string key)
    {
        string lowerCaseKey = key.ToLower();
        string[] validKeys = {
            "domain",
            "expires",
            "max-age",
            "partitioned",
            "path",
            "samesite"
        };

        bool isCustom = !validKeys.Contains(lowerCaseKey);
        return isCustom;
    }

    private (string, string) GetKeyValueFromCookiePart(string cookiePart)
    {
        string[] cookiePartSplit = cookiePart.Split("=", 2);

        if (cookiePartSplit.Length == 1)
        {
            string trimmedPart = cookiePartSplit[0].Trim();
            string lowerCasePart = trimmedPart.ToLowerInvariant();
            if (string.Equals(lowerCasePart, Keys.Secure))
            {
                return (trimmedPart, "");    
            }

            return ("", cookiePartSplit[0].Trim());
        }

        string key = cookiePartSplit[0].Trim();
        string value = cookiePartSplit[1].Trim();
        return (key, value);
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return InvokeAsync<TValue>(identifier, args);
    }
}
