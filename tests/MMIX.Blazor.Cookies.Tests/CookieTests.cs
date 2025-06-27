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
            Assert.Empty(cookie.Name);
            Assert.NotEmpty(cookie.Value);
        }

        [Fact]
        public void ConstructCookie_WithEmptyName_ShouldThrowException()
        {
            Assert.Throws<CookieException>(() =>
            {
                Cookie cookie = new Cookie("", "value");
            });
        }

        [Fact]
        public void ConstructCookie_WithInvalidCharacterInName_ShouldThrowException()
        {
            Assert.Throws<CookieException>(() =>
            {
                Cookie cookie = new Cookie("name=", "value");
            });
        }

        [Fact]
        public void ConstructCookie_WithInvalidCharacterInNameAndEmptyValue_ShouldThrowException()
        {
            Assert.Throws<CookieException>(() =>
            {
                Cookie cookie = new Cookie("name=", "");
            });
        }

        [Fact]
        public void ConstructCookie_WithInvalidCharacterInValue_ShouldCreateCookie()
        {
            Cookie cookie = new Cookie("name", "value=");

            Assert.IsType<Cookie>(cookie);
            Assert.NotEmpty(cookie.Name);
            Assert.NotEmpty(cookie.Value);
        }
    }
}
