-- Insert subcategories for 'Electronics'
INSERT INTO Categories (CategoryParentId, Name, Slug, Description, ImgUrl, IsActive, CreatedAt, UpdatedAt, Code)
VALUES 
(73, 'Mobile Phones', 'mobile-phones', 'Smartphones of all brands', NULL, 1, GETDATE(), GETDATE(), 'MOBL'),
(73, 'Laptops', 'laptops', 'Laptops and notebooks', NULL, 1, GETDATE(), GETDATE(), 'LAPT'),
(73, 'Tablets', 'tablets', 'Tablets and e-readers', NULL, 1, GETDATE(), GETDATE(), 'TABL'),
(73, 'Televisions', 'televisions', 'Smart TVs and LED TVs', NULL, 1, GETDATE(), GETDATE(), 'TV'),
(73, 'Headphones & Earphones', 'headphones-earphones', 'Audio devices', NULL, 1, GETDATE(), GETDATE(), 'HEAD');

-- Insert subcategories for 'Fashion'
INSERT INTO Categories (CategoryParentId, Name, Slug, Description, ImgUrl, IsActive, CreatedAt, UpdatedAt, Code)
VALUES 
(74, 'Men’s Clothing', 'mens-clothing', 'Clothing for men', NULL, 1, GETDATE(), GETDATE(), 'MCLTH'),
(74, 'Women’s Clothing', 'womens-clothing', 'Clothing for women', NULL, 1, GETDATE(), GETDATE(), 'WCLTH'),
(74, 'Kids’ Clothing', 'kids-clothing', 'Clothing for kids', NULL, 1, GETDATE(), GETDATE(), 'KCLTH'),
(74, 'Footwear', 'footwear', 'Shoes and sandals', NULL, 1, GETDATE(), GETDATE(), 'FTWR'),
(74, 'Jewelry', 'jewelry', 'Fashion jewelry and ornaments', NULL, 1, GETDATE(), GETDATE(), 'JEWL');

-- Repeat similarly for all other categories and subcategories