# 🚀 Restoran API Test Rehberi

## 📋 Genel Bakış
Bu API **çok basit yapıda** tasarlanmıştır. **DI (Dependency Injection) kullanılmaz**. Her metodda BLL'ler doğrudan `new` operatörü ile oluşturulur.

## 🔧 Teknik Yaklaşım
- **Basit implementasyon** - Gereksiz karmaşıklık yok
- **DI yapısı yok** - Constructor'larda bile BLL inject edilmez
- **JWT Authentication** - Güvenli login sistemi
- **Role-based Authorization** - 3 farklı kullanıcı rolü
- **File Upload** - Resim yükleme desteği
- **CRUD Operations** - Tam işlevsellik

## 👥 Kullanıcı Rolleri

### 🔓 Guest (Misafir)
- ✅ Menüyü görüntüleyebilir
- ✅ Kategorileri görüntüleyebilir
- ✅ Masaları görüntüleyebilir
- ❌ Sipariş veremez
- ❌ Rezervasyon yapamaz

### 👤 Customer (Müşteri)
- ✅ Guest'in tüm yetkileri
- ✅ Sipariş oluşturabilir
- ✅ Rezervasyon yapabilir
- ✅ Kendi siparişlerini görüntüleyebilir
- ❌ Ürün/kategori yönetimi yapamaz

### ⚡ Admin (Yönetici)
- ✅ Tüm yetkileri var
- ✅ Ürün/kategori yönetimi
- ✅ Masa yönetimi
- ✅ Tüm siparişleri görüntüleyebilir
- ✅ Rezervasyon yönetimi

## 🚀 API'yi Başlatma

```bash
cd Restoran.Api
dotnet run
```

API şu adreste çalışacak: `https://localhost:7xxx`
Swagger UI: `https://localhost:7xxx/swagger`

## 🔐 Authentication Testleri

### 1. Kullanıcı Kayıt
```
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Test",
  "lastName": "User", 
  "username": "testuser",
  "email": "test@example.com",
  "phone": "1234567890",
  "password": "123456"
}
```

### 2. Kullanıcı Giriş
```
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "123456"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Giriş başarılı",
  "data": {
    "user": { /* kullanıcı bilgileri */ },
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

## 📦 Product API Testleri

### 1. Ürün Listesi (Public)
```
GET /api/product
```

### 2. Ürün Detay (Public)
```
GET /api/product/{id}
```

### 3. Ürün Ekleme (Authenticated)
```
POST /api/product
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- Name: "Test Ürün"
- Description: "Test açıklama"
- Price: 25.50
- CategoryId: 1
- StockQuantity: 100
- image: [file]
```

### 4. Ürün Güncelleme (Authenticated)
```
PUT /api/product/{id}
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- Name: "Güncellenmiş Ürün"
- Description: "Güncellenmiş açıklama"
- Price: 30.00
- CategoryId: 1
- StockQuantity: 75
- image: [file] (opsiyonel)
```

### 5. Ürün Silme (Authenticated)
```
DELETE /api/product/{id}
Authorization: Bearer {token}
```

## 🏷️ Category API Testleri

### 1. Kategori Listesi (Public)
```
GET /api/category
```

### 2. Kategori Detay (Public)
```
GET /api/category/{id}
```

### 3. Kategori Ekleme (Authenticated)
```
POST /api/category
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- Name: "Test Kategori"
- Description: "Test açıklama"
- DisplayOrder: 1
- image: [file]
```

### 4. Kategori Güncelleme (Authenticated)
```
PUT /api/category/{id}
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- Name: "Güncellenmiş Kategori"
- Description: "Güncellenmiş açıklama"
- DisplayOrder: 2
- image: [file] (opsiyonel)
```

### 5. Kategori Silme (Authenticated)
```
DELETE /api/category/{id}
Authorization: Bearer {token}
```

## 📦 Order API Testleri

### 1. Sipariş Listesi (Admin Only)
```
GET /api/order
Authorization: Bearer {admin_token}
```

### 2. Sipariş Detay (Customer/Admin)
```
GET /api/order/{id}
Authorization: Bearer {token}
```

### 3. Sipariş Oluşturma (Customer/Admin)
```
POST /api/order
Authorization: Bearer {token}
Content-Type: application/json

{
  "userId": 1,
  "items": [
    {
      "productId": 1,
      "productName": "Pizza",
      "quantity": 2,
      "unitPrice": 45.00
    }
  ]
}
```

### 4. Sipariş Silme (Admin Only)
```
DELETE /api/order/{id}
Authorization: Bearer {admin_token}
```

## 🏢 Reservation API Testleri

### 1. Rezervasyon Listesi (Admin Only)
```
GET /api/reservation
Authorization: Bearer {admin_token}
```

### 2. Rezervasyon Detay (Customer/Admin)
```
GET /api/reservation/{id}
Authorization: Bearer {token}
```

### 3. Rezervasyon Oluşturma (Customer/Admin)
```
POST /api/reservation
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerName": "John Doe",
  "customerPhone": "1234567890",
  "reservationDateTime": "2024-01-15T19:00:00",
  "numberOfGuests": 4,
  "userId": 1,
  "tableId": 1
}
```

### 4. Rezervasyon Güncelleme (Admin Only)
```
PUT /api/reservation/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "customerName": "Jane Doe",
  "customerPhone": "0987654321",
  "reservationDateTime": "2024-01-15T20:00:00",
  "numberOfGuests": 2,
  "status": "Confirmed",
  "tableId": 2
}
```

### 5. Rezervasyon Silme (Admin Only)
```
DELETE /api/reservation/{id}
Authorization: Bearer {admin_token}
```

## 🪑 Table API Testleri

### 1. Masa Listesi (Public)
```
GET /api/table
```

### 2. Masa Detay (Public)
```
GET /api/table/{id}
```

### 3. Masa Ekleme (Admin Only)
```
POST /api/table
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "tableNumber": "T001",
  "capacity": 4,
  "isAvailable": true
}
```

### 4. Masa Güncelleme (Admin Only)
```
PUT /api/table/{id}
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "tableNumber": "T002",
  "capacity": 6,
  "isAvailable": false
}
```

### 5. Masa Silme (Admin Only)
```
DELETE /api/table/{id}
Authorization: Bearer {admin_token}
```

## 📁 File Upload Özellikleri

### Desteklenen Formatlar
- ✅ .jpg, .jpeg, .png
- ❌ Diğer formatlar reddedilir

### Boyut Limiti
- Max: 5MB
- Bu limit aşılırsa hata döner

### Kayıt Yeri
- Resimler `wwwroot/uploads/` dizinine kaydedilir
- Benzersiz dosya adı oluşturulur (GUID)

## 🎯 Response Format

### Başarılı Response
```json
{
  "success": true,
  "data": { /* veri */ },
  "message": "İşlem başarılı"
}
```

### Hata Response
```json
{
  "success": false,
  "message": "Hata mesajı"
}
```

## 🔍 Test Senaryoları

### 1. HTTP Methods Test
- ✅ GET - Veri çekme
- ✅ POST - Veri ekleme
- ✅ PUT - Veri güncelleme
- ✅ DELETE - Veri silme

### 2. Authentication Test
- ✅ Public endpoints (GET Product/Category/Table) - Token gerektirmez
- ✅ Customer endpoints (POST Order/Reservation) - Customer/Admin token
- ✅ Admin endpoints (POST/PUT/DELETE Product/Category) - Admin token
- ✅ Invalid token - 401 hatası
- ✅ Insufficient role - 403 hatası

### 3. Validation Test
- ✅ Required fields - Zorunlu alanlar
- ✅ Data types - Veri tipi kontrolü
- ✅ ModelState - DTO validasyonu

### 4. File Upload Test
- ✅ Valid image - Geçerli resim
- ✅ Invalid format - Geçersiz format
- ✅ Size limit - Boyut limiti
- ✅ No image - Resim olmadan

## 🛠️ Swagger UI Kullanımı

1. **Swagger'a git:** `https://localhost:7xxx/swagger`
2. **Authorize butonuna tıkla**
3. **Token'ı gir:** `Bearer {token}`
4. **Endpoint'leri test et**

## 📝 Notlar

- **DI kullanılmıyor** - Her metodda BLL'ler `new` ile oluşturuluyor
- **Basit yapı** - Gereksiz karmaşıklık yok
- **JWT güvenli** - Token tabanlı authentication
- **File upload güvenli** - Format ve boyut kontrolü
- **CORS açık** - Frontend bağlantısı için

## 🚨 Dikkat Edilecekler

1. **JWT Token** - Login sonrası alınan token'ı sakla
2. **User Roles** - Customer default role, Admin manuel atanır
3. **Authorization Header** - Bearer prefix'i unutma
4. **Role Permissions** - Yetki kontrolü 403 hatası verir
5. **Multipart Form** - Resim yüklerken Content-Type dikkat
6. **File Size** - 5MB limit aşma
7. **File Format** - Sadece jpg, jpeg, png

## 🔐 Test Senaryoları

### Admin User Oluşturma
```sql
-- Database'de manuel admin user oluştur
UPDATE Users SET Role = 3 WHERE Username = 'admin';
```

### Role Test Senaryoları
1. **Guest** - Sadece GET endpoint'leri test et
2. **Customer** - Order/Reservation POST test et
3. **Admin** - Tüm endpoint'leri test et
4. **Wrong Role** - 403 hatası alacağını doğrula 