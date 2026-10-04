# Nhà Giả Kim - Bookstore

ASP.NET Core 10 MVC application with a SQL Server-backed bookstore, storefront, checkout, and authenticated administration.

## Deployment

### Deploy directly to Render

Render runs the existing ASP.NET application from the Linux Dockerfile. The SQL Server database remains hosted separately.

1. Confirm that the online SQL Server accepts encrypted SQL-authenticated connections from Render. If the database has an IP firewall, allow the outbound IP addresses shown for the Render service. Do not use the Windows-authenticated development connection string.
2. Push this repository to GitHub, then in Render choose **New + → Blueprint** and select the repository. Render reads `render.yaml`; choose the free web service if prompted.
3. When asked for `ConnectionStrings__DefaultConnection`, enter the production SQL Server connection string as a secret. Do not commit it to Git or send it in chat.
4. Create the Blueprint. Render builds the Docker image and runs EF Core migrations during startup. Wait until the service is **Live**, then open its `onrender.com` URL.
5. In the service's **Environment** settings, set `Admin__InitialPassword` to a strong password of at least 12 characters if the database does not already contain an administrator. `Admin__InitialUsername` defaults to `admin`; the app only creates this account when the `Users` table is empty and does not reset an existing account. Keep the password in Render's secret environment settings, never in source control or chat. Save changes to redeploy. Add `Email__Smtp__...` variables there if order confirmation email is required.

The included `free` plan may spin down after inactivity and take time to respond to the first request. Render's plan availability and limits can change; check its current pricing before deploying. The app applies database migrations on startup, so back up and review any production database before the first deploy. Keep credentials in Render's environment settings.

Subsequent pushes to the connected branch trigger a new deployment. If the build fails, inspect **Events** and **Logs** in Render; database connection or firewall errors usually mean the SQL Server is not reachable from the service.

### Optional: deploy through Vercel

If you specifically want a Vercel URL, deploy the ASP.NET Docker service on a separate container host and set Vercel's server-side `ASPNET_ORIGIN` to that host's HTTPS origin. Render can serve the app directly, so Vercel is not needed for a Render deployment.

For a separate Vercel deployment, the repository can also be configured as a split deployment:

- **Vercel** runs a small Next.js reverse proxy and serves the public Vercel URL.
- **A container host** runs the existing ASP.NET MVC app from the included Linux Docker image.
- **SQL Server** must be reachable by the ASP.NET container; use a managed or separately hosted database in production.

Vercel does not run this ASP.NET application from its Dockerfile. The Vercel project proxies every route, including storefront, checkout, admin pages, static files, and `/api`, to the container host. The MVC pages and application behavior remain in ASP.NET; the small Next.js app is only the proxy.

### Deploy the ASP.NET container

1. Provision a SQL Server database reachable from your container host. Configure a SQL-authenticated connection string; the development Windows-authenticated connection string in `appsettings.json` will not work in the Linux container.
2. Copy `.env.example` to `.env`, then set `ConnectionStrings__DefaultConnection`. Set the initial admin values when provisioning the first administrator, and SMTP values if order confirmation email is needed.
3. Build and start the container:

   ```sh
   docker compose up --build -d
   ```

   The app listens on port `8080`. Configure the container host to expose that port over HTTPS and note its public origin, for example `https://bookstore.example-host.com` (no path or trailing slash is required).

The app applies EF Core migrations at startup. Back up and review the database before deploying, especially when it contains existing data. Keep `.env` and all production credentials out of source control. Do not expose SQL Server publicly unless the provider requires it; restrict database access to the app host where possible.

### Deploy the Vercel proxy

1. Import this repository into Vercel and use the repository root as the project root. Vercel detects the root `package.json` as a Next.js project.
2. Add the server-side environment variable `ASPNET_ORIGIN` to the Vercel project for every deployment environment. Set it to the public HTTPS origin of the running ASP.NET container, such as `https://bookstore.example-host.com`.
3. Deploy. Vercel's public domain will proxy requests to the ASP.NET app. Re-deploy if the container host origin changes.

For a local proxy build, set `ASPNET_ORIGIN` to a reachable ASP.NET origin and run `npm ci && npm run build`. Keep the variable server-side; it must not use a `NEXT_PUBLIC_` prefix.

Because Vercel proxies requests to the container, IP-based rate limits in the app may see the proxy's egress address rather than each visitor's address. Verify checkout and admin-login throttling after deployment; use a hosting/proxy setup that forwards trusted client IPs if per-visitor throttling is required.

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

For Render, `Admin__InitialUsername` is preconfigured as `admin`; set `Admin__InitialPassword` directly in the service's **Environment** settings to a strong password of at least 12 characters. Alternatively, use `dotnet user-secrets set "Admin:InitialUsername" "admin"` and `dotnet user-secrets set "Admin:InitialPassword" "<at least 12 characters>"` from the project directory. The password is stored as a password hash. Bootstrap runs only when the `Users` table is empty; changing these settings does not change existing credentials. If an administrator already exists, use the existing account or an authorized password-reset procedure. If there is no configured account, the app logs a warning and leaves the login unavailable until settings are supplied.

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
