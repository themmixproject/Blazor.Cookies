const document = {
    _cookies: {},
    _keys: {
        domain: "domain",
        expires: "expires",
        maxAge: "max-age",
        path: "path",
        samesite: "samesite",
        secure: "secure"
    },
    _cookieDateTimeFormat: "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
    get cookie(){
        this.RemoveExpiredCookies();
        return this.GetCookieString();
    },
    set cookie(cookieString){
        let cookieParts = this.GetCookiePartsFromCookieString(cookieString);
        cookieParts = this.RemoveDuplicateCookieParts(cookieParts);
        cookieParts = this.RemoveExtraCustomKeys(cookieParts);
        
        let cookie = this.CreateCookieFromCookieParts(cookieParts);
        this.AddOrUpdateCookie(cookie);
    },

    GetCookieString: function() {
        let cookies = [];

        let keys = Object.keys(this._cookies);
        for (let i = 0; i < keys.length; i++){
            let key = keys[i];

            let cookie = this._cookies[key];

            if (cookie.name) {
                cookies.push(`${cookie.name}=${cookie.value}`);
            }
            else {
                cookies.push(cookie.value);
            }
        }

        return cookies.join("; ")
    },
    
    GetCookiePartsFromCookieString: function (cookieString) {
        let stringParts = cookieString.split(";");

        let cookieParts = [];
        for (let i = 0; i < stringParts.length; i++) {
            let stringPart = stringParts[i];
            let keyValuePair = this.GetKeyValueFromCookiePart(stringPart);

            let isEmptyKeyValue = keyValuePair.key == "" &&
                                  keyValuePair.value == "";
            if (isEmptyKeyValue) { continue; }
            
            cookieParts.push({
                name: keyValuePair.key,
                value: keyValuePair.value
            });
        };

        return cookieParts;
    },
    
    GetKeyValueFromCookiePart: function (cookiePart) {
        let cookiePartSplit = cookiePart.split("=", 2);
        
        if (cookiePartSplit.length == 1){
            let trimmedPart = cookiePartSplit[0].trim();
            let lowerCasePart = trimmedPart.toLowerCase();
            
            if(lowerCasePart == this._keys.secure){
                return { key: trimmedPart, value: ""};
            }

            return { key: "", value: cookiePartSplit[0].trim() };
        }

        let key = cookiePartSplit[0].trim();
        let value = cookiePartSplit[1].trim();
        return { key: key, value: value };
    },

    RemoveDuplicateCookieParts: function (cookieParts) {
        let existingKeys = [];
        let nonDuplicates = [];

        cookieParts.forEach(function(cookiePart){
            if (!existingKeys.indexOf(cookiePart.name.toLowerCase()) > -1){
                nonDuplicates.push(cookiePart);
            }
        });

        return nonDuplicates;
    },

    RemoveExtraCustomKeys: function (cookieParts) {
        let customKeyExists = false;
        let filteredParts = [];

        for (let i = 0; i < cookieParts.length; i++) {
            let cookiePart = cookieParts[i];

            if (this.IsCustomKey(cookiePart.name)) {
                if (!customKeyExists) {
                    filteredParts.push(cookiePart);
                }
                
                customKeyExists = true;
            }
            else {
                filteredParts.push(cookiePart);
            }
        };

        return filteredParts;
    },

    IsCustomKey: function (key) {
        validKeys = [
            this._keys.domain,
            this._keys.expires,
            this._keys.maxAge,
            "partitioned",
            this._keys.path,
            this._keys.samesite
        ];
        
        let lowerCaseKey = key.toLowerCase();
        let isCustomKey = validKeys.indexOf(lowerCaseKey) == -1;
        return isCustomKey;
    },

    CreateCookieFromCookieParts: function (cookieParts) {
        let cookie = {};

        for (let i = 0; i < cookieParts.length; i++) {
            let cookiePart = cookieParts[i];

            let lowerCaseKey = cookiePart.name.toLowerCase();
            if (lowerCaseKey == this._keys.domain) { cookie.domain = cookiePart.value; }
            else if (lowerCaseKey == this._keys.expires) { cookie.expires = Date.parse(cookiePart.value); }
            else if (lowerCaseKey == this._keys.secure) { cookie.secure = true; }
            else if (lowerCaseKey == this._keys.maxAge) { cookie.maxAge = parseInt(cookiePart.value); }
            else if (lowerCaseKey == this._keys.path) { cookie.path = cookiePart.value; }
            else if (lowerCaseKey == this._keys.samesite) { cookie.samesite = cookiePart.value; }
            else if (this.IsCustomKey(cookiePart.name)) {
                cookie.name = cookiePart.name;
                cookie.value = cookiePart.value;
            }
        };

        return cookie;
    },

    AddOrUpdateCookie: function (cookie) {
        if (cookie.name in this._cookies) {
            let oldCookie = this._cookies[cookie.name];
            this._cookies[cookie.name] = this.UpdateCookie(oldCookie, cookie);

            this.RemoveCookieIfExpired(cookie);
        }
        
        else {
            this._cookies[cookie.name] = cookie;
        }
    },

    UpdateCookie: function (oldCookie, newCookie){
        if (newCookie.value) { oldCookie.value = newCookie.value; }
        if (newCookie.domain) { oldCookie.domain = newCookie.domain; }
        if (newCookie.expires) { oldCookie.expires = newCookie.expires; }
        if (newCookie.path) { oldCookie.path= newCookie.path; }
        if (newCookie.maxAge) { oldCookie.maxAge = maxAge.MaxAge; }
        if (newCookie.secure) { oldCookie.secure = newCookie.secure; }

        return oldCookie;
    },

    RemoveCookieIfExpired: function (cookie) {
        let isExpired = new Date() > cookie.expires;
        if (isExpired) {
            delete this._cookies[cookie.name]
        }
    },

    RemoveExpiredCookies: function () {
        let keys = Object.keys(this._cookies);
        
        for (let i = 0; i < keys.length; i++) {
            let key = keys[i];
            let cookie = this._cookies[key];
            
            this.RemoveCookieIfExpired(cookie);
        };
    }    
};
