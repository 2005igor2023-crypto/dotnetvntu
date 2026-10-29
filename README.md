# Практичні роботи з дисципліни «Розробка проєктів засобами платформи .NET»

**Вінницький національний технічний університет (ВНТУ)**  
**Факультет інформаційних технологій та комп'ютерної інженерії (ФІТКІ)**  
**Кафедра програмного забезпечення (ПЗ)**  
**Студент:** Слободян Ігор, група 2ПІ-24б  
**Варіант:** №1  

---

## Структура рішень та репозиторію

Репозиторій організовано відповідно до вимог курсу та містить усі практичні роботи (Модуль 1 та Модуль 2):

```text
DOTNET/
├── DotNetLabs.sln                           # Головне рішення (Solution) з усіма проєктами
├── .gitignore                               # Ігнорування бінарних та тимчасових файлів
├── README.md                                # Загальний опис робіт та інструкції запуску
│
├── 01-csharp-basics/                        # Практична робота №1 (Базові конструкції C#)
│   ├── 01-csharp-basics.csproj
│   ├── Program.cs                           # Статистика результатів тестування
│   ├── REPORT_LAB01.docx                    # Звіт у форматі Word (Times New Roman 14)
│   └── REPORT_LAB01.md                      # Звіт у форматі Markdown
│
├── 02-oop-exceptions/                       # Практична робота №2 (ООП та винятки)
│   ├── 02-oop-exceptions.csproj
│   ├── Models/ (Shape, Circle, Rectangle)   # Поліморфізм, IComparable<Shape>
│   ├── Exceptions/                          # Власний виняток InvalidShapeDimensionException
│   ├── Program.cs
│   ├── REPORT_LAB02.docx
│   └── REPORT_LAB02.md
│
├── 03-generics-delegates-events/             # Практична робота №3 (Generics, Делегати, Події)
│   ├── 03-generics-delegates-events.csproj
│   ├── Collections/ (Cache<TKey, TValue>)   # Узагальнений кеш із витісненням
│   ├── Events/ (ItemRemovedEventArgs)       # Подія вилучення найстарішого запису
│   ├── Program.cs                           # Кеш валют, Func/Action, замикання, лямбди
│   ├── REPORT_LAB03.docx
│   └── REPORT_LAB03.md
│
├── 04-linq-to-objects/                      # Практична робота №4 (LINQ to Objects та Async)
│   ├── 04-linq-to-objects.csproj
│   ├── Program.cs                           # Task.WhenAll, Stopwatch, MinBy, Join, CSV
│   ├── REPORT_LAB04.docx
│   └── REPORT_LAB04.md
│
├── 05-aspnetcore-efcore-sqlite-crud/        # Практична робота №5 (CRUD Web API + EF Core + SQLite)
│   ├── 05-aspnetcore-efcore-sqlite-crud.csproj
│   ├── Models/ (Author, Book)               # Сутності домену «Бібліотека»
│   ├── Data/ (LibraryDbContext)
│   ├── Controllers/ (BooksController)
│   ├── REPORT_LAB05.docx
│   └── REPORT_LAB05.md
│
├── 06-rest-api/                             # Практична робота №6 (RESTful API, DTO, валідація)
│   ├── 06-rest-api.csproj
│   ├── DTOs/ (BookDtos, AuthorDtos, ReaderDtos, PagedResult)
│   ├── Middleware/ (ExceptionHandlingMiddleware - ProblemDetails)
│   ├── Controllers/ (Books, Authors, Readers)
│   ├── REPORT_LAB06.docx
│   └── REPORT_LAB06.md
│
├── 07-auth-jwt/                             # Практична робота №7 (JWT автентифікація та авторизація)
│   ├── 07-auth-jwt.csproj
│   ├── Services/ (PasswordHasher, JwtTokenService)
│   ├── Controllers/ (AuthController, BooksController з ролями Admin/User)
│   ├── REPORT_LAB07.docx
│   └── REPORT_LAB07.md
│
└── 08-architecture-testing/                 # Практична робота №8 (Clean Architecture & xUnit/Moq)
    ├── 08-architecture-testing.csproj
    ├── Domain/ (Entities, Interfaces, Exceptions)
    ├── Application/ (BookService, DTOs)
    ├── Infrastructure/ (DbContext, Repositories, UnitOfWork)
    ├── Controllers/ (BooksController)
    ├── tests/LibraryApi.UnitTests/          # 13 модульних та інтеграційних тестів
    ├── REPORT_LAB08.docx
    └── REPORT_LAB08.md
```

---

## Тематика та реалізований функціонал робіт (Варіант 1)

### Модуль 1. Консольні застосунки C#/.NET
- **ПР №1: «Базові конструкції C#. Консольний застосунок»**
  - Тема: Статистика результатів тестування (`int[] scores`, від 0 до 100 балів).
  - Динамічне розширення масиву через `Array.Resize`, обчислення середнього, пошук мінімуму та максимуму з фіксацією індексів, розподіл за категоріями, коректна обробка порожнього масиву («Немає даних»).
- **ПР №2: «ООП та обробка виключень у консольному застосунку»**
  - Тема: Геометричні фігури.
  - Абстрактний базовий клас `Shape`, похідні класи `Circle` та `Rectangle`.
  - Власний виняток `InvalidShapeDimensionException` при спробі створити фігуру з недопустимими розмірами.
  - Реалізація інтерфейсу `IComparable<Shape>` для сортування за площею.
- **ПР №3: «Узагальнення, делегати, події та лямбда-вирази»**
  - Тема: Кеш курсів валют (`Cache<TKey, TValue> where TKey : notnull`).
  - Фіксована місткість із витісненням найстаріших записів при переповненні.
  - Делегати `Func<TKey, TValue>` (метод завантаження `LoadRate`), `Action<TKey, TValue>` (обхід через `ForEach`).
  - Подія `ItemRemoved` з власним класом `ItemRemovedEventArgs<TKey, TValue>` та двома підписниками (іменований метод та лямбда-вираз).
  - Замикання у лямбда-виразі для фільтрації за порогом курсу. Демонстрація другого екземпляра `Cache<int, string>`.
- **ПР №4: «LINQ to Objects та асинхронне програмування»**
  - Тема: Порівняння цін в інтернет-магазинах (агрегатор пропозицій).
  - Асинхронне одночасне завантаження через `Task.WhenAll` та послідовне завантаження із заміром часу через `Stopwatch`.
  - Симуляція збою одного з магазинів з перехопленням `HttpRequestException` та ізоляцією помилки.
  - Запити LINQ: найдешевша пропозиція кожного товару (`MinBy`), статистика категорій, фільтрація за діапазоном (`OrderBy` + `ThenBy`), зіставлення зі списком бажань (`Join`), експорт звіту в CSV.

### Модуль 2. Наскрізний Web API мікропроєкт (Домен «Бібліотека»)
- **ПР №5: «Основи ASP.NET Core, EF Core та SQLite: перший CRUD Web API»**
  - Сутності `Book` та `Author` зі зв'язком «один-до-багатьох».
  - Міграції Entity Framework Core, підключення до файлової бази SQLite, початковий посів даних (Seed).
  - Повний асинхронний CRUD (GET, POST, PUT, DELETE), підключення Scalar UI для тестування API.
- **ПР №6: «Проєктування та розробка REST API»**
  - Розширення моделі: додано сутності `Reader` (Читач) та `BookLoan` (Видача книги).
  - Розподіл на DTO (Request/Response) з валідацією через Data Annotations.
  - Підтримка пагінації (`page`, `pageSize`), фільтрації та сортування.
  - Глобальна обробка помилок через `ExceptionHandlingMiddleware` за стандартом RFC 7807 (ProblemDetails).
