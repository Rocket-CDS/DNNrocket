# Login Redirect (Cross-Site)

### Introduction
When a DNN page or module denies access to an unauthenticated user, DNN issues a `302` redirect to its own internal login page.  
This feature intercepts that redirect and forwards the user to a **login page on a different website** instead.

This is useful in SSO scenarios where authentication is managed by a central hub installation.

The feature is built into `HttpModuleRocket` (in `DNNrocketAPI`) and requires no additional HTTP module registration.

---

### How It Works

`HttpModuleRocket` hooks the `EndRequest` pipeline event. On every request it checks:

1. Is the response a `302` redirect?
2. Does the redirect `Location` header contain `/Login` or `ctl=Login`?
3. Is there an **External API** entry configured with the flag `LOGINREDIRECT` in its **ApiKey** field?

If all three conditions are true, the redirect destination is replaced with the configured `ApiUrl`.

---

### Configuration

No code changes are required. The redirect is activated purely through the **Global Settings > External APIs** panel in the DNNrocket admin UI.

Add an entry with the following values:

| Field      | Value                                      |
|------------|--------------------------------------------|
| **ApiRef** | Any unique name (e.g. `LoginHub`)          |
| **ApiUrl** | The full URL of the external login page (e.g. `https://hub.example.com/login`) |
| **ApiKey** | Must contain `LOGINREDIRECT`               |

**To disable the redirect**, remove the entry or clear the `ApiKey` value.

---

### Resulting Redirect

When triggered, the browser is redirected to:

```
https://hub.example.com/login
```

The external login page is responsible for authenticating the user.

---

### Notes

- The `ApiKey` field is checked with a `Contains("LOGINREDIRECT")` match, so additional text can coexist in that field if needed.
- If multiple External API entries have `LOGINREDIRECT` in the `ApiKey`, the **first match** is used.
- This only intercepts DNN's own login redirect. Direct `401`/`403` HTTP responses (rare in DNN) are not affected.
- Errors are written to the standard DNNrocket log (`\Portals\_default\Logs\*`).

---

### Related Files

- `DNNrocketAPI/Components/HttpModuleRocket.cs` — `OnEndRequest` method contains the redirect logic.
- `DNNrocketAPI/Components/SystemGlobalData.cs` — `GetExternalAPIs()` reads the configured entries.
