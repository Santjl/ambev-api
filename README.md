# 🛍️ Developer Evaluation - Sales API

This project implements a **Sales Management API** using **.NET 8**, following **DDD principles**.  
It handles sales records, applies **business rules for discounts**, and publishes **domain events** (logged by default, pluggable via `IMessageBus`).

---

## 🚀 Features

- Complete **CRUD for Sales**
- Sales contain:
  - Sale number
  - Date
  - Customer (Id + Name)
  - Branch (Id + Name)
  - Products, quantities, unit prices
  - Discounts and totals
  - Cancelled / Not Cancelled status
- **Business Rules:**
  - 4–9 items → **10% discount**
  - 10–20 items → **20% discount**
  - Maximum **20 items per product**
  - <4 items → **no discount**
- **Events published** (to logs by default):
  - `SaleCreated`
  - `SaleModified`
  - `SaleCancelled`
  - `ItemCancelled`

You can switch to another broker by implementing the `IMessageBus` interface.

---

## 🐳 Running with Docker

The repository already includes a `docker-compose.yml`.

```sh
docker-compose up -d
```

This will start:

- `db`: PostgreSQL 13 with migrations automatically applied
- `api`: Sales API running on .NET 8

API will be available at:

👉 [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

---

## 🔑 Authentication

To authenticate in Swagger UI:
1. Create an user in `POST /api/users`
1. Request a token via `POST /api/auth/login` with your credentials.
2. Copy the returned **JWT token**.
3. In Swagger UI, click **Authorize** and paste:  
   ```
   Bearer {your_token_here}
   ```

Now you can call any secured endpoint.

---

# 📦 Sales Endpoints (Requests & Responses)

### 🔹 Create Sale – `POST /api/Sale`

**Request**
```json
{
  "number": "SO-1001",
  "customerId": "11111111-1111-1111-1111-111111111111",
  "branchId": "22222222-2222-2222-2222-222222222222",
  "items": [
    { "productId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "quantity": 2 },
    { "productId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "quantity": 5 }
  ]
}
```

**Response – 201 Created**
```json
{
  "success": true,
  "data": {
    "id": "e34d4f93-7b5f-48a8-a1f0-0bb1f9cb3a4f"
  }
}
```

**Response – 400 Bad Request (Example: quantity > 20)**
```json
{
  "success": false,
  "message": "Cannot sell more than 20 items of the same product"
}
```

---

### 🔹 Get Sale by Id – `GET /api/Sale/{id}`

**Response – 200 OK**
```json
{
  "success": true,
  "data": {
    "id": "e34d4f93-7b5f-48a8-a1f0-0bb1f9cb3a4f",
    "number": "SO-1001",
    "date": "2025-08-18T10:15:30Z",
    "customerId": "11111111-1111-1111-1111-111111111111",
    "customerName": "Alice Smith",
    "branchId": "22222222-2222-2222-2222-222222222222",
    "branchName": "Downtown",
    "items": [
      {
        "productId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
        "productName": "Mouse",
        "quantity": 2,
        "unitPrice": 50,
        "discount": 0,
        "total": 100
      },
      {
        "productId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
        "productName": "USB Cable",
        "quantity": 5,
        "unitPrice": 10,
        "discount": 5,
        "total": 45
      }
    ],
    "cancelled": false,
    "totalAmount": 145
  }
}
```

**Response – 404 Not Found**
```json
{
  "success": false,
  "message": "Sale not found"
}
```

---

### 🔹 Get All Sales – `GET /api/Sale`

**Response – 200 OK**
```json
{
  "success": true,
  "data": [
    {
      "id": "e34d4f93-7b5f-48a8-a1f0-0bb1f9cb3a4f",
      "number": "SO-1001",
      "customerName": "Alice Smith",
      "branchName": "Downtown",
      "date": "2025-08-18T10:15:30Z",
      "totalAmount": 145,
      "cancelled": false
    },
    {
      "id": "f12a9a65-25f1-412f-a7e2-8fa6c22ddf87",
      "number": "SO-1002",
      "customerName": "Bob Johnson",
      "branchName": "Airport",
      "date": "2025-08-18T11:05:10Z",
      "totalAmount": 90,
      "cancelled": true
    }
  ]
}
```

---

### 🔹 Modify Sale – `PUT /api/Sale`

**Request**
```json
{
  "id": "e34d4f93-7b5f-48a8-a1f0-0bb1f9cb3a4f",
  "items": [
    { "productId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "quantity": 10 },
    { "productId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "quantity": 0 }
  ]
}
```

**Response – 200 OK**
```json
{
  "success": true,
  "data": {
    "id": "e34d4f93-7b5f-48a8-a1f0-0bb1f9cb3a4f",
    "totalAmount": 500
  }
}
```

---

### 🔹 Cancel Sale – `PATCH /api/Sale/CancelSale/{id}`

**Response – 200 OK**
```json
{
  "success": true,
  "data": {
    "id": "e34d4f93-7b5f-48a8-a1f0-0bb1f9cb3a4f",
    "cancelled": true
  }
}
```

**Response – 404 Not Found**
```json
{
  "success": false,
  "message": "Sale not found"
}
```

---

## 📢 Events

By default, events are only logged:

```
Publishing event sales.item.cancelled payload={"SaleItemId": "...", "$type": "SaleItemCancelledEvent"}
```

To change the output (e.g. publish to RabbitMQ, Kafka, etc.), implement the `IMessageBus` interface.

---
