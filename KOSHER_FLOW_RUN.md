# Запуск проверки кошерности

Запускайте сценарий из корня репозитория `MeetingFlow`, чтобы не перепутать его с проектом `MeetingFlow.ClientServer`.

Требуется .NET SDK версии 10.x. Проверьте установленную версию и восстановите зависимости монолита:

```powershell
dotnet --version
dotnet restore .\MeetingFlow.Monolith\MeetingFlow.Monolith.csproj
```

Запустите одно монолитное приложение Razor Pages одним процессом:

```powershell
dotnet run --project .\MeetingFlow.Monolith\MeetingFlow.Monolith.csproj
```

Откройте сценарий в браузере:

<http://localhost:5000/KosherCheck>

Интерфейс Razor Pages обслуживается тем же процессом `MeetingFlow.Monolith`; отдельный клиентский процесс не запускается. Если отсутствует `AiChat__ApiKey`, страница откроется, но корректная отправленная проверка получит HTTP 503 и общее сообщение о недоступности. Имя модели и адрес имеют значения по умолчанию, но ключ API обязателен. Настройте его согласно README монолита.

Для остановки процесса нажмите `Ctrl+C` в окне терминала.
