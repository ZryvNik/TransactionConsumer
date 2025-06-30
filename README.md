# Transaction Consumer API

Web API для создания и получения транзакций согласно техническому заданию.

## Функциональность

### POST /api/v1/Transaction
Создает новую транзакцию. Метод идемпотентный - при повторной отправке запроса с тем же Id возвращает результат предыдущего запроса.

**Запрос:**
```json
{
    "id": "1afa615f-af61-4d8a-b891-bc874c937772",
    "transactionDate": "2024-10-25T00:00:00+05:00",
    "amount": 12.34
}
```

**Ответ:**
```json
{
    "insertDateTime": "2024-10-25T12:03:34+05:00"
}
```

### GET /api/v1/Transaction?id={guid}
Получает транзакцию по Id.

**Ответ:**
```json
{
    "id": "1afa615f-af61-4d8a-b891-bc874c937772",
    "transactionDate": "2024-10-25T00:00:00+05:00",
    "amount": 12.34
}
```

## Валидация

- Сумма транзакции должна быть положительной
- Дата транзакции не может быть в будущем
- Ограничение на количество одновременно хранимых транзакций: 100 штук

## Обработка ошибок

Обработка ошибок реализована в соответствии с RFC 9457 (Problem Details for HTTP APIs).

### Типы ошибок

#### 1. Ошибка валидации (ArgumentException)
- Причина: сумма отрицательная или дата в будущем
- HTTP Status: 400
- Response:
```json
{
  "type": "<значение из appsettings.json>",
  "title": "Validation error",
  "status": 400,
  "detail": "The transaction amount must be positive",
  "instance": "/api/v1/Transaction"
}
```

#### 2. Ошибка бизнес-логики (InvalidOperationException)
- Причина: превышен лимит транзакций или транзакция не найдена
- HTTP Status: 400 (лимит) или 400/404 (не найдена)
- Response (лимит):
```json
{
  "type": "<значение из appsettings.json>",
  "title": "Business logic error",
  "status": 400,
  "detail": "Transaction limit exceeded. Max: 100",
  "instance": "/api/v1/Transaction"
}
```
- (TransactionNotFoundException)
- Response (не найдена):
```json
{
  "type": "<значение из appsettings.json>",
  "title": "Business logic error",
  "status": 404,
  "detail": "Transaction with Id {id} not found",
  "instance": "/api/v1/Transaction"
}
```

#### 3. Внутренняя ошибка сервера (Exception)
- Причина: любая непредвиденная ошибка
- HTTP Status: 500
- Response:
```json
{
  "type": "<значение из appsettings.json>",
  "title": "Internal server error",
  "status": 500,
  "detail": "An unexpected error occurred",
  "instance": "/api/v1/Transaction"
}
```

## Технологии

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL

## Запуск проекта

1. Убедитесь, что у вас установлен Docker и Docker Compose
2. Клонируйте репозиторий
3. В корневой папке проекта выполните:

```bash
docker-compose up --build
```

Приложение будет доступно по адресу: http://localhost:5000

## Swagger UI

Для тестирования API доступен Swagger UI:
http://localhost:5000/swagger

При запуске через Docker Compose автоматически создается контейнер с PostgreSQL:
- Хост: postgres (внутри Docker сети) или localhost (для внешних подключений)
- Порт: 5432
- База данных: transactiondb
- Пользователь: postgres
- Пароль: postgres
