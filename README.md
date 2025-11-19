# paymentsportal-paymentsportal-API
Payments Portal – Full Stack Application (Angular + .NET Core)

A complete Payments Management Portal built using Angular (Frontend) and .NET Core Web API (Backend).
The system allows users to view, add, edit, and delete payments, with strong backend rules such as duplicate prevention and sequential reference generation.

# PaymentsApi (.NET 8) - Backend for Payments Portal

This is the backend API for the Payments Portal assignment.
It provides CRUD endpoints for payments with idempotency (clientRequestId) and per-day sequential reference generation.

## Features
- POST /api/payments : create payment (idempotent by clientRequestId)
- GET /api/payments : list payments
- PUT /api/payments/{id} : update amount/currency
- DELETE /api/payments/{id} : delete payment
- Reference format: `PAY-YYYYMMDD-####` (sequential per day)
- SQLite DB by default (`payments.db`)
- EF Core migrations and Swagger included

## Prerequisites
- .NET 8 SDK installed
- (Optional) dotnet-ef tool for migrations: `dotnet tool install --global dotnet-ef --version 8.0.0`

## Restore & run (development)

1. Restore packages(using command line tool or Powershell terminal):
   
   dotnet restoredotnet restore

2. Create initial migration (if you want explicit migrations):

    dotnet ef migrations add InitialCreate
    dotnet ef database update
   If you skip migrations the app will create DB and tables at runtime.
4. Run API.

Example curl requests
1. Create Payment
curl -k -X POST https://localhost:5001/api/payments \
  -H "Content-Type: application/json" \
  -d '{"amount":100.00,"currency":"USD","clientRequestId":"11111111-1111-1111-1111-111111111111"}'
<img width="815" height="1111" alt="image" src="https://github.com/user-attachments/assets/5dbe33d1-5839-42aa-bff6-534b55909c22" />

   
3. List payments
   curl -k https://localhost:5001/api/payments
<img width="394" height="539" alt="image" src="https://github.com/user-attachments/assets/9edf968a-7da5-4bd6-902f-8c41ba0c1a7c" />

4. Update payment
  curl -k -X PUT https://localhost:5001/api/payments/{id} \
  -H "Content-Type: application/json" \
  -d '{"amount":150.00,"currency":"USD","clientRequestId":"some-guid"}'
   <img width="373" height="522" alt="image" src="https://github.com/user-attachments/assets/f98c668e-52bf-4636-975a-2b910c36cb49" />

   
6. Delete payment
  curl -k -X DELETE https://localhost:5001/api/payments/{id}
<img width="374" height="251" alt="image" src="https://github.com/user-attachments/assets/f578e4bb-2572-4f32-906a-509c0574cf79" />

   
Notes & guidance
The code uses UTC for CreatedAt and reference date. If you need local-time references, adjust DateTime.UtcNow usage.
For production and high concurrency, replace the SQLite + DailySequence approach with a DB-native sequence or a stored procedure to guarantee atomic sequence generation at scale.
Add authentication/authorization before exposing to the public.
    


    
