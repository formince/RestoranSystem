-- GitHub Action Testi için geçici veri ekleme
INSERT INTO Categories (
    Name, 
    Description, 
    DisplayOrder, 
    ImageUrl, 
    CreatedDate, 
    UpdatedDate, 
    IsActive
) 
VALUES (
    'GitHub Test Kategorisi', 
    'Bu kayýt otomatik deploy testi sýrasýnda eklendi.', 
    999, -- En sona atsýn diye yüksek numara verdik
    'no-image.jpg', 
    GETDATE(), 
    GETDATE(), 
    1
);