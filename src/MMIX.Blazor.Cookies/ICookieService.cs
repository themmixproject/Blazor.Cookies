using System.Net;
using Microsoft.AspNetCore.Http;

namespace MMIX.Blazor.Cookies;

/// <summary>
/// Allows interacting with browser cookies.
/// </summary>
public interface ICookieService
{
    /// <summary>
    /// Retrieves all cookies.
    /// </summary>
    public Task<IEnumerable<Cookie>> GetAllAsync();

    /// <summary>
    /// Retrieves a cookie by its name.
    /// </summary>
    /// <param name="name">The cookie name.</param>
    public Task<Cookie?> GetAsync(string name);

    /// <summary>
    /// Removes a cookie by marking it as expired.
    /// </summary>
    /// <param name="name">The cookie name.</param>
    /// <include file='ICookieService.doc.xml' path='doc/params'/>
    /// 
    public Task RemoveAsync(
        string name,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Removes all cookies by marking them as expired.
    /// </summary>
    /// <param name="cancellationToken">
    /// Token that can be used to cancel the operation.
    /// </param>
    /// <returns></returns>
    public Task RemoveAllAsync(CancellationToken cancellationToken);

    /// <summary>Creates or updates a cookie.</summary>
    /// <remarks>
    /// In Static SSR mode, newly set cookies are only available on the client
    /// after a page reload.
    /// </remarks>
    /// <param name="cookie">The cookie object to set.</param>
    /// <param name="cancellationToken">
    /// Token that can be used to cancel the operation.
    /// </param>
    public Task SetAsync(
        Cookie cookie,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates or updates multiple cookies.
    /// </summary>
    /// <remarks>
    /// In Static SSR mode, newly set cookies are only available on the client
    /// after a page reload.
    /// </remarks>
    /// <param name="cookies">The cookies to set.</param>
    /// <param name="cancellationToken">
    /// Token that can be used to cancel the operation.
    /// </param>
    public Task SetAsync(
        IEnumerable<Cookie> cookies,
        CancellationToken cancellationToken = default
    );

    /// <summary>Creates or updates a cookie.</summary>
    /// <remarks>
    /// In Static SSR mode, newly set cookies are only available on the client
    /// after a page reload.
    /// </remarks>
    /// <param name="name">The cookie name.</param>
    /// <param name="value">The cookie value.</param>
    /// <param name="cancellationToken">
    /// Token that can be used to cancel the operation.
    /// </param>
    public Task SetAsync(
        string name,
        string value,
        CancellationToken cancellationToken = default
    );

    /// <summary>Creates or updates a cookie.</summary>
    /// <remarks>
    /// In Static SSR mode, newly set cookies are only available on the client
    /// after a page reload.
    /// </remarks>
    /// <param name="expires">The cookie expiration date.</param>
    /// <inheritdoc cref="SetAsync(string, string, CancellationToken)"/>
    public Task SetAsync(
        string name,
        string value,
        DateTime expires,
        CancellationToken cancellationToken = default
    );

    /// <summary>Creates or updates a cookie.</summary>
    /// <remarks>
    /// In Static SSR mode, newly set cookies are only available on the client
    /// after a page reload.
    /// </remarks>
    /// <param name="cookieOptions">The cookie options to apply </param>
    /// <inheritdoc cref="SetAsync(string, string, CancellationToken)"/>
    public Task SetAsync(
        string name,
        string value,
        CookieOptions cookieOptions,
        CancellationToken cancellationToken = default
    );

    // TODO: Add SetAsync(IEnumerable, CancellationToken) overload
}
