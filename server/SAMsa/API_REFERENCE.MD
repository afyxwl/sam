# API Reference — SAMsa (бекенд)

Цей файл містить опис REST API для сервера SAMsa — простої соціальної мережі про фільми/серіали.
Всі приклади показані для базової адреси:

`http://localhost:5005`

## Загальні зауваги
- Сервер використовує InMemory базу даних (дані втрачаються при перезапуску).
- Для TMDB ендпоінтів потрібен API-ключ TMDB, який потрібно вказати в конфігурації під ключем `Tmdb:ApiKey`.
  - Наприклад, у `appsettings.Development.json`:

```json
{
  "Tmdb": { "ApiKey": "YOUR_TMDB_API_KEY" }
}
```

- Всі тіла запитів повинні бути у форматі JSON.
- Дати повертаються у форматі UTC.

---

## Ендпоінти

### POSTS — робота з постами (огляди)

1) Отримати список постів

- Метод: GET
- URL: `/api/posts`
- Параметри: відсутні
- Відповідь: 200 OK
- Тіло відповіді: масив об'єктів Post

Приклад відповіді:

```json
[
  {
    "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "author": "Alice",
    "title": "Dune (2021)",
    "content": "Фантастичні візуальні рішення",
    "rating": 8,
    "createdAt": "2025-10-28T17:22:00Z",
    "tmdbId": 12345,
    "replies": [ /* масив reply */ ]
  }
]
```

2) Отримати один пост за Id

- Метод: GET
- URL: `/api/posts/{id}`
- Параметри: `id` — GUID поста
- Відповідь: 200 OK або 404 Not Found

3) Створити пост

- Метод: POST
- URL: `/api/posts`
- Тіло запиту (JSON):

```json
{
  "author": "Alice",
  "title": "Dune (2021)",
  "content": "Враження від фільму...",
  "rating": 8,
  "tmdbId": 12345
}
```

- Поля `author`, `title`, `content` — обов'язкові. `rating` та `tmdbId` — опціональні.
- Відповідь: 201 Created з тілом створеного об'єкта Post.

4) Оновити пост

- Метод: PUT
- URL: `/api/posts/{id}`
- Тіло запиту: повний об'єкт або лише поля для оновлення (сервер застосовує поля `title`, `content`, `rating`, `tmdbId`).
- Відповідь: 204 No Content або 404 Not Found

5) Видалити пост

- Метод: DELETE
- URL: `/api/posts/{id}`
- Відповідь: 204 No Content або 404 Not Found
- Примітка: видалення поста також видаляє прив'язані replies (cascade).

---

### REPLIES — відповіді на пост

1) Отримати всі відповіді для поста

- Метод: GET
- URL: `/api/posts/{postId}/replies`
- Параметр: `postId` — GUID поста
- Відповідь: 200 OK або 404 Not Found (якщо пост не знайдено)

Приклад відповіді:

```json
[
  {
    "id": "39d7e3fc-8c1a-4f4f-8ea1-57b0a9b2d3c8",
    "author": "Bob",
    "content": "Згоден з рецензією",
    "createdAt": "2025-10-28T18:00:00Z",
    "postId": "f47ac10b-58cc-4372-a567-0e02b2c3d479"
  }
]
```

2) Створити відповідь

- Метод: POST
- URL: `/api/posts/{postId}/replies`
- Тіло запиту:

```json
{
  "author": "Bob",
  "content": "Дякую за огляд!"
}
```

- Відповідь: 201 Created з тілом створеного Reply або 404 Not Found якщо пост не знайдено.

3) Видалити відповідь

- Метод: DELETE
- URL: `/api/posts/{postId}/replies/{id}`
- Відповідь: 204 No Content або 404 Not Found

---

### TMDB — проксі до TheMovieDB

> Примітка: для роботи цих ендпоінтів потрібно вказати TMDB API-ключ у конфігурації (`Tmdb:ApiKey`). Якщо ключ відсутній, сервер поверне 200 з тілом `null` або 400 — залежно від виклику.

1) Пошук (multi-search)

- Метод: GET
- URL: `/api/tmdb/search?q={query}&page={page}`
- Параметри:
  - `q` — строка пошуку (обов'язково)
  - `page` — номер сторінки (опційно, за замовчуванням 1)
- Відповідь: 200 OK з результатом, як його повертає TMDB (у форматі JSON).

Приклад:

`GET /api/tmdb/search?q=dune`

2) Деталі

- Метод: GET
- URL: `/api/tmdb/{mediaType}/{id}`
- Параметри:
  - `mediaType` — `movie` або `tv` (підтримуються ті, що TMDB дозволяє)
  - `id` — числовий id в TMDB
- Відповідь: 200 OK з деталями від TMDB або 404/помилка від TMDB (проксі).

---

## Приклади використання (curl)

Створити пост:

```bash
curl -X POST http://localhost:5005/api/posts \
  -H "Content-Type: application/json" \
  -d '{"author":"Alice","title":"Dune (2021)","content":"Фантастика!","rating":8}'
```

Додати відповідь до поста:

```bash
curl -X POST http://localhost:5005/api/posts/<POST_ID>/replies \
  -H "Content-Type: application/json" \
  -d '{"author":"Bob","content":"Погоджуюсь"}'
```

Пошук по TMDB:

```bash
curl "http://localhost:5005/api/tmdb/search?q=dune"
```

Отримати деталі фільму (TMDB movie id = 603 for The Matrix наприклад):

```bash
curl "http://localhost:5005/api/tmdb/movie/603"
```

---

## Статуси помилок та формати
- 200 — успіх (GET)
- 201 — створено (POST)
- 204 — успіх без тіла (PUT/DELETE)
- 400 — некоректний запит (наприклад, відсутній обов'язковий параметр)
- 404 — об'єкт не знайдено
- 500 — внутрішня помилка сервера

## Примітки для розробника
- Для продакшену замініть InMemory DB на постійну БД та додайте міграції.
- Авторизація поки що не реалізована — поле `author` є простим рядком. Додайте автентифікацію та таблицю користувачів при потребі.

---

Якщо хочете — можу додати OpenAPI/Swagger опис ендпоінтів як окремий файл JSON/YAML або розширити документацію прикладами запитів і детальними схемами об'єктів у форматі OpenAPI (Ukrainian).