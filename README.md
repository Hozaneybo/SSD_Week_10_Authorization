# Authorization start

## Overview
This project implements a web API for a news site, enforcing access control policies based on user roles. The API ensures that only authorized users can perform specific actions, such as creating, editing, or deleting articles and comments. The solution follows the principle of least privilege and uses role-based authorization.

## Getting started

```sh
dotnet run
```
## Access the API:
<http://localhost:5171/swagger/index.html>

Set `useCookies` and `useSessionCookies` to true for login.


## Access Control Policies
The API enforces the following policies:

### Roles and Permissions
1. **Editor**:
   - Edit and delete articles.
   - Edit and delete comments.

2. **Writer/Journalist**:
   - Create and edit their own articles.

3. **Subscriber/Registered User**:
   - Comment on articles.

4. **Guest/Public User**:
   - Read articles.

---

## Seeded Data
The database is pre-seeded with the following users for testing:

| Email          | Password | Role       |
|----------------|----------|------------|
| `editor`       | `S3cret!`| Editor     |
| `writer`       | `S3cret!`| Writer     |
| `anotherWriter`| `S3cret!`| Writer     |
| `subscriber`   | `S3cret!`| Subscriber |

## Evaluation Notes
The API is designed to be easy to evaluate:
 - All endpoints are documented in Swagger UI.
 - Pre-seeded users allow immediate testing of all roles.
 - Clear error messages are provided for unauthorized actions.
