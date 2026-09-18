# TodoAuthApi – JWT Kimlik Doğrulamalı Görev Yöneticisi

Kullanıcıların kayıt olup giriş yapabildiği ve yalnızca kendi görevlerini yönetebildiği, JWT tabanlı kimlik doğrulama kullanan bir ASP.NET Core Web API projesi. Basit bir HTML/JavaScript arayüzü de içerir.

## Özellikler

- Kullanıcı kaydı ve girişi
- Şifrelerin BCrypt ile hash'lenerek saklanması
- JWT (1 gün geçerli) ile korunan uç noktalar
- Her kullanıcı yalnızca kendi görevlerini görür, ekler, günceller ve siler
- Görevi tamamlandı / tamamlanmadı olarak işaretleme
- CORS açık; `wwwroot/index.html` üzerinden kullanılabilen arayüz

## Teknolojiler

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 (SQLite)
- JWT Bearer Authentication
- BCrypt.Net-Next
- Swagger (Swashbuckle)
- HTML + Vanilla JavaScript (Fetch API)

## Veri Modeli

**User:** `Id`, `Username`, `PasswordHash`, `Tasks`

**ToDoItem:** `Id`, `Title`, `IsCompleted`, `UserId`

## API Uç Noktaları

| Metot | Adres | Yetki | Açıklama |
|-------|-------|-------|----------|
| POST | `/api/Auth/register` | – | Yeni kullanıcı oluşturur |
| POST | `/api/Auth/login` | – | Giriş yapar, JWT döner |
| GET | `/api/ToDoItems` | JWT | Kullanıcının görevlerini listeler |
| POST | `/api/ToDoItems` | JWT | Yeni görev ekler |
| PUT | `/api/ToDoItems/{id}` | JWT | Görevin tamamlanma durumunu günceller |
| DELETE | `/api/ToDoItems/{id}` | JWT | Görevi siler |

Korunan uç noktalara istek atarken başlığa token eklenmelidir:

```
Authorization: Bearer <token>
```

## Kurulum ve Çalıştırma

```bash
git clone https://github.com/smlmrt/todo-auth-api.git
cd todo-auth-api
dotnet restore
dotnet ef database update
dotnet run
```

- API: `http://localhost:5221`
- Swagger: `http://localhost:5221/swagger`
- Arayüz: `wwwroot/index.html` dosyasını tarayıcıda açın (API adresi `http://localhost:5221/api` olarak ayarlıdır).

## Yapılandırma

JWT ayarları `appsettings.json` içindeki `Jwt` bölümündedir (`Key`, `Issuer`, `Audience`). Gerçek bir ortamda `Key` değerini değiştirip kaynak kod yerine ortam değişkeni veya User Secrets üzerinden verin.

## Proje Yapısı

```
TodoAuthApi/
├── Controllers/
│   ├── AuthController.cs       # Kayıt, giriş, token üretimi
│   └── ToDoItemsController.cs  # [Authorize] görev işlemleri
├── Data/AppDbContext.cs
├── Models/
│   ├── User.cs
│   └── ToDoItem.cs
├── wwwroot/index.html          # Basit web arayüzü
├── Migrations/
└── Program.cs
```
