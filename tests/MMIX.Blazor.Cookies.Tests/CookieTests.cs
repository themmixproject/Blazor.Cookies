using System.Net;

namespace MMIX.Blazor.Cookies.Tests
{
    public class CookieTests
    {
        [Fact]
        public void ConstructCookie_WithEmptyNameAndValue_ShouldThrowException()
        {
            Assert.Throws<CookieException>(() =>
            {
                Cookie cookie = new Cookie("", "");
            });
        }

        [Fact]
        public void ConstructCookie_WithEmptyValue_ShouldCreateCookie()
        {
            var cookie = new Cookie("name", "");
            Assert.IsType<Cookie>(cookie);
        }

        [Fact]
        public void ConstructCookie_WithEmptyName_ShouldThrowException()
        {
            Assert.Throws<CookieException>(() =>
            {
                Cookie cookie = new Cookie("", "value");
            });
        }
    }
}
