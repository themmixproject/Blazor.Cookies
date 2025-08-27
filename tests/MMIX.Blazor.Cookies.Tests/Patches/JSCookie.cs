using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace MMIX.Blazor.Cookies.Tests.Patches;

public class JSCookie
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string? Domain { get; set; } = null;
    public string Path { get; set; } = "/";
    public DateTime? Expires { get; set; } = null;
    public int? MaxAge
    {
        get
        {
            if (Expires == null)
            {
                return null;
            }
            
            var remainingSeconds = ((DateTime)Expires - DateTime.UtcNow).TotalSeconds;
            return Convert.ToInt32(Math.Floor(remainingSeconds));
        }
        set
        {
            if (value == null)
            {
                Expires = null;
            }
            else
            {
                Expires = DateTime.UtcNow.AddSeconds(Convert.ToDouble(value));
            }
        }
    }
    public bool Secure { get; set; } = false;
    public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;

    public JSCookie()
    {
        Name = "";
        Value = "";
    }

    public JSCookie(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public JSCookie(string name, string value, DateTime expires)
    {
        Name = name;
        Value = value;
        Expires = expires;
    }

    public JSCookie(string name, string value, int maxAge)
    {
        Name = name;
        Value = value;
        MaxAge = maxAge;
    }
}