# Alumni Portal — Mezun Yönetim Sistemi

Antalya Belek Üniversitesi mezunlarını dijital ortamda yönetmek ve mezunlar arasında iletişim kurmak amacıyla geliştirilmiş web tabanlı bir sistemdir.

---

## Proje Amacı

- Mezun kayıtlarını merkezi bir sistemde tutmak
- Kariyer gelişimlerini takip etmek
- Mezunlar arasında gerçek zamanlı iletişim sağlamak
- Duyuru ve istatistik yönetimi sunmak

---

## Teknoloji Stack

| Katman | Teknoloji |
|---|---|
| Backend | ASP.NET Core MVC (.NET 10) |
| Veritabanı | PostgreSQL (NeonDB) |
| ORM | Entity Framework Core |
| Frontend | Bootstrap 5 + Bootstrap Icons |
| Gerçek Zamanlı | SignalR |
| Kimlik Doğrulama | Cookie Authentication |
| Dil Desteği | ASP.NET Core Localization (TR/EN/DE) |
| PDF Export | QuestPDF |
| Excel Export | EPPlus |

---

## Özellikler

- Kayıt & Giriş (Admin onay mekanizması)
- Profil yönetimi (fotoğraf, kariyer bilgisi)
- Admin paneli (mezun listesi, onay, istatistikler)
- Duyuru yönetimi (CRUD)
- Gerçek zamanlı mesajlaşma (SignalR)
- Excel ve PDF dışa aktarma
- Türkçe / İngilizce / Almanca dil desteği

---

## Kurulum

### Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL bağlantısı (NeonDB veya yerel)

### Adımlar

**1. Repoyu klonlayın**
```bash
git clone https://github.com/alperensen/YAZ-Tasarim-Projeleri.git
cd YAZ-Tasarim-Projeleri
git checkout bu-mezun-bilgi
cd Alumni_Portal
```

**2. Yapılandırma dosyasını oluşturun**

`appsettings.example.json` dosyasını kopyalayıp `appsettings.json` olarak yeniden adlandırın ve aşağıdaki alanları doldurun:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
  },
  "EmailSettings": {
    "SenderEmail": "...",
    "SenderPassword": "..."
  }
}
```

**3. Veritabanı migration**
```bash
dotnet ef database update
```

**4. Uygulamayı çalıştırın**
```bash
dotnet run
```

Uygulama varsayılan olarak `https://localhost:5001` adresinde açılır.

---

## Veritabanı Yapısı

**Schema: `belek_graduate`**
- `graduate` — Mezun kişisel bilgileri
- `graduatecareer` — Kariyer bilgileri
- `announcement` — Duyurular
- `messages` — Mesajlar

**Schema: `public`**
- `users` — Kullanıcı hesapları
- `user_schemas` — Şema yetkileri

---

## Varsayılan Admin Hesabı

```
E-posta : admin@alumni.com
Şifre   : admin123
```

---

## Proje Yapısı

```
Alumni_Portal/
├── Controllers/        # MVC Controller'lar
├── Models/             # Veritabanı modelleri
├── ViewModels/         # View modelleri
├── Views/              # Razor sayfaları
├── Hubs/               # SignalR Hub
├── Services/           # E-posta servisi
├── Resources/          # Çok dil dosyaları (.resx)
└── wwwroot/            # Statik dosyalar
```