# EcommerceAPI

A production-oriented RESTful e-commerce backend built with **ASP.NET Core .NET 8**, using **ADO.NET, SQL Server, DTOs, FluentValidation, AutoMapper, JWT authentication, refresh tokens, middleware, API versioning, caching, ETags, rate limiting, and stored procedures and comprehensive API testing**.

---

# Features

## Product Management

* Product CRUD operations
* Product search
* Combined product search by:

  * Name
  * Description
  * Category
  * Brand
* Product filtering
* Price range filtering
* Rating filtering
* Sorting
* Pagination
* Category APIs
* Brand APIs
* Product price management
* Product availability validation
* Product inventory management
* Single inventory/stock updates
* Bulk inventory updates
* Product image upload and download
* Image type validation
* Image file-size validation

---

## Customer & Authentication

* Customer registration
* Customer login
* Customer model
* Customer-specific operations
* JWT authentication
* Refresh token support
* Refresh token rotation
* Refresh token expiration
* Logout and refresh-token revocation
* Role-based authorization
* Claims-based authorization
* Customer and Admin roles
* `CanManageProducts` claim
* `CanManageOrders` claim
* Public registration always creates a Customer account

Protected operations use JWT authentication and authorization policies.

---

# Address Management

Customers can maintain multiple saved addresses.

Address information includes:

* Address ID
* Customer ID
* Address Line 1
* Address Line 2
* City
* State
* Postal Code
* Country
* Default address flag
* Created date
* Updated date

Each address is associated with a customer through `CustomerId`.

Addresses are also used during checkout to identify the customer's shipping address.

---

# Shopping Cart

* Add items to cart
* Update cart items
* Remove cart items
* Clear cart
* Retrieve cart details
* Cart item count
* Cart subtotal
* Product validation
* Quantity validation
* Price validation
* Stock validation

Cart operations validate product availability and enforce business rules before modifying cart data.

---

# Inventory & Availability

The application contains a dedicated inventory system.

Inventory supports:

* Product stock management
* Single stock updates
* Bulk stock updates
* Product availability checking
* Bulk availability validation
* Non-negative stock validation
* Product existence validation
* Stock validation during cart and checkout operations

Inventory data is stored separately from product information in:

`Srizan_Inventory`

The application uses SQL Server table-valued parameters for bulk inventory operations.

---

# Checkout

The checkout process validates the customer's cart before creating an order.

Supported functionality:

* Shipping address selection
* Shipping method selection
* Shipping charge calculation
* Tax calculation
* Subtotal calculation
* Final total calculation
* Current price validation
* Stock availability validation
* Cart validation
* Order creation
* Pending order creation before payment

### Checkout Flow

```text
Customer Cart
     ↓
Validate Products
     ↓
Validate Stock
     ↓
Validate Current Prices
     ↓
Calculate Subtotal
     ↓
Calculate Shipping
     ↓
Calculate Tax
     ↓
Calculate Final Total
     ↓
Create Pending Order
     ↓
Process Mock Payment
     ↓
Update Order Status
```

---

# Orders

* Create orders
* Retrieve order details
* Retrieve order items
* Retrieve order shipping information
* Retrieve order payment information
* Order totals
* Customer-specific order history
* Order history status filtering
* Order history pagination
* Reorder previous orders
* Order status workflow
* Order tracking

### Order Status Workflow

Supported order lifecycle:

```text
Pending / Placed
       ↓
   Processing
       ↓
    Shipped
       ↓
   Delivered
```

Cancellation is supported from appropriate order states.

Invalid status transitions are rejected by the database business rules.

---

# Order Tracking

Order status changes are recorded in a dedicated tracking table.

Tracking information includes:

* Tracking ID
* Order ID
* Status
* Tracking note
* Created date
* Created by

The `Srizan_UpdateOrderStatus` stored procedure updates the order status and records the corresponding tracking entry.

Customers can retrieve the tracking history for their own orders.

---

# Payment

The application contains a mock payment API for testing.

Supported functionality:

* Payment method handling
* Payment amount validation
* Mock payment processing
* UUID transaction reference generation
* Payment status tracking
* Order payment creation
* Order status update after successful payment

### Payment Flow

```text
Checkout
   ↓
Pending Order
   ↓
Mock Payment API
   ↓
Generate UUID Transaction Reference
   ↓
Create Order Payment
   ↓
Payment Status = Success
   ↓
Order Status = Placed
```

The transaction reference is stored in:

`Srizan_OrderPayments`

---

# API Versioning

The API uses URL-based API versioning.

## Version 1

```text
/api/v1/products
/api/v1/cart
/api/v1/checkout
/api/v1/orders
```

## Version 2

