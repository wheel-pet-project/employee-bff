# ExceptionHandlerMiddleware

## Описание

Промежуточное программное обеспечение (middleware) для обработки исключений в gRPC-запросах. Преобразует gRPC исключения
в соответствующие HTTP-ответы.

## Конструктор

```csharp
public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
```

### Параметры

- `logger`: Логгер для записи информации об исключениях

## Методы

### InvokeAsync

```csharp
public async Task InvokeAsync(HttpContext context, RequestDelegate next)
```

Обрабатывает HTTP-запрос и перехватывает возможные исключения.

#### Параметры

- `context`: Контекст HTTP-запроса
- `next`: Делегат для передачи управления следующему компоненту в конвейере

#### Обрабатываемые исключения

1. `RpcException` со статусом `Unavailable`:
    - Логирует предупреждение
    - Возвращает HTTP 502 Bad Gateway
    - Сообщение: "Service unavailable"

2. Другие `RpcException`:
    - Логирует предупреждение
    - Преобразует gRPC код статуса в соответствующий HTTP-статус
    - Возвращает оригинальное описание ошибки

3. Все остальные исключения:
    - Логирует критическую ошибку
    - Возвращает HTTP 500 Internal Server Error
    - Сообщение: "Internal server error"

### ChangeResponse (private)

```csharp
private async Task ChangeResponse(HttpContext context, int statusCode, string description)
```

Изменяет HTTP-ответ, устанавливая указанный код статуса и описание ошибки.

#### Параметры

- `context`: Контекст HTTP-запроса
- `statusCode`: HTTP-код статуса
- `description`: Описание ошибки

## RpcCodesToHttpTranslator

Вспомогательный класс для преобразования кодов состояния gRPC в HTTP-коды.

### Translate

```csharp
public int Translate(StatusCode statusCode)
```

Преобразует код состояния gRPC в соответствующий HTTP-код состояния.

#### Таблица соответствия кодов

| gRPC StatusCode    | HTTP Status Code          |
|--------------------|---------------------------|
| OK                 | 200 OK                    |
| Cancelled          | 500 Internal Server Error |
| Unknown            | 500 Internal Server Error |
| InvalidArgument    | 400 Bad Request           |
| DeadlineExceeded   | 504 Gateway Timeout       |
| NotFound           | 404 Not Found             |
| AlreadyExists      | 409 Conflict              |
| PermissionDenied   | 403 Forbidden             |
| ResourceExhausted  | 429 Too Many Requests     |
| FailedPrecondition | 412 Precondition Failed   |
| Aborted            | 500 Internal Server Error |
| OutOfRange         | 400 Bad Request           |
| Unimplemented      | 501 Not Implemented       |
| Internal           | 500 Internal Server Error |
| Unavailable        | 503 Service Unavailable   |
| DataLoss           | 500 Internal Server Error |
| Unauthenticated    | 401 Unauthorized          |
| Default            | 500 Internal Server Error | 