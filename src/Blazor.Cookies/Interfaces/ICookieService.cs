using System.Net;
using Microsoft.AspNetCore.Http;

namespace Blazor.Cookies.Interfaces;

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
    /// Retrieves a cookie by its key.
    /// </summary>
    /// <param name="name"> Key of the cookie to retrieve. </param>
    public Task<Cookie?> GetAsync(string name);

    /// <summary>
    /// Removes a cookie by marking it as expired.
    /// </summary>
    /// <param name="key"> The key of the cookie to remove. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    public Task RemoveAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds or updates a browser cookie. <br/> <br/>
    /// <b>Note: </b>
    /// When in Static SSR render mode, the new value will only be sent to
    /// the client machine after the page has completed rendering, and thus
    /// will not appear in the cookies collection until the next request.
    /// </summary>
    /// <param name="cookie"> The cookie to set. </param>
    /// <param name="cancellationToken"> Cancellation token. </param>
    public Task SetAsync(Cookie cookie, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="SetAsync(Cookie, CancellationToken)"/>
    /// <param name="sameSiteMode">
    /// Controls whether or not a cookie is sent with cross-site
    /// requests
    /// <br />
    /// <b>Note:</b> Null value will result in the browser using it's
    /// default behavior.
    /// </param>
    public Task SetAsync(Cookie cookie, SameSiteMode sameSiteMode, CancellationToken cancellationToken = default);
    /// <summary>
    /// Adds or updates a browser cookie.
    /// </summary>
    /// <param name="name">The name of the cookie to set.</param>
    /// <param name="value">The value of the cookie to set.</param>
    /// <param name="cancellationToken"></param>
    public Task SetAsync(string name, string value, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="SetAsync(string, string, CancellationToken)"/>
    /// <param name="expires">
    /// The cookie's expiration date.
    /// </param>
    public Task SetAsync(string name, string value, DateTime expires, CancellationToken cancellationToken = default);
    /// <inheritdoc cref="SetAsync(string, string, DateTime, CancellationToken)"/>
    /// <param name="sameSiteMode">
    /// Controls whether or not a cookie is sent with cross-site
    /// requests
    /// <br />
    /// <b>Note:</b> Null value will result in the browser using it's
    /// default behavior.
    /// </param>
    public Task SetAsync(string name, string value, DateTime expires, SameSiteMode sameSiteMode, CancellationToken cancellationToken = default);
}