```text
/api/v2/products
/api/v2/cart
/api/v2/checkout
/api/v2/orders
```

Versioned Product, Cart, Checkout, and Order endpoints are implemented using `Asp.Versioning.Mvc`.

---

# Caching

The application uses **ASP.NET Core `IMemoryCache`** for product listing caching.

Product data is cached using:

```text
products:all
```

The cache is invalidated when product information that affects the listing changes, including:

* Product creation
* Product update
* Product deletion
* Product image update
* Inventory/stock update
* Bulk inventory update

This ensures that subsequent requests retrieve fresh product information.

---

# ETags & Conditional GET

The product listing endpoint supports **ETags** for conditional GET requests.

An ETag is generated by creating a SHA256 hash of the serialized product response.

### First Request

```http
GET /api/v1/products
```

The server returns:

```http
200 OK
ETag: "ABC123..."
```

### Subsequent Request

The client can send:

```http
GET /api/v1/products
If-None-Match: "ABC123..."
```

If the product data has not changed, the API returns:

```http
304 Not Modified
```

without sending the complete product response again.

If the data has changed, the API returns:

```http
200 OK
```

with the updated product data and a new ETag.

### ETag Flow

```text
Product Data
     ↓
Serialize to JSON
     ↓
SHA256 Hash
     ↓
ETag
     ↓
Compare with If-None-Match
     ↓
   ┌───────────────┐
   │               │
 Same           Different
   │               │
   ↓               ↓
 304              200
```

---

# Product Images

Products support image upload and download.

## Upload

```http
POST /api/v1/products/{id}/image
```

Supported image extensions:

```text
.jpg
.jpeg
.png
.webp
```

Maximum file size:

```text
5 MB
```

Images are uploaded using `IFormFile` and `multipart/form-data`.

The actual image file is stored under:

```text
wwwroot/product-images
```

The application does **not** convert the image to Base64.

Instead:

```text
Actual Image
     ↓
wwwroot/product-images
     ↓
ImagePath stored in SQL Server
```

The image path is stored in:

`Srizan_Products.ImagePath`

The product cache is invalidated after an image update.

## Download

```http
GET /api/v1/products/{id}/image
```

The API retrieves the stored image path, locates the physical file, determines the content type, and returns the image as a file response.

---

# Bulk Operations

The API supports bulk inventory operations using SQL Server table-valued parameters.

## Bulk Inventory Update

```http
PUT /api/v1/products/bulk-stock
```

Multiple products can have their stock quantities updated in a single request.

The operation validates:

* Product IDs
* Product existence
* Non-negative quantities

## Bulk Availability Validation

```http
POST /api/availability/check
```

Multiple products and requested quantities can be checked in one request.

The API returns:

* Product ID
* Requested quantity
* Available quantity
* Availability status

---

# Enhanced Product Search

The product search procedure supports combined searching across:

* Product name
* Product description
* Category name
* Brand name

Example:

```http
GET /api/v1/products/search?search=phone
```

The search can also be combined with existing filters:

```http
GET /api/v1/products/search?search=phone&categoryId=1&brandId=2&minPrice=10000&minRating=4&page=1&pageSize=10
```

Existing sorting and pagination continue to be handled by the SQL stored procedure.

---

# Logging & Error Handling

## Request Logging Middleware

The application records request information including:

* User ID
* Username
* HTTP method
* Request path
* Request parameters
* Response status
* Request duration
* Timestamp

Request audit records are persisted in:

`Srizan_RequestAuditLogs`

Stored procedure:

`Srizan_LogRequest`

---

## Global Exception Handling Middleware

Unhandled exceptions are handled centrally by custom middleware.

The middleware:

* Logs exceptions
* Persists error information to the database
* Returns standardized `ProblemDetails` responses
* Converts known business validation SQL errors into `400 Bad Request`
* Returns `500 Internal Server Error` for unexpected exceptions

Error records are persisted in:

`Srizan_ErrorLogs`

Stored procedure:

`Srizan_LogError`

Example:

```json
{
  "type": "about:blank",
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "The server encountered an unexpected error.",
  "instance": "/api/v1/products"
}
```

---

# Rate Limiting

The API uses ASP.NET Core rate limiting middleware.

The configured limit is:

```text
100 requests per minute per user
```

Authenticated users are partitioned using their JWT `NameIdentifier` claim.

Unauthenticated requests are partitioned using the client's IP address.

When the limit is exceeded, the API returns:

```http
429 Too Many Requests
```

### Rate Limiting Flow

```text
Request
   ↓
Identify User
   ↓
Check Request Count
   ↓
Under 100?
   ├── Yes → Continue
   └── No  → 429 Too Many Requests
```

---

