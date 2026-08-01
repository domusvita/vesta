# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Vesta is a family app monorepo with two main components:

- **`src/api`** — .NET Azure Functions (Isolated Worker model) using clean architecture
- **`src/web`** — React 19 + TypeScript SPA (Vite)
- **`src/shared`** — currently unused (empty)

## Build & Run Commands

**Web** (run from `src/web`):
```
npm run dev          # dev server
npm run build         # tsc -b && vite build
npm run lint          # oxlint
npm run preview       # preview production build
```

**API** (run from `src/api`):
```
dotnet build          # build solution (Vesta.slnx)
dotnet run --project Vesta.Functions   # run Azure Functions locally
```

**EF Core migrations** (run from `src/api/Vesta.Infrastructure`, targeting `Vesta.Functions` as startup project):
```
dotnet ef migrations add <Name> --startup-project ../Vesta.Functions
dotnet ef database update --startup-project ../Vesta.Functions
```

No test projects exist yet.

## API Architecture (`src/api`)

Clean architecture, strictly layered — dependencies only point downward:

```
Vesta.Domain         → Entities only (no dependencies)
Vesta.Application    → Services, DTOs, Interfaces, Mappers (depends on Domain)
Vesta.Infrastructure → EF Core + PostgreSQL (depends on Application/Domain)
Vesta.Functions      → Azure Functions HTTP triggers, middleware (depends on all)
```

`Vesta.slnx` only includes these four projects. **`Vesta.Api` is a leftover ASP.NET Core Web API scaffold (controllers, `WeatherForecast`) that is not part of the solution and is not the real entry point** — the actual HTTP surface is `Vesta.Functions`. Don't build features there.

### Domain model

Core entities (`Vesta.Domain/Entities/`): `UserEntity`, `UserRoleEntity`, `FamilyEntity`, `FamilyMemberEntity`, `InviteCodeEntity`, `QuestionEntity`, `DailyQuestionEntity`, `AnswerEntity`, `CommentEntity`, `NotificationEntity`.

A `DailyQuestion` links a `Family` to a `Question` for a specific date; users submit `Answer`s and `Comment`s against it.

### Key conventions

- **HTTP responses** always use `FunctionResponse<T>` (`Vesta.Functions/Models`) — a wrapper with `bool Success` and `T? Data`. Return `new FunctionResponse<T>(true, dto)` on success and `new FunctionResponse<T>(false)` on failure.

- **Authentication** is handled globally by `JwtMiddleware` (`Vesta.Functions/Middleware`). It validates the Auth0 JWT against the tenant's OpenID Connect config and stores the `ClaimsPrincipal` at `context.Items["User"]`. It also short-circuits `OPTIONS` requests and adds CORS headers itself (currently hardcoded to `http://localhost:5173`). Functions retrieve the Auth0 user ID with:
  ```csharp
  var user = context.Items["User"] as ClaimsPrincipal;
  var auth0Id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  ```

- **Services** depend on `IVestaDbContext`, not the concrete `VestaDbContext`. Follow the same pattern for new services (see `IUserService`/`UserService`).

- **Mappers** are static extension methods on entity types (e.g., `user.ToDto()`), kept in `Vesta.Application/Mappers/`.

- **EF entity configuration** goes in separate `IEntityTypeConfiguration<T>` classes in `Vesta.Infrastructure/Persistence/Configurations/`. PostgreSQL `timestamptz` is used for all `DateTime` columns.

- **DI registration** follows the `AddVesta{Layer}()` extension pattern (e.g., `AddVestaApplication()`, `AddVestaInfrastructure()`, `AddVestaFunctions()`). New services should be registered in their layer's `ServiceCollectionExtensions`. Note the extension class in `Vesta.Application` is named `ServicesCollectionExtensions` (extra "s") — an existing inconsistency, not a typo to silently rename elsewhere.

- **Secrets** in production come from Azure Key Vault (configured via the `KeyVaultUrl` app setting, wired up in `Vesta.Functions/Program.cs`). Local secrets go in `local.settings.json` (gitignored) or user-secrets.

- **Primary constructors** are used for function classes that follow the completed `UserFunctions` pattern (constructor-injected services, `GetAuth0Id`/`CreateJsonResponse` helpers). Older stubs (`FamilyFunctions`, `QuestionFunctions`, `AnswerFunctions`) still use field-based DI and placeholder bodies — convert to the `UserFunctions` pattern when implementing them for real.

## Web Architecture (`src/web`)

- **Auth** uses `@auth0/auth0-react`. The `Auth0Provider` is set up in `main.tsx`; environment variables are `VITE_AUTH0_DOMAIN`, `VITE_AUTH0_CLIENT_ID`, `VITE_AUTH0_AUDIENCE` (from `.env.local`).

- **UI components** come from `@radix-ui/themes`. The `<Theme>` wrapper in `App.tsx` sets `appearance="light"`, `accentColor="indigo"`, `radius="medium"`.

- **Routing** uses React Router v7 (`BrowserRouter`). `/setup` (`ProfileSetupPage`) is reachable pre-`MainLayout`; everything else is nested under `ProtectedRoute` → `MainLayout` via `<Outlet />`. Pages live in `src/pages/`, layouts in `src/layouts/`, shared components in `src/components/`.

- **Current-user bootstrap**: `useCurrentUser()` (`src/hooks/`) runs once at the top of `AppRoutes` in `App.tsx` and populates `userStore`; `ProtectedRoute` and `MainLayout` read from that Zustand store rather than calling Auth0/the API directly.

- **State management** uses Zustand (`src/store/`). Forms use React Hook Form + Zod for validation.

- **HTTP** calls use Axios (`src/services/apiClient.ts` for the shared client, per-domain service modules like `userService.ts`). The API base URL is `VITE_API_BASE_URL` from `.env.local`.

- **Linter** is oxlint (not ESLint). Config is in `.oxlintrc.json`. Plugins: `react`, `typescript`, `oxc`. Run with `npm run lint` from `src/web`.

- **Feature code** goes in `src/features/` (currently empty — new feature areas should be created here rather than growing `src/pages`/`src/components` indefinitely).
