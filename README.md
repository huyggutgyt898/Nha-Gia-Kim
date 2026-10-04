# Nhà Giả Kim - Bookstore

ASP.NET Core 10 MVC application with a SQL Server-backed bookstore, storefront, checkout, and authenticated administration.

## Database

The default connection string targets the `localhost\SQL2025` SQL Server instance using Windows authentication. The application applies EF Core migrations at startup and creates the `NhaGiaKim` database if it does not exist. Override the connection string for another SQL Server instance using `ConnectionStrings__DefaultConnection` (or edit `ConnectionStrings:DefaultConnection` in `appsettings.json`). For example:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost\SQL2025;Database=NhaGiaKim;Trusted_Connection=True;TrustServerCertificate=True"
```

The database uses SQL Server and an EF Core schema equivalent to the requested bookstore tables: `Authors`, `Categories`, `Books`, `PressArticles`, `Feedbacks`, `Orders`, `OrderItems`, and `Users`. Existing storefront tables `Carts` and `CartItems` are retained for guest checkout. Orders also retain checkout fields such as email, order number, total, and item title snapshots.

The schema-alignment migration renames the existing reviews table and its columns, creates author/article/admin tables, converts each book's author text into an author relationship, and maps recognized legacy order statuses to `Đăng ký`, `Đã gửi`, `Đang giao`, or `Đã giao`. It preserves existing review, book, and order records. Review unrecognized order statuses or amounts outside `decimal(10,2)` before deploying: the migration intentionally fails rather than silently changing data to fit the new constraints.

Run the app with `dotnet run`; it listens at `http://localhost:5088` when using the included `http` launch profile. OpenAPI JSON is available at `/openapi/v1.json` in Development.

## Website

The home page presents “Nhà Giả Kim”, reader reviews, and related in-stock books. Visitors can submit a 1–5 star review with an optional display name; reviews are stored in SQL Server. Search, category filtering, and the multi-book catalog remain available at `/Catalog`. Checkout uses the anonymous cart, supports changing/removing multiple book lines, and displays book types, total copies, and order total. Placing an order validates stock, stores the selected delivery city/payment method, reserves stock transactionally, clears the cart, and attempts to send an order confirmation email.

Configure SMTP outside source control before expecting confirmation email delivery. For example, set these environment variables in the deployment environment (use a secret store for credentials):

```powershell
$env:Email__Smtp__Host = "smtp.example.com"
$env:Email__Smtp__Port = "587"
$env:Email__Smtp__EnableSsl = "true"
$env:Email__Smtp__Username = "store@example.com"
$env:Email__Smtp__Password = "<SMTP app password>"
$env:Email__Smtp__FromAddress = "store@example.com"
$env:Email__Smtp__FromName = "Nhà Giả Kim"
```

When the customer presses **Đặt hàng**, the API saves the order and sends an email to the exact address entered in the checkout form. The confirmation includes the customer contact and delivery details, payment choice, ordered titles and quantities, and total amount. Configure SMTP outside source control before expecting delivery. If SMTP is missing or unavailable, the order remains saved, the API response reports `confirmationEmailSent: false`, the failure is logged, and the page tells the customer the email was not sent while preserving the order number. The checkout endpoint is limited to five placement attempts per client IP in ten minutes. “Chuyển khoản” and “Ví điện tử” are recorded choices only; no payment gateway or money transfer is processed.

### Administration

The authenticated admin area is available at `/admin/login`. After signing in, an administrator can manage orders and their status, books and stock, authors, categories, press articles, and reader feedback. Admin pages require an authenticated account with the `Admin` role; login is protected by antiforgery validation and IP-based rate limiting.

The first administrator is created only when the `Users` table is empty and both bootstrap settings are configured. Set them with environment variables or User Secrets (never commit credentials):

```powershell
$env:Admin__InitialUsername = "store-admin"
$env:Admin__InitialPassword = "<at least 12 characters>"
$env:Admin__InitialFullName = "Store administrator"
```

Alternatively, use `dotnet user-secrets set "Admin:InitialUsername" "store-admin"` and `dotnet user-secrets set "Admin:InitialPassword" "<at least 12 characters>"` from the project directory. The password is stored as a password hash. Once the first user exists, changing the bootstrap settings does not change existing credentials. If there is no configured account, the app logs a warning and leaves the login unavailable until settings are supplied.

The app applies EF Core migrations at startup. Back up the database and review the migration before deploying it to a database with existing data. Keep the administrator password in a secret store in production.

The included `http` development profile intentionally stays on HTTP; HTTPS redirection and HSTS are enabled outside Development.

To add a schema migration, restore the local EF tool once with `dotnet tool restore`, then run `dotnet ef migrations add <MigrationName>`.

## API

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/api/books?search=&categoryId=&page=1&pageSize=20` | Browse active books |
| GET | `/api/books/{id}` | Read an active book |
| GET | `/api/categories` | List categories |
| GET | `/api/carts/{cartId}` | Read an anonymous cart |
| POST | `/api/carts/{cartId}/items` | Add a book; creates the cart if needed |
| PUT | `/api/carts/{cartId}/items/{bookId}` | Set a cart line quantity |
| DELETE | `/api/carts/{cartId}/items/{bookId}` | Remove a cart line |
| POST | `/api/orders` | Place an order from a cart and decrement stock |
| GET | `/api/orders/{orderNumber}` | Read order confirmation details |

The client generates a GUID for a new cart and reuses it for subsequent requests. Checkout validates current stock, stores the order and line-item price snapshots, decrements inventory transactionally, then deletes the cart. API errors use Problem Details. A ready-to-run request collection is in [NhaGiaKim.http](./NhaGiaKim.http).
