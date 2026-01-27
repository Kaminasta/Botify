# Botify 🚀

Лёгкая C# библиотека для удобной работы с Telegram-ботами с полной интеграцией в Dependency Injection.
Позволяет быстро и удобно создавать ботов с обработкой команд, сообщений и callback-запросов.

## ⚡ Особенности

- Полная поддержка **Dependency Injection** через `Microsoft.Extensions.DependencyInjection`.
- Простое добавление обработчиков **команд**, **сообщений** и **callback-запросов**.
- Встроенная работа с **Telegram.Bot API**.
- Кеширование данных и удобная работа с пользовательскими профилями.
- Возможность интеграции с любыми базами данных через DbContext (например, PostgreSQL + EF Core).

---

## 📦 Установка

Через NuGet:

```bash
dotnet add package Botify --source <директория, где находится скачанный релиз Botify>
```

---

## 🛠 Пример использования

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

## 📝 Основные концепции

| Концепт | Описание |
|----------|----------|
| **CommandHandler** | Обработка команд, например `/start`, `/schedule`. |
| **MessageHandler** | Обработка текстовых сообщений от пользователей. |
| **CallbackHandler** | Обработка callback-запросов от inline-клавиатур. |
| **BotifyOptions** | Настройки бота (токен, start char command) |

---

## 🔧 Примеры обработчиков

### Callbacks

```csharp
[CallbackHandler]
public class CallbacksHandler
{
    [Callback(Callbacks.Change)]
    public async Task ChangeCallback(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        // Обработка callback Callbacks.Change
    }
}
```
### Commands

```csharp
[CommandHandler]
public class CommandsHandler
{
    [Callback("start", "Описание комманды")]
    public async Task StartCommand(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        // Обработка command start
    }
}
```

### Message

```csharp
[MessageHandler]
public class MessagesHandler
{
    // Regex
    [Message("привет|hi")] 
    public async Task HelloMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        // Обработка сообщений "привет" и "hi"
    }

    [Message(".*")]
    public async Task AllMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        // Обработка всех остальных сообщений
    }
}
```

## 📖 Пример интеграции с базой данных

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));
```

---

## 🔗 Ссылки

- GitHub: [https://github.com/Kaminasta/Botify](https://github.com/Kaminasta/Botify)
