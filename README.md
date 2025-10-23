# 💳 Payment Gateway Microservices System

## 🧩 1. شرح پروژه

### 🎯 هدف پروژه
این پروژه یک **درگاه پرداخت آزمایشی** است که با معماری **Microservices + Clean Architecture** طراحی شده است.  
کاربر از طریق Gateway وارد  صفحه پرداخت شده و پس از انجام عملیات پرداخت (موفق یا ناموفق)،  
نتیجه تراکنش به سرویس Payment ارسال و وضعیت به‌روزرسانی می‌شود.

در نهایت سرویس Notification از طریق RabbitMQ رویدادها را دریافت کرده و لاگ یا Callback انجام می‌دهد.

---------------------------

### 🏗️ معماری استفاده‌شده
- **Clean Architecture** (Domain, Application, Infrastructure, API)
- **CQRS + MediatR** برای جداسازی Command/Query
- **Microservices Architecture** با سه سرویس مستقل:
  1. **Payment Service** (پورت 5001)
     - مدیریت تراکنش‌ها و Tokenها  
     - ارتباط با دیتابیس  
     - ارسال رویداد به RabbitMQ  
     - Background Job برای انقضای پرداخت‌ها  
  2. **Gateway Service** (پورت 5002)
     - شبیه‌سازی درگاه بانکی  
     - شبیه‌سازی پرداخت موفق/ناموفق  
     - ارتباط با Payment از طریق HTTP (Refit) یا RabbitMQ  
  3. **Notification Service** (پورت 5003)
     - دریافت پیام‌های RabbitMQ  
     - ثبت لاگ یا ارسال Callback به RedirectUrl  

-----------------------------------------

### 🧰 تکنولوژی‌های به‌کاررفته
| دسته | ابزار / فریم‌ورک |
|------|-------------------|
| Backend | .NET 8, ASP.NET Core Web API |
| Messaging | RabbitMQ (v7.1.2) |
| Background Jobs | Hangfire |
| HTTP Client | Refit |
| Validation | FluentValidation |
| ORM / DB Access | Dapper / EFCore (قابل تنظیم) |
| Testing | xUnit, Moq, FluentAssertions | Swagger
| Logging | Serilog (File Sink) |
----------------------------------------------------------
اجرای هر سرویس:

cd PaymentService.Api
dotnet run --urls=http://localhost:5001

cd GatewayService.Api
dotnet run --urls=http://localhost:5002

cd NotificationService.Api
dotnet run --urls=http://localhost:5003
-----------------------------------------------

چرا از این ساختار استفاده شده؟

جداسازی وابستگی‌ها (Separation of Concerns)

تست‌پذیری بالا به‌واسطه MediatR و Mockable Interfaces

مقیاس‌پذیری با استفاده از RabbitMQ برای ارتباطات ناهمزمان

انعطاف در توسعه؛ هر سرویس می‌تواند مستقل مستقر (Deploy) شود

-----------------------------------------------
چالش‌های پیاده‌سازی

هماهنگی بین سرویس‌ها (Sync و Async)

مدیریت وضعیت‌های مختلف تراکنش (Pending, Success, Failed, Expired)

پیاده‌سازی مصرف‌کننده RabbitMQ با نسخه جدید (بدون IModel)

مدیریت خطاهای Refit و بازگشت پاسخ استاندارد در همه‌ی APIها
--------------------------------------------------------
بهبودهای پیشنهادی در آینده

افزودن Authentication/Authorization (JWT یا API Key)

پیاده‌سازی Polly Retry Policy برای ارتباطات HTTP

ذخیره لاگ‌ها در Elastic Stack یا Seq

افزودن Observability (Prometheus + Grafana)

افزودن Integration Tests برای Gateway-Payment Flow
