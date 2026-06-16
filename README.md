# BestEvents
BestEvents - учебный проект по созданию сервиса регистрации и изменения событий.
## Технологии
.Net9
ASP.Net
Docker
PostgreSQL
## Предварительные требования
* [.NET SDK 9.0](https://dotnet.microsoft.com/download)
* Visual Studio 2022 / VS Code
* Docker
## Установка и запуск
1. Клонируйте репозиторий
2. Перейдите в папку проекта bestevents
3. Запустите docker и в терминале выполните команду docker compose up -d 
4. В терминале выполните команду dotnet run
5. В браузере откройте "https://localhost:7046/swagger" или "http://localhost:5038/swagger" для просмотра API (Swagger/OpenAPI).
6. Для запуска тестов перейдите в папку src и выполните команду dotnet test

## Архитектура
Выделены четыре слоя:
* Domain - содержит сущности
* Application - содержит сервисы
* Infrastructure - содержит репозитории
* Presentation - содержит API

## API
Сущности:

Событие. 
Содержит параметры:
 - id (String / GUID) идентификатор события
 - title (String) название мероприятия.
 - description (String) детальное описание или дополнительная информация.
 - startAt (String / Date) дата начала. Формат: YYYY-MM-DD (ISO 8601).
 - endAt (String / Date) дата окончания. Формат: YYYY-MM-DD (ISO 8601).
 - totalSeats (Integer) общее количество мест.
 - availableSeats (Integer) доступное количество доступных мест.

Бронирование.
Содержит параметры:
 - id (String / GUID) идентификатор бронирования
 - eventId (String / GUID) идентификатор события
 - status (String) статус бронирования (pending - в ожидании, confirmed - подтверждена, rejected - отклонена)
 - createdAt (String / Date) дата начала. Формат: YYYY-MM-DD (ISO 8601).
 - processedAt (String / Date) дата окончания. Формат: YYYY-MM-DD (ISO 8601).

Эндпоинты:

POST /events создает новое событие. При вызове id и availableSeats не передаются. У нового события Id генерируется сервисом, а availableSeats устанавливается равным totalSeats. 

GET /events - возвращает события, фильтруя их по параметрам name, from, to и выполняя пагинацию по page и pageSIze.
Все параметры необязательны. По умолчанию используется пагинация page = 1 и pageSize = 10
Пример строки запроса:'https://localhost:7046/events?title=title&from=2026-01-01&to=2026-02-01&page=1&pageSize=10'

GET /events/{id} - возвращает событие по идентификатору

PUT /events/{id} перезаписывает существующее событие по идентификатору 

DELETE /events/{id} удаляет событие по идентификатору

POST /events/{id}/book делает попытку создания нового бронирования на событие с идентификатором id. Если параметр availableSeats события больше нуля, создается бронирование со статусом pending и параметром processedAt равным null. Возвращает URL эндпоинта для проверки статуса события /booking/{id}. Далее бронирование обрабатывается фоновым сервисом. Если availableSeats равен нулю, событие не найдено или закончилось, бронирование не создается, возвращается код 409.

GET /booking/{id} возвращает информацию о бронировании бронирования по его идентификатору

База данных:

Для хранения данных используется PostgreSQL в docker-контейнере.
Схема базы данных включает две таблицы events и bookings для сущностей событий и бронирования соответственно. Таблицы events и bookings имеют связь один ко многим через FK EventId в таблице bookings. Для воссоздания начальной схемы базы данных используйте миграцию InitialCreate.
Для тестирования работы репозиториев с базой данных используются интеграционные тесты на основе Testcontainers
Для создания миграции используйте команду dotnet ef migrations add SomeMigration --project BestEvents.Infrastructure --startup-project BestEvents.Presentation 