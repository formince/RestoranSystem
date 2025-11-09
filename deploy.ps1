# Kaynak: Publish klasörü
$source = "C:\Users\Yunus\source\repos\RestoranSistemi\Restoran.Web\bin\Debug\net8.0"
# Hedef: Test klasörü
$target = "C:\Users\Yunus\Desktop\TestDeploy"

# Renkli mesaj için
function Write-Success($message) { Write-Host "✅ $message" -ForegroundColor Green }
function Write-WarningMsg($message) { Write-Host "⚠️ $message" -ForegroundColor Yellow }
function Write-ErrorMsg($message) { Write-Host "❌ $message" -ForegroundColor Red }

# Eski dosyaları temizle
if (Test-Path $target) {
    Remove-Item "$target\*" -Recurse -Force
    Write-Success "Eski dosyalar temizlendi."
} else {
    New-Item -ItemType Directory -Path $target
    Write-Success "Test deploy klasörü oluşturuldu."
}

# Dosyaları kopyala ve logla
Get-ChildItem -Path $source -Recurse | ForEach-Object {
    $dest = $_.FullName.Replace($source, $target)
    if ($_.PSIsContainer) {
        if (-not (Test-Path $dest)) { 
            New-Item -ItemType Directory -Path $dest | Out-Null
            Write-Success "Klasör oluşturuldu: $dest"
        }
    } else {
        try {
            Copy-Item $_.FullName $dest -ErrorAction Stop
            Write-Success "Kopyalandı: $($_.FullName) -> $dest"
        } catch {
            Write-ErrorMsg "Kopyalanamadı: $($_.FullName) -> $dest"
        }
    }
}

Write-Success "🎉 Deploy tamamlandı! (IIS restart simülasyonu)"