# Validation

The API uses multiple levels of validation.

## Data Annotations

Used for basic model validation such as:

* Required fields
* String length
* Numeric ranges
* Valid quantities
* Valid IDs

## FluentValidation

Used for application-level validation such as:

* Registration validation
* Login validation
* Cart item validation
* Quantity validation
* Business input validation

## Database Validation

Stored procedures also enforce important business rules such as:

* Product existence
* Valid inventory quantities
* Valid order status transitions
* Stock availability
* Product relationships

---

# Async Operations

The API uses asynchronous programming throughout the application.

`CancellationToken` support is implemented across:

* Controllers
* Services
* Repositories
* ADO.NET database operations

Database operations use asynchronous ADO.NET methods such as:

```text
OpenAsync()
ExecuteReaderAsync()
ExecuteScalarAsync()
ExecuteNonQueryAsync()
ReadAsync()
```

This allows database operations to be cancelled when the client request is aborted.

---

# Postman Documentation

The API endpoints are organized and documented in a Postman collection covering:

* Authentication
* Products
* Categories
* Brands
* Prices
* Availability
* Inventory
* Cart
* Checkout
* Payments
* Orders
* Order History
* Address management
* Order tracking
* API versioning
* Error handling
* Validation
* Bulk operations

Each endpoint can include:

* HTTP method
* URL
* Authentication requirements
* Parameters
* Request body
* Example response
* Expected HTTP status codes

The Postman collection can be imported into Postman for API testing and documentation.

---

# Technologies

* .NET 8
* ASP.NET Core Web API
* C#
* SQL Server
* ADO.NET
* `Microsoft.Data.SqlClient`
* JWT Authentication
* Refresh Tokens
* Claims-based Authorization
* AutoMapper
* FluentValidation
* `IMemoryCache`
* ASP.NET Core Response Caching
* ASP.NET Core Rate Limiting
* API Versioning
* Swagger / OpenAPI
* Postman

---

# Project Structure

```text
EcommerceAPI
│
├── Controllers
├── DTOs
├── Middleware
├── Models
├── Repositories
│   ├── Interfaces
│   └── Implementations
├── Services
│   ├── Interfaces
│   └── Implementations
├── Validators
├── Mapping
├── Profiles
└── Program.cs
```

---

# Architecture

The API follows a layered architecture:

```text
Controller
     ↓
Service
     ↓
Repository Interface
     ↓
Repository Implementation
     ↓
Stored Procedure
     ↓
SQL Server
```

DTOs and AutoMapper are used to separate API request/response models from application/domain models.

Cross-cutting concerns such as authentication, authorization, exception handling, logging, caching, ETags, CORS, and rate limiting are configured through ASP.NET Core middleware and services.

---

# Database Configuration

The application uses the company-provided:

```text
TrainingDB
```

The database connection is configured using the `DefaultConnection` connection string.

The application uses the existing TrainingDB database and its required SQL objects.

---

# Database Objects

## Tables

```text
Srizan_Customers

Srizan_Categories
Srizan_Brands
Srizan_Products
Srizan_ProductPrices
Srizan_Inventory

Srizan_Carts
Srizan_CartItems

Srizan_Addresses

Srizan_Orders
Srizan_OrderItems
Srizan_OrderShipping
Srizan_OrderPayments
Srizan_OrderTracking

Srizan_ErrorLogs
Srizan_RequestAuditLogs
```

## Table-Valued Types

```text
Srizan_InventoryItemType
Srizan_BulkInventoryType
```

---

# Stored Procedures

## Products

```text
Srizan_GetAllProducts
Srizan_GetProductById
Srizan_SearchProducts
Srizan_CreateProduct
Srizan_UpdateProduct
Srizan_DeleteProduct
Srizan_UpdateProductImage
Srizan_UpdateProductStock
Srizan_BulkUpdateInventory
```

## Product Availability & Inventory

```text
Srizan_GetProductAvailability
Srizan_CheckInventory
Srizan_UpdateProductStock
Srizan_BulkUpdateInventory
```

## Cart

```text
Srizan_GetCart
Srizan_AddCartItem
Srizan_UpdateCartItem
Srizan_RemoveCartItem
Srizan_ClearCart
Srizan_GetCartItemCount
Srizan_GetCartSubtotal
```

## Address

```text
Srizan_CreateAddress
Srizan_GetCustomerAddresses
Srizan_UpdateAddress
Srizan_DeleteAddress
```

## Orders

```text
Srizan_CreateOrder
Srizan_CreateOrderItem
Srizan_CreateOrderShipping
Srizan_CreateOrderPayment
Srizan_GetOrderById
Srizan_GetOrderItems
Srizan_GetOrderShipping
Srizan_GetOrderPayment
Srizan_UpdateOrderStatus
Srizan_GetOrderHistory
```

