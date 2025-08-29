# Wallet API

A minimalistic wallet management API built with ASP.NET Core, supporting **Credit**, **Debit**, and **Transfer** operations with **idempotency** and **ETag-based optimistic concurrency control**.

## Features

- Create and manage wallets
- Perform operations:
  - Credit
  - Debit
  - Transfer
- Idempotent POST requests using `Idempotency-Key` headers
- ETag headers for version control
- Centralized exception handling via `ExceptionMapper`
- Factory pattern for operation handlers (`CreditHandler`, `DebitHandler`, `TransferHandler`)

## Project Structure

Api/
├── Dtos/ # Request and response DTOs
├── EndPoints/ # Minimal API endpoint definitions
├── Helper/ # Utilities like ETagHelper, ExceptionMapper
├── Operations/ # Operation handlers and factory
Core/
├── Services/ # WalletService for business logic
Infrastructure/
├── Idempotency/ # Idempotency store implementation



## API Endpoints

### Wallets

- `POST /wallets` – Create a new wallet  
  Headers: `Idempotency-Key` (optional)  
  Body: `{ "ownerName": "Alice", "currency": "USD" }`

- `GET /wallets/{id}` – Retrieve wallet details  
  Returns current balance, version, and metadata

### Operations

- `POST /operations` – Execute a wallet operation  
  Headers: `Idempotency-Key` (optional), `If-Match` (optional for version control)  
  Body examples:
  
  **Credit**  
  ```json
  {
    "type": "credit",
    "walletId": "GUID",
    "amount": 100,
    "reference": "optional note"
  }
**debit**
{
  "type": "debit",
  "walletId": "GUID",
  "amount": 50
}

**transfer**
{
  "type": "transfer",
  "fromWalletId": "GUID",
  "toWalletId": "GUID",
  "amount": 30
}
**Note**
Operations are thread-safe using SyncRoot locks per wallet.

ETag headers provide optimistic concurrency to prevent conflicts.

All operations return consistent responses for idempotent requests.
