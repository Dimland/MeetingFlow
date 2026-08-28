# Подключение модели для проверки кошерности

Приложение может обращаться к одному из двух поставщиков: OpenAI или Groq. За один запуск выбирайте только один набор переменных. Ключ доступа — секрет: не сохраняйте его в `appsettings.json` и не добавляйте в Git.

Перед сменой поставщика откройте новый терминал либо заново задайте все переменные. Это не позволит случайно отправить запрос не тому поставщику.

## Вариант 1. OpenAI

1. Создайте ключ на [странице ключей OpenAI](https://platform.openai.com/api-keys).
2. Подробнее о начале работы — в [официальном руководстве OpenAI](https://developers.openai.com/api/docs/quickstart).
3. В PowerShell вставьте команды ниже. Вместо `<OPENAI_API_KEY>` укажите свой ключ, но не сохраняйте его в файлах проекта.

```powershell
$env:AiChat__Model = 'gpt-5-mini'
$env:AiChat__Endpoint = 'https://api.openai.com/v1'
$env:AiChat__ApiKey = '<OPENAI_API_KEY>'
dotnet run --project .\MeetingFlow.Monolith\MeetingFlow.Monolith.csproj
```

## Вариант 2. Groq

1. Создайте ключ на [странице ключей Groq](https://console.groq.com/keys).
2. У Groq есть бесплатный тариф, но на нём действуют ограничения. Проверьте их в [списке лимитов](https://console.groq.com/docs/rate-limits).
3. Модель `openai/gpt-oss-20b` поддерживает JSON Schema — это нужно приложению, чтобы получить ответ в ожидаемом формате.
4. Полезные официальные материалы: [быстрый старт](https://console.groq.com/docs/quickstart), [совместимость с OpenAI](https://console.groq.com/docs/openai), [описание модели openai/gpt-oss-20b](https://console.groq.com/docs/model/openai/gpt-oss-20b).
5. В PowerShell вставьте команды ниже. Вместо `<GROQ_API_KEY>` укажите свой ключ, но не сохраняйте его в файлах проекта.

```powershell
$env:AiChat__Model = 'openai/gpt-oss-20b'
$env:AiChat__Endpoint = 'https://api.groq.com/openai/v1'
$env:AiChat__ApiKey = '<GROQ_API_KEY>'
dotnet run --project .\MeetingFlow.Monolith\MeetingFlow.Monolith.csproj
```

## Если возникла ошибка

- `401` — проверьте, что ключ скопирован полностью, относится к выбранному поставщику и ещё действует.
- `429` — достигнут лимит запросов. Подождите или проверьте лимиты своего тарифа.
- Ошибка схемы — выберите модель с поддержкой JSON Schema. Для Groq используйте `openai/gpt-oss-20b` из команд выше.
