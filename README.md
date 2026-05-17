# Botify 🚀

Botify — лёгкий C#-фреймворк для создания Telegram-ботов с интеграцией в Dependency Injection и удобной моделью атрибутов. Он позволяет быстро собирать ботов с обработкой команд, сообщений, callback-запросов, inline-query и платежных событий.

## ⚡ Особенности

- Полная поддержка **Dependency Injection** через `Microsoft.Extensions.DependencyInjection`.
- Простое добавление обработчиков **команд**, **сообщений** и **callback-запросов**.
- Встроенная работа с **Telegram.Bot API**.
- Кеширование данных и удобная работа с пользовательскими профилями.
- Возможность интеграции с любыми базами данных через DbContext (например, PostgreSQL + EF Core).

---

## 📦 Установка

Через NuGet-источник проекта:

```bash
 dotnet add package Botify --source <папка_или_feed_с_Botify>
```

Если используется локальная сборка, можно подключить пакет из собственного feed или артефактов релиза.

---

## 🛠 Быстрый старт

```csharp
public class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        var botToken = builder.Configuration["Telegram:BotToken"]
            ?? throw new InvalidOperationException("BotToken не указан");

        builder.Services.AddBotifyHandlers();
        builder.Services.AddBotify(options => {
            options.SetToken(botToken);
        });

        var host = builder.Build();
        host.Run();
    }
}
```

---

## 🧱 Основные концепции

| Концепт | Описание |
|---|---|
| `CommandHandler` | Обработка команд вида `/start`, `/help`, `/schedule`. |
| `CallbackHandler` | Обработка `CallbackQuery` от inline-кнопок. |
| `MessageHandler` | Обработка входящих сообщений по regex-паттернам. |
| `InlineHandler` | Обработка inline-запросов Telegram. |
| `PaymentHandler` | Обработка платежных событий Telegram. |
| `BotifyOptionsBuilder` | Конфигурация бота: токен, разделители, логирование, HTTP-параметры. |

---

## 🔧 Примеры обработчиков

### Команды

```csharp
using Botify.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

[CommandHandler]
public class CommandsHandler
{
    [Command("start", "Начать работу с ботом")]
    public async Task StartCommand(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.Message == null)
            return;

        await client.SendMessage(update.Message.Chat.Id, "Привет! Я Botify-бот.", cancellationToken: ct);
    }
}
```

### Callback-запросы

```csharp
using Botify.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

[CallbackHandler]
public class CallbacksHandler
{
    [Callback("change")]
    public async Task ChangeCallback(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.CallbackQuery?.Message == null)
            return;

        await client.EditMessageText(
            update.CallbackQuery.Message.Chat.Id,
            update.CallbackQuery.Message.MessageId,
            "Изменено",
            cancellationToken: ct);
    }
}
```

### Сообщения

```csharp
using Botify.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

[MessageHandler]
public class MessagesHandler
{
    [Message("^(привет|hello)$", MessageType.Text)]
    public async Task HelloMessage(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.Message == null)
            return;

        await client.SendMessage(update.Message.Chat.Id, "Привет!", cancellationToken: ct);
    }

    [Message(".*")]
    public async Task FallbackMessage(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.Message == null)
            return;

        await client.SendMessage(update.Message.Chat.Id, "Неизвестное сообщение.", cancellationToken: ct);
    }
}
```

### Inline-запросы

```csharp
using Botify.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

[InlineHandler]
public class InlineSearchHandler
{
    [Inline("search")]
    public async Task Search(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        if (update.InlineQuery == null)
            return;

        // обработка inline query
    }
}
```

### Платежи

```csharp
using Botify.Attributes;
using Telegram.Bot;
using Telegram.Bot.Types;

[PaymentHandler]
public class PaymentsHandler
{
    [PreCheckoutPayment]
    public Task PreCheckout(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        // проверка заказа перед оплатой
        return Task.CompletedTask;
    }

    [SuccessfulPayment]
    public Task SuccessfulPayment(ITelegramBotClient client, Update update, CancellationToken ct)
    {
        // обработка успешной оплаты
        return Task.CompletedTask;
    }
}
```

---

## ⚙️ Конфигурация

`BotifyOptionsBuilder` позволяет настроить:

- токен бота;
- базовый URL Telegram API;
- символ начала команды;
- разделитель callback-данных;
- разделитель inline-данных;
- логирование;
- пользовательский `HttpClientHandler`.

Пример:

```csharp
builder.Services.AddBotify(options =>
{
    options.SetToken(botToken);
    options.SetBaseURL("https://api.telegram.org");
    options.UseLogger(logger, LogLevel.Information);
});
```

---

## 🗄 Работа с базой данных

Botify не ограничивает способ хранения данных. Можно использовать EF Core, Dapper или любой другой подход.

Пример с EF Core:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
```

---

## 🔗 Ссылки

- GitHub: [https://github.com/Kaminasta/Botify](https://github.com/Kaminasta/Botify)
- Documentation: [https://kaminasta.github.io/doc/botify/api/Botify](https://kaminasta.github.io/doc/botify/api/Botify)
