# BestEvents
BestEvents - учебный проект по созданию сервиса регистрации и изменения событий.
## Технологии
.Net9
ASP.Net
## Предварительные требования
* [.NET SDK 9.0](https://dotnet.microsoft.com/download)
* Visual Studio 2022 / VS Code
## Установка и запуск
1. Клонируйте репозиторий
2. Перейдите в папку проекта bestevents
3. Выполните команду dotnet run
4. В браузере откройте "https://localhost:7046/swagger" или "http://localhost:5038/swagger" для просмотра API (Swagger/OpenAPI).
5. Для запуска тестов перейдите в папку src и выполните команду dotnet test
## API
GET /events - возвращает события, фильтруя их по параметрам name, from, to и выполняя пагинацию по page и pageSIze.
Все параметры необязательны. По умолчанию используется пагинация page = 1 и pageSize = 10
Пример строки запроса:'https://localhost:7046/events?title=title&from=2026-01-01&to=2026-02-01&page=1&pageSize=10'

GET /events/{id} - возвращает событие по id

POST /events добавляет новое событие

PUT /events/{id} перезаписывает существующее событие по идентификатору

DELETE /events/{id} удаляет событие по идентификатору

POST /events/{id}/book создает бронирование на событие с идентификатором id

GET /booking/{id} возвращает статус бронирования по его идентификатору