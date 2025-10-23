export function getAllCookies() {
    return document.cookie;
}
export function setCookie(command) {
    document.cookie = command;
}
export function deleteCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT; path=/';
}
export function deleteAllCookies() {
    document.cookie.split(';').forEach(c => {
        let eqPos = c.indexOf('=');
        let name = eqPos > -1 ? c.substr(0, eqPos).trim() : c.trim();
        document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/';
    });
}
