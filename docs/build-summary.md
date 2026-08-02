# Build Summary — Admin User Management

## What was built

An admin-only "Users" feature: a paginated grid of all users, a modal to view/edit a user's display name, date of birth, and avatar URL (with a live preview), an "Add New User" flow for profiles without an Auth0 login, and a "delete" action that revokes all of a user's roles (not a hard delete of the account/data).

### Backend (`src/api`, Azure Functions / Clean Architecture)

- **Fixed a pre-existing compile-blocking bug**: `Vesta.Functions/Middleware/JwtMiddleware.cs` was missing a closing brace on the `Invoke` method, and `Vesta.Functions/Program.cs` used `ConfigureFunctionsWorkerDefaults` alongside the ASP.NET Core HTTP integration package, which Azure Functions' analyzer rejects (`AZFW0014`). Both were fixed; the solution did not build before this work started.
- `UserEntity.Auth0Id` is now nullable, so an admin can create a user profile that has no Auth0 login yet (e.g. a family member without their own account).
- `UserDto` gained `dateOfBirth` and `avatarUrl` (previously stored on the entity but never exposed over the API).
- New `UserService` methods: `UpdateAsync`, `CreateByAdminAsync`, `RemoveAllRolesAsync`, all server-side validated (display name 2–100 chars, date of birth not in the future, avatar URL must be an absolute `http`/`https` URL).
- New Azure Functions, all Admin-role-gated using the existing `currentUser.Roles.Contains("Admin")` pattern:
  - `PUT /api/users/{id}` — update a user
  - `POST /api/users/admin` — create a user (no Auth0 identity attached)
  - `DELETE /api/users/{id}` — remove all role assignments from a user
- `FunctionResponse<T>` gained an optional `message` field, surfaced to the frontend on validation/not-found/self-lockout errors.
- New EF Core migration `MakeAuth0IdNullable`, applied to the local dev database as part of this work.
- New `Vesta.UnitTests` project (xUnit + FluentAssertions 7.x + Moq + EF Core InMemory), added to `Vesta.slnx`: **57/57 tests passing**, 100% line coverage on every class touched by this feature. See `report.md` at the repo root for full details and documented coverage gaps (`JwtMiddleware`'s live Auth0 network validation is intentionally excluded — not unit-testable without a fake identity provider).

### Frontend (`src/web`, React + TypeScript)

- `AdminUsersPage.tsx` now has real pagination (10/25/50/All), a row-click-to-edit modal, an "Add New User" button, and a per-row delete button with a confirmation dialog.
- New `UserFormModal` component (Radix `Dialog` + React Hook Form + Zod) shared by create and edit, with a live avatar preview that gracefully hides on a broken image URL.
- New `Pagination` component (nothing existed to reuse — Radix Themes has no built-in pagination primitive).
- `MainLayout.tsx` now shows an "Admin: Users" nav link, gated on `user?.roles.includes('Admin')` (same check used by the existing `AdminRoute` guard) — non-admins never see it.
- `userService.ts` gained `createUser`/`updateUser`/`deleteUser`, all surfacing the backend's `message` field on error.

## Key decisions

- **Admin-created users get a nullable `Auth0Id`** rather than adding an email + invite-linking flow. There's no email field anywhere in the schema today; adding one plus a real linking flow was judged out of scope for this feature. Confirmed with the user before implementing.
- **FluentAssertions pinned to 7.x**, not 8+, because v8+ requires a paid commercial license for for-profit use. Confirmed with the user before implementing.
- **Pagination is client-side** (fetch the full user list once via the existing `GET /api/users`, page through it in the UI) rather than adding server-side paging query params — simpler, lower risk, and appropriate for the expected user-base size.
- **Self-lockout guard**: an admin cannot remove their own roles via the delete action, preventing accidental lockout. (Note: this does not prevent two admins from de-roling each other in separate requests, leaving zero admins — flagged as an accepted residual risk, not fixed, since a "last admin" check was outside this feature's scope.)
- **Avatar URL is restricted to `http`/`https` schemes**, validated on both the server and client, since the value is later rendered as an `<img src>` — defense against `javascript:`/`data:` URI misuse.
- **"Delete" is role-removal, not row deletion.** The user's account and historical data (answers, comments, family memberships) are preserved; only their role tags are stripped. The UI's confirmation copy says this explicitly.

## How to run it

**Prerequisites**: Docker Postgres container `vesta-db` running on port 5432 (already running in this environment), `.NET user secrets` configured for `Vesta.Functions` with `ConnectionStrings:DefaultConnection` and `Auth0` settings, `src/web/.env.local` with `VITE_AUTH0_*` and `VITE_API_BASE_URL=/`.

1. Apply migrations (already done in this environment, but for a fresh DB):
   ```
   cd src/api
   dotnet ef database update --project Vesta.Infrastructure --startup-project Vesta.Functions
   ```
2. Start the Functions host **on port 7207** (matches the Vite dev proxy in `src/web/vite.config.ts`, which forwards `/api` to `http://localhost:7207`):
   ```
   cd src/api/Vesta.Functions
   func host start --port 7207
   ```
   (There's also a VS Code task and `launchSettings.json` profile already configured for this port.)
3. Start the frontend:
   ```
   cd src/web
   npm run dev
   ```
   then open `http://localhost:5173`.
4. Run the backend tests:
   ```
   cd src/api
   dotnet test Vesta.UnitTests --collect:"XPlat Code Coverage"
   ```

## Verification performed

- `dotnet build src/api/Vesta.slnx` — clean build, independently re-run (not just trusting the sub-agent report).
- `dotnet test src/api/Vesta.UnitTests` — 57/57 passing, independently re-run.
- `npm run lint` and `npm run build` in `src/web` — both pass.
- Applied the new migration to the real local dev Postgres database.
- Started the real Functions host (port 7207) and the real Vite dev server, and confirmed the full request path — browser → Vite proxy → Functions host → Postgres — works: all five user-management endpoints correctly return `401` without a valid token, proving both that the JwtMiddleware fix works and that the new routes are live and reachable through the proxy exactly as the frontend will call them.
- **Not performed**: an interactive browser walkthrough logging in as an admin via Auth0 and clicking through the grid/modal/delete flow. That requires real Auth0 user credentials, which aren't available in this environment and shouldn't be handled in an automated way. Everything below the login layer (server startup, DB connectivity, auth enforcement, routing, build, lint, and unit tests) has been verified directly; the actual logged-in UI interaction has not been.
