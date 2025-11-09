# Kaynak: Publish klasörü
$source = "C:\Users\Yunus\source\repos\RestoranSistemi\Restoran.Web\bin\Debug\net8.0"
# Hedef: Test klasörü
$target = "C:\Users\Yunus\Desktop\TestDeploy"

# Eski dosyaları temizle
if (Test-Path $target) {
    Remove-Item "$target\*" -Recurse -Force
    Write-Host "Eski dosyalar temizlendi."
} else {
    New-Item -ItemType Directory -Path $target
    Write-Host "Test deploy klasörü oluşturuldu."
}

# Yeni dosyaları kopyala ve her dosyayı yazdır
Get-ChildItem -Path $source -Recurse | ForEach-Object {
    $dest = $_.FullName.Replace($source, $target)
    if ($_.PSIsContainer) {
        if (-not (Test-Path $dest)) { 
            New-Item -ItemType Directory -Path $dest | Out-Null
            Write-Host "Klasör oluşturuldu: $dest"
        }
    } else {
        Copy-Item $_.FullName $dest
        Write-Host "Kopyalandı: $($_.FullName) -> $dest"
    }
}

# IIS restart simülasyonu
Write-Host "Deploy tamamlandı! (IIS restart simülasyonu)"
