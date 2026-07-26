# Copilot Instructions for Vesta

## Architecture Overview

Vesta is a family app monorepo with two main components:

- **`src/api`** — .NET Azure Functions (Isolated Worker model) using clean architecture
- **`src/web`** — React 19 + TypeScript SPA (Vite)
- **`src/shared`** — Currently unused

### API Layer Structure (`src/api`)

Clean architecture, strictly layered:

```
Vesta.Domain        → Entities only (no dependencies)
Vesta.Application   → Services, DTOs, Interfaces, Mappers (depends on Domain)
Vesta.Infrastructure → EF Core + PostgreSQL (depends on Application/Domain)
Vesta.Functions     → Azure Functions HTTP triggers, middleware (depends on all)
```

### Domain Model

Core entities: `UserEntity`, `FamilyEntity`, `FamilyMemberEntity`, `InviteCodeEntity`, `QuestionEntity`, `DailyQuestionEntity`, `AnswerEntity`, `CommentEntity`, `NotificationEntity`.

A `DailyQuestion` links a `Family` to a `Question` for a specific date; users submit `Answer`s and `Comment`s against it.

## Build Commands

**Web** (run from `src/web`):
```
npm run dev          # dev server
npm run build        # tsc + vite build
npm run lint         # oxlint
```

**API** (run from `src/api`):
```
dotnet build         # build solution
dotnet run --project Vesta.Functions   # run functions locally
```

No test projects exist yet.

## Key Conventions

### API

- **HTTP responses** always use `FunctionResponse<T>` — a wrapper with `bool Success` and `T? Data`. Return `new FunctionResponse<T>(true, dto)` on success and `new FunctionResponse<T>(false)` on failure.

- **Authentication** is handled globally by `JwtMiddleware`. It validates the Auth0 JWT and stores the `ClaimsPrincipal` at `context.Items["User"]`. Functions retrieve the Auth0 user ID with:
  ```csharp
  var user = context.Items["User"] as ClaimsPrincipal;
  var auth0Id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
  ```

- **Services** depend on `IVestaDbContext`, not the concrete `VestaDbContext`. Follow the same pattern for new services.

- **Mappers** are static extension methods on entity types (e.g., `user.ToDto()`), kept in `Vesta.Application/Mappers/`.

- **EF entity configuration** goes in separate `IEntityTypeConfiguration<T>` classes in `Vesta.Infrastructure/Persistence/Configurations/`. PostgreSQL `timestamptz` is used for all `DateTime` columns.

- **DI registration** follows the `AddVesta{Layer}()` extension pattern (e.g., `AddVestaApplication()`, `AddVestaInfrastructure()`). New services should be registered in their layer's `ServiceCollectionExtensions`.

- **Secrets** in production come from Azure Key Vault (configured via `KeyVaultUrl` app setting). Local secrets go in `local.settings.json` (gitignored).

- **Primary constructors** are used for function classes that follow the completed `UserFunctions` pattern. Older stubs use field-based DI — convert when implementing.

### Web

- **Auth** uses `@auth0/auth0-react`. The Auth0 provider is set up in `main.tsx`; environment variables are `VITE_AUTH0_DOMAIN`, `VITE_AUTH0_CLIENT_ID`, `VITE_AUTH0_AUDIENCE`.

- **UI components** come from `@radix-ui/themes`. The `<Theme>` wrapper in `App.tsx` sets `appearance="light"`, `accentColor="indigo"`, `radius="medium"`.

- **Routing** uses React Router v7. All routes are nested under `MainLayout` via `<Outlet />`. Pages live in `src/pages/`, layouts in `src/layouts/`.

- **State management** uses Zustand (`src/store/`). Forms use React Hook Form + Zod for validation.

- **HTTP** calls use Axios. The API base URL is `VITE_API_BASE_URL` from `.env.local`.

- **Linter** is oxlint (not ESLint). Config is in `.oxlintrc.json`. Plugins: `react`, `typescript`, `oxc`.

- **Feature code** goes in `src/features/` (currently empty — new feature areas should be created here).
