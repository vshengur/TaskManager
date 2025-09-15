# TaskManager (мини-версия Jira)

**Стек**: .NET 8, ASP.NET Core, CQRS (MediatR), EF Core (PostgreSQL), JWT‑аутентификация, Swagger, DI, Docker Compose.  
Бонус: модульные тесты на xUnit для бизнес-правил.

## Запуск локально

1. Убедитесь, что установлен .NET 8 SDK.
2. Запустите Postgres через Docker (рекомендуется):  
   ```bash
   docker compose up -d db
   ```
3. При необходимости настройте строку подключения в `src/TaskManager.Api/appsettings.json`.
4. Запустите API:
   ```bash
   dotnet build
   dotnet run --project src/TaskManager.Api
   ```
   По умолчанию API будет доступно на `http://localhost:5177` (Dev‑профиль). Swagger — на `/swagger`.

> Для удобства при старте используется `EnsureCreated()` — таблицы создаются автоматически.  
> Для прод-сценариев переключитесь на миграции: замените `EnsureCreated()` на `Migrate()` и выполните:
> ```bash
> dotnet tool install --global dotnet-ef
> dotnet ef migrations add Initial --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api
> dotnet ef database update --project src/TaskManager.Infrastructure --startup-project src/TaskManager.Api
> ```

## Запуск всего через Docker

```bash
docker compose up --build
```
- API: `http://localhost:8080/swagger`(локальный запуск)
- API: `http://localhost:53089/swagger (docker compose запуск)
- БД: Postgres 16 на `localhost:5432` (user: postgres / pass: postgres)

## Аутентификация (JWT)
Хранения пользователей нет. Получение JWT‑токена:
```
POST /api/auth/login
{ "userName": "alice" }
```
Используйте полученный `accessToken` в заголовке `Authorization: Bearer <token>` для всех эндпоинтов `/api/tasks/*`.

### Swagger — авторизация
Откройте Swagger UI и нажмите **Authorize** (справа вверху). Введите значение:
```
Bearer <ваш JWT токен>
```
Получить токен можно через `POST /api/auth/login`.

## Обзор API

- `GET /api/tasks` — список с опциональными фильтрами `assignee`, `status`, `priority`
- `GET /api/tasks/{id}` — получить по идентификатору
- `POST /api/tasks` — создать задачу
- `PUT /api/tasks/{id}` — обновить задачу
- `DELETE /api/tasks/{id}` — удалить задачу (запрещено, если у задачи есть подзадачи)
- `POST /api/tasks/{id}/relations/{relatedId}` — добавить симметричную связь “related to”
- `DELETE /api/tasks/{id}/relations/{relatedId}` — удалить связь в обе стороны

### Модель задачи
```
Id, Title, Description, Author, Assignee, Status(New|InProgress|Done), Priority(Low|Medium|High), ParentTaskId
```
- **Подзадачи**: установите `ParentTaskId` на `Id` родительской задачи.
- **Связи**: симметричная self‑relation; нельзя связывать задачу саму с собой; дубликаты игнорируются.

## Тесты
`tests/TaskManager.Tests` содержит несколько модульных тестов xUnit по базовым бизнес‑правилам уровня Application.

## Чистая архитектура (Clean Architecture)
- **Domain**: только сущности и enum’ы.
- **Application**: CQRS‑команды/запросы (MediatR), DTO, маппинг, абстракция `IApplicationDbContext`.
- **Infrastructure**: EF Core `AppDbContext`, регистрация в DI.
- **Api**: контроллеры, настройка Program, Swagger и JWT.

## Примечания
- Чтобы перейти на **MSSQL**, замените провайдер Npgsql и строку подключения в проектах Infrastructure и Api.
- Для краткости валидации/пагинация сделаны минимально — их легко расширить.
