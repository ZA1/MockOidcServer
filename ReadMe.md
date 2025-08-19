MockAuth
========

A simple, lightweight mock OIDC (OpenID Connect) service designed for development and testing environments. MockAuth allows you to quickly define test users and simulate an OIDC authentication flow with a simple one-click login, eliminating the need for complex password management or client/application registration setup.

Features
--------

*   **Mock OIDC Provider:** Implements core OIDC endpoints including discovery, authorisation, token exchange, and logout.
*   **Easy User Authentication:** Offers a passwordless, one-click login experience for defined users.
*   **Simplified Client Integration:** No client registration or `client_id`/`client_secret` management is required for integration.
*   **Automatic Certificate Generation:** Self-signed certificates for HTTPS and JWT signing are automatically generated on startup.
*   **Health Check:** Includes a basic health check endpoint for service monitoring.

Configuration
-------------

MockAuth provides flexible configuration options for defining users and customizing token generation.

### User Configuration

Users represent the identities that can authenticate with MockAuth. The properties defined for each user directly become claims within the generated JWT (ID Token and Access Token). **At a minimum, each user must have an `email` property, which will be used as the `sub` (subject) claim if `sub` is not explicitly provided.**

Users can be defined using one of the following methods:

*   **`users.json` file:** Place a `users.json` file at the root of your application. This file should contain a `Users` array, where each object represents a user.

    Example `users.json` structure:
    ```json
    {
      "Users": [
        {
          "email": "elara.voss@example.com",
          "given_name": "Elara",
          "family_name": "Voss",
          "picture": "https://api.dicebear.com/9.x/avataaars/svg?seed=elara.voss@example.com",
          "roles": ["admin", "editor"]
        },
        {
          "email": "miles.tran@example.com",
          "given_name": "Miles",
          "family_name": "Tran"
        }
      ]
    }
    ```

*   **`appsettings.json` (or `appsettings.Development.json`):** You can also define the `Users` array directly within your application's `appsettings.json` or `appsettings.Development.json` file. This is useful for simpler setups or when you want user definitions to be part of your standard application configuration.

    Example `appsettings.json` snippet for users:
    ```json
    {
      "Users": [
        {
          "email": "jessica.wang@example.com",
          "given_name": "Jessica",
          "family_name": "Wang"
        },
        {
          "email": "david.nguyen@example.com",
          "given_name": "David",
          "family_name": "Nguyen",
          "roles": ["viewer"]
        }
      ],
      "Logging": {
        // ... other logging settings
      }
      // ... other app settings
    }
    ```

*   **Environment Variables:**
    *   `USERS`: Provide user data as a JSON string directly via this environment variable. Ensure proper escaping for the JSON string.
        ```bash
        USERS='{"Users":[{"email":"test@example.com","given_name":"Test"}]}'
        ```
    *   `USERS_BASE64`: Provide user data as a base64 encoded JSON string via this environment variable. This is useful for passing complex JSON structures securely or when direct JSON string escaping is problematic.
        ```bash
        # Example: echo '{"Users":[{"email":"test@example.com","given_name":"Test"}]}' | base64
        USERS_BASE64='eyJVc2VycyI6W3siZW1haWwiOiJ0ZXN0QGV4YW1wbGUuY29tIiwiZ2l2ZW5fbmFtZSI6IlRlc3QifV19'
        ```

    User definitions are loaded in a specific order: `appsettings.json` (and `appsettings.Development.json`), then `users.json`, and finally environment variables (`USERS` and `USERS_BASE64`). Definitions from later sources will merge with or override users from earlier sources. If a user with the same `email` exists in multiple sources, the latest definition takes precedence.

### Access Token Claims

The `AccessTokenClaims` setting in your `appsettings.json` (or `appsettings.Development.json`) controls which claims from the user's profile are included in the generated access tokens.

*   By default, `AccessTokenClaims` includes `sub` and `roles`.
*   You can customize this array to include any other properties defined for your users (e.g., `email`, `given_name`, `family_name`).

Example `appsettings.json` snippet:
```json
{
  "AccessTokenClaims": ["email", "given_name", "roles"]
}
```

### Force HTTPS Scheme

The `ForceHttps` setting can be configured in `appsettings.json` (or `appsettings.Development.json`).

*   Set `ForceHttps` to `true` to force the request scheme to HTTPS. This is particularly useful when running MockAuth behind a proxy or load balancer that handles SSL termination but doesn't correctly propagate the original scheme (e.g., via `X-Forwarded-Proto` headers).

Example `appsettings.json` snippet:
```json
{
  "ForceHttps": true
}
```

Why Use MockAuth?
-----------------

Developing and testing applications that rely on OIDC authentication can often be cumbersome:

*   Setting up and managing a full-fledged identity provider (like Keycloak, Auth0, Okta, etc.) is frequently overkill and time-consuming for simple testing and local development scenarios.
*   The overhead of managing numerous test users and their credentials can complicate development workflows.
*   Dealing with client registration and secrets can add unnecessary complexity to test automation setups.

MockAuth provides a straightforward and lightweight alternative. Simply run the service, configure your desired test users, and point your application’s OIDC configuration towards MockAuth to streamline your development and testing processes.