- **ПР №7: «Автентифікація та авторизація в ASP.NET Core»**
  - Сутність `User`, реєстрація та авторизація через JWT-токени (`JwtBearer`).
  - Безпечне хешування паролів за алгоритмом PBKDF2 HMAC-SHA256.
  - Рольова авторизація (RBAC): ролі `Admin` та `User`. Захист операцій модифікації даних атрибутом `[Authorize(Roles = "Admin")]`.
- **ПР №8: «Архітектурні патерни та тестування .NET-застосунку»**
  - Організація за принципами Clean Architecture: шари `Domain`, `Application`, `Infrastructure`, `Api`.
  - Патерни `Repository` та `Unit of Work`.
  - Сервісний шар `BookService` з інкапсуляцією бізнес-правил.
  - Набір із 13 тестів (8 модульних тестів на базі xUnit + Moq та 5 інтеграційних тестів через `WebApplicationFactory`).

---

## Запуск проєктів

```bash
# Запуск консольних робіт (ПР 1-4):
dotnet run --project 01-csharp-basics
dotnet run --project 02-oop-exceptions
dotnet run --project 03-generics-delegates-events
dotnet run --project 04-linq-to-objects

# Запуск Web API проєктів (ПР 5-8):
dotnet run --project 05-aspnetcore-efcore-sqlite-crud
dotnet run --project 06-rest-api
dotnet run --project 07-auth-jwt
dotnet run --project 08-architecture-testing

# Запуск тестів (ПР №8):
dotnet test 08-architecture-testing/tests/LibraryApi.UnitTests
```

---

## Звіти до практичних робіт

До кожної практичної роботи підготовлено звіт у двох форматах:
1. Microsoft Word (`.docx`) - оформлений відповідно до стандартів (шрифт Times New Roman 14 pt, полуторний інтервал, чорно-біле оформлення без декоративних кольорів, таблиці з чіткими межами).
2. Markdown (`.md`) - для перегляду у веб-інтерфейсі репозиторію.

| № ПР | Назва роботи | Звіт Word (.docx) | Звіт Markdown (.md) |
|---|---|---|---|
| 1 | Базові конструкції C#. Консольний застосунок | [REPORT_LAB01.docx](01-csharp-basics/REPORT_LAB01.docx) | [REPORT_LAB01.md](01-csharp-basics/REPORT_LAB01.md) |
| 2 | ООП та обробка виключень у консольному застосунку | [REPORT_LAB02.docx](02-oop-exceptions/REPORT_LAB02.docx) | [REPORT_LAB02.md](02-oop-exceptions/REPORT_LAB02.md) |
| 3 | Узагальнення, делегати, події та лямбда-вирази | [REPORT_LAB03.docx](03-generics-delegates-events/REPORT_LAB03.docx) | [REPORT_LAB03.md](03-generics-delegates-events/REPORT_LAB03.md) |
| 4 | LINQ to Objects та асинхронне програмування | [REPORT_LAB04.docx](04-linq-to-objects/REPORT_LAB04.docx) | [REPORT_LAB04.md](04-linq-to-objects/REPORT_LAB04.md) |
| 5 | Основи ASP.NET Core, EF Core та SQLite | [REPORT_LAB05.docx](05-aspnetcore-efcore-sqlite-crud/REPORT_LAB05.docx) | [REPORT_LAB05.md](05-aspnetcore-efcore-sqlite-crud/REPORT_LAB05.md) |
| 6 | Проєктування та розробка REST API | [REPORT_LAB06.docx](06-rest-api/REPORT_LAB06.docx) | [REPORT_LAB06.md](06-rest-api/REPORT_LAB06.md) |
| 7 | Автентифікація та авторизація в ASP.NET Core (JWT) | [REPORT_LAB07.docx](07-auth-jwt/REPORT_LAB07.docx) | [REPORT_LAB07.md](07-auth-jwt/REPORT_LAB07.md) |
| 8 | Архітектурні патерни та тестування (xUnit + Moq) | [REPORT_LAB08.docx](08-architecture-testing/REPORT_LAB08.docx) | [REPORT_LAB08.md](08-architecture-testing/REPORT_LAB08.md) |
