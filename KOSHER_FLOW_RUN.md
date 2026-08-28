# Запуск проверки кошерности

Запускайте сценарий из корня репозитория `MeetingFlow`, чтобы не перепутать его с проектом `MeetingFlow.ClientServer`.

Требуется .NET SDK версии 10.x. Проверьте установленную версию и восстановите зависимости монолита:

```powershell
dotnet --version
dotnet restore .\MeetingFlow.Monolith\MeetingFlow.Monolith.csproj
```

Запустите сервер и клиентскую часть Razor Pages одним процессом:

```powershell
dotnet run --project .\MeetingFlow.Monolith\MeetingFlow.Monolith.csproj
```

Откройте сценарий в браузере:

<http://localhost:5000/KosherCheck>

Серверная и клиентская части Razor Pages стартуют вместе в приложении `MeetingFlow.Monolith`. Без настроенной модели страница всё равно откроется, но при проверке покажет сообщение о недоступности. Чтобы проверка выполнялась, настройте параметры модели согласно README монолита.

Для остановки процесса нажмите `Ctrl+C` в окне терминала.