## Order Tracking

```text
Srizan_AddOrderTracking
Srizan_GetOrderTracking
```

## Logging

```text
Srizan_LogError
Srizan_LogRequest
```

---

# Authentication & Authorization

The API uses JWT authentication with refresh tokens.

Two primary roles are supported:

```text
Customer
Admin
```

## Customer

Customers can access customer-specific operations such as:

* Cart
* Checkout
* Orders
* Order history
* Reorder
* Addresses
* Order tracking

## Admin

Admins can access administrative operations such as:

* Product creation
* Product updates
* Product deletion
* Product image management
* Inventory management
* Administrative product operations

Claims-based policies are also used.

### Product Management Claim

```text
CanManageProducts = true
```

### Order Management Claim

```text
CanManageOrders = true
```

The JWT contains standard identity information such as:

* Customer ID
* Name
* Email
* Role

Admin tokens additionally contain the management claims.

---

# Refresh Token Flow

```text
Login
   ↓
Generate Access Token
   ↓
Generate Refresh Token
   ↓
Store Refresh Token Session
   ↓
Client receives both tokens
```

When the access token expires:

```text
Refresh Token
      ↓
Validate Refresh Session
      ↓
Generate New Access Token
      ↓
Rotate Refresh Token
      ↓
Return New Token Pair
```

Refresh tokens have an expiration period and are revoked when the customer logs out.

---

# Order History

Customers can retrieve their previous orders.

Supported features:

* Customer-specific order history
* Status filtering
* Pagination
* Reordering previous orders

Example:

```http
GET /api/v1/orders/history
```

With filtering and pagination:

```http
GET /api/v1/orders/history?status=Pending&page=1&pageSize=10
```

Reorder:

```http
POST /api/v1/orders/{orderId}/reorder
```

---

# API Testing

The API can be tested using Swagger or Postman.

Test areas include:

* Customer registration
* Login
* JWT authentication
* Refresh tokens
* Logout
* Role-based authorization
* Claims-based authorization
* Product CRUD
* Product search
* Combined search
* Filtering
* Sorting
* Pagination
* Product availability
* Inventory updates
* Bulk inventory updates
* Bulk availability validation
* Cart operations
* Address management
* Checkout
* Shipping calculation
* Tax calculation
* Mock payment
* Order creation
* Order retrieval
* Order history
* Order status workflow
* Order tracking
* Reorder
* API versioning
* Memory caching
* Cache invalidation
* ETags
* Conditional GET
* Product image upload
* Product image download
* Rate limiting
* Validation errors
* Not Found responses
* Unauthorized responses
* Forbidden responses
* Global exception handling
* Request logging
* Error logging

---

# Running the API

Run the application using:

```bash
dotnet run
```

When Swagger is enabled, it can be accessed through the application's Swagger endpoint.

---

# Example API Flow

A typical customer purchase flow is:

```text
Register
   ↓
Login
   ↓
Receive JWT + Refresh Token
   ↓
Browse Products
   ↓
Check Product Availability
   ↓
Add Product to Cart
   ↓
Select Shipping Address
   ↓
Checkout
   ↓
Validate Price & Stock
   ↓
Calculate Order Total
   ↓
Create Pending Order
   ↓
Mock Payment
   ↓
Payment Success
   ↓
Order Placed
   ↓
Order Processing
   ↓
Order Shipped
   ↓
Order Delivered
   ↓
View Order History
   ↓
Track Order
   ↓
Reorder if Required
```

---

# Error Response

Unhandled exceptions are returned using the standard `ProblemDetails` format.

Example:

```json
{
  "type": "about:blank",
  "title": "An unexpected error occurred.",
  "status": 500,
  "detail": "The server encountered an unexpected error.",
  "instance": "/api/v1/products"
}
```

Known business validation errors are returned as:

```http
400 Bad Request
```

Rate-limit violations are returned as:

```http
429 Too Many Requests
```

Authentication and authorization failures return the appropriate HTTP status codes such as:

```http
401 Unauthorized
403 Forbidden
```

---

# Development Approach

The application emphasizes:

* Layered architecture
* Separation of concerns
* Stored-procedure-based data access
* Parameterized SQL commands
* DTO-based API contracts
* Centralized validation
* Centralized exception handling
* Structured logging
* Secure JWT authentication
* Refresh token rotation
* Claims-based authorization
* Cache invalidation
* Conditional HTTP requests using ETags
* Rate limiting
* Asynchronous database operations
* Cancellation support
* Bulk database operations using table-valued parameters
* Business-rule enforcement at the database and application layers
