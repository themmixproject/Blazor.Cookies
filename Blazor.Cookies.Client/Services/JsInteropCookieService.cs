﻿using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Net;
using Blazor.Cookies.Interfaces;

namespace Blazor.Cookies.Client.Services
{
    public class JsInteropCookieService(IJSRuntime JSRuntime) : ICookieService
    {
        private const string CookieFlagsExplainMessage = "Setting HttpOnly or Secure cookies is only possible when using Satic SSR render mode.";
        private const string NotSupportedSuffix = "are not supported in this rendering environment";
        private const string HttpOnlyFlagExceptionMessage = $"HttpOnly cookies {NotSupportedSuffix}. {CookieFlagsExplainMessage}";
        private const string SecureFlagExceptionMessage = $"Secure cookies {NotSupportedSuffix}. {CookieFlagsExplainMessage}";

        public async Task<IEnumerable<Cookie>> GetAllAsync()
        {
            var raw = await JSRuntime.InvokeAsync<string>("eval", "document.cookie");
            if (string.IsNullOrWhiteSpace(raw)) { return []; }

            return raw.Split("; ").Select(ToCookie);
        }

        private Cookie ToCookie(string raw)
        {
            var parts = raw.Split("=");
            return new Cookie { Name = parts[0], Value = parts[1] };
        }

        public async Task<Cookie?> GetAsync(string name)
        {
            var cookies = await GetAllAsync();
            return cookies.FirstOrDefault(c => c.Name == name);
        }
        
        public async Task SetAsync(
            Cookie cookie,
            CancellationToken cancellationToken = default
        ) {
            ValidateCookie(cookie);
            await ExecuteSetCookieJavaScriptInteropAsync(cookie);
        }
        public async Task SetAsync(
            Cookie cookie,
            SameSiteMode sameSite,
            CancellationToken cancellationToken = default
        ) {
            ValidateCookie(cookie);
            await ExecuteSetCookieJavaScriptInteropAsync(cookie, sameSite);
        }
        public async Task SetAsync(
            string name,
            string value, 
            CancellationToken cancellationToken = default
        ) {
            Cookie cookie = new Cookie(name, value);
            ValidateCookie(cookie);
            await ExecuteSetCookieJavaScriptInteropAsync(cookie);
        }
        public async Task SetAsync(
            string name,
            string value,
            DateTime expires,
            CancellationToken cancellationToken = default
        ) {
            Cookie cookie = new Cookie
            {
                Name = name,
                Value = value,
                Expires = expires,
            };
            ValidateCookie(cookie);
            await ExecuteSetCookieJavaScriptInteropAsync(cookie);
        }
        public async Task SetAsync(
            string name,
            string value,
            DateTime expires,
            SameSiteMode sameSiteMode,
            CancellationToken cancellationToken = default
        ) {
            Cookie cookie = new Cookie
            {
                Name = name,
                Value = value,
                Expires = expires,
            };
            ValidateCookie(cookie);
            await ExecuteSetCookieJavaScriptInteropAsync(cookie, sameSiteMode);
        }

        private void ValidateCookie(Cookie cookie)
        {
            if (cookie.HttpOnly) {
                throw new InvalidOperationException(HttpOnlyFlagExceptionMessage);
            }
            if (cookie.Secure) {
                throw new InvalidOperationException(SecureFlagExceptionMessage);
            }
        }

        private async Task ExecuteSetCookieJavaScriptInteropAsync(
            Cookie cookie,
            SameSiteMode? sameSite
        ) {
            string command =
                $"document.cookie = '{cookie.Name}={cookie.Value}; " +
                $"expires={cookie.Expires};" +
                $"path=/";

            if (sameSite != null) { command += $"; SameSite={sameSite.ToString()}"; }
            command += "'";

            await JSRuntime.InvokeVoidAsync("eval", command);
        }

        private async Task ExecuteSetCookieJavaScriptInteropAsync(Cookie cookie)
        {
            string command =
                $"document.cookie = '{cookie.Name}={cookie.Value}; " +
                $"expires={cookie.Expires};" +
                $"path=/";

            await JSRuntime.InvokeVoidAsync("eval", command);
        }

        public async Task RemoveAsync(
            string name,
            CancellationToken cancellationToken = default
        ) {
            if (string.IsNullOrWhiteSpace(name)) { throw new Exception("Name is required when removing a cookie."); }

            string command = $"document.cookie = '{name}=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/'";
            await JSRuntime.InvokeVoidAsync("eval", command);
        }
    }
}
