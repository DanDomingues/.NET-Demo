# .NET Demo

ASP.NET Core 8 MVC portfolio project that demonstrates a small e-commerce workflow with role-based administration, checkout, order management, and reusable CRUD infrastructure.

The application combines a customer storefront with an admin back office. Customers can browse a seeded catalog, add items to a cart, place orders, and review their order history. Admin users can manage products, categories, companies, users, and orders from a separate area of the application.

## Project Highlights

- Customer storefront with product catalog, category grouping, product detail pages, cart flow, and order history
- Role-based access model for `Admin`, `Employee`, `Company`, and `Customer`
- Admin tooling for category, company, product, and user management
- Order lifecycle support for pending, approved, processing, shipped, cancelled, refunded, and rejected states
- Stripe Checkout integration for card payments in test mode
- Delayed-payment flow for company accounts
- Seeded catalog, company data, roles, and initial admin user
- Reusable repository, unit-of-work, and controller-module patterns to reduce CRUD duplication

## Feature Overview

### Customer Experience

- Browse products from the home page, grouped by category
- View product details and add products to the shopping cart
- Maintain cart state through session-backed cart count updates
- Submit order details during checkout
- Complete payment through Stripe Checkout for standard users
- Place approved-but-delayed-payment orders for company accounts
- Review personal order history and order details

### Admin Experience

- Manage categories, including display ordering controls
- Create and maintain companies used by company and employee accounts
- Create, edit, and delete products
- Upload product images, prevent duplicate filenames, and reorder image display priority
- Review user accounts, assign roles, link users to companies, and toggle account lock state
- Review all orders and move orders through operational states such as processing and shipping
- Update tracking and carrier information
- Cancel orders and trigger Stripe refunds for paid orders

## Architecture

The solution is split into focused projects:

- `Demo.Main`
  ASP.NET Core MVC application, Razor views, Identity UI, front-end assets, and controllers
- `Demo.DataAccess`
  Entity Framework Core context, migrations, repositories, unit of work, initialization, and Stripe helpers
- `Demo.Models`
  Domain models and view models
- `Demo.Utility`
  Shared constants, extension helpers, feedback utilities, and demo email sender

### Notable Design Choices

- Repository + Unit of Work pattern
  Data access is encapsulated behind repository interfaces and coordinated through a unit-of-work implementation.
- Reusable controller modules
  Shared CRUD behavior is centralized through `RepositoryBoundController` and controller module classes instead of being repeated in each controller.
- ASP.NET Core Areas
  The app separates customer and admin concerns through `Customer`, `Admin`, and `Identity` areas.
- Startup seeding
  The application applies pending migrations on startup and seeds roles, an admin account, catalog data, companies, and product images.

## Tech Stack

### Backend

- ASP.NET Core 8 MVC
- Razor Views and Razor Pages
- ASP.NET Core Identity
- Entity Framework Core
- SQL Server
- Stripe.net

### Frontend

- Bootstrap 5
- jQuery
- DataTables
- SweetAlert2
- Toastr
- TinyMCE
- Bootstrap Icons

## Domain Model

The main entities in the project are:

- `ApplicationUser`
  Extended Identity user with profile fields, role-aware UI behavior, and optional company association
- `Category`
  Product grouping with sortable display order
- `Product`
  Catalog item with tiered pricing, category relation, and image collection
- `ProductImage`
  Product gallery image with explicit display ordering
- `Company`
  Organization record used for company and employee accounts
- `ShoppingCartItem`
  User-owned cart item with quantity and computed total cost
- `OrderHeader`
  Order-level customer, shipping, payment, and fulfillment data
- `OrderItemDetails`
  Line-item snapshot for purchased products

## Repository Structure

```text
.NET Demo/
├── Demo.Main/          Web application, controllers, views, wwwroot
├── Demo.DataAccess/    EF Core context, migrations, repositories, seeding
├── Demo.Models/        Domain models and view models
├── Demo.Utility/       Shared constants and helper utilities
└── README.md
```

## Configuration Notes

### Database

- The default local configuration targets SQL Server with trusted connection support.
- The `ApplicationDbContextFactory` exists so EF Core design-time commands work even though the `DbContext` lives in a separate project.

### Payments

- Stripe is wired for Checkout-based payment flow
- The app expects test credentials for local development
- Company users follow a delayed-payment path instead of immediate card checkout

### Email

- The app includes a required `IEmailSender` implementation for Identity integration
- Email sending is intentionally disabled because this project is positioned as a demo, not a production email workflow

## Example Workflows

### Customer Flow

1. Browse products on the storefront
2. Open a product detail page
3. Add the product to cart
4. Review the order summary
5. Submit shipping details
6. Complete payment through Stripe or use delayed payment through a company account
7. Revisit order history and order detail pages

### Admin Flow

1. Sign in as admin
2. Open the `Manage` navigation menu
3. Maintain categories, products, companies, and users
4. Assign company relationships and roles to users
5. Review incoming orders
6. Update order details, set carrier data, process shipments, or cancel/refund orders

## Current Limitations

This project is intentionally demo-oriented. A production hardening pass would still include:

- moving all secrets out of tracked configuration
- adding automated test coverage
- improving validation and error handling across flows
- replacing the no-op email sender with a real provider
- adding deployment-specific configuration and health monitoring
- documenting screenshots, recorded walkthroughs, and hosted demo links

## Build Status

The project currently builds successfully with:

```powershell
dotnet build Demo.Main\Demo.csproj
```

There are existing compiler warnings in the solution, primarily around nullable reference types and a few legacy migration naming issues, but the application builds and runs.

## License

No license file is currently included in the repository. Add one before public distribution if you want to define reuse terms explicitly.
