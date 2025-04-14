-- Insert subcategories for 'Electronics'
INSERT INTO Categories (CategoryParentId, Name, Slug, Description, ImgUrl, IsActive, CreatedAt, UpdatedAt, Code)
VALUES 
(1, 'Mobile Phones', 'mobile-phones', 'Smartphones of all brands', NULL, 1, GETDATE(), GETDATE(), 'MOBL'),
(1, 'Laptops', 'laptops', 'Laptops and notebooks', NULL, 1, GETDATE(), GETDATE(), 'LAPT'),
(1, 'Tablets', 'tablets', 'Tablets and e-readers', NULL, 1, GETDATE(), GETDATE(), 'TABL'),
(1, 'Televisions', 'televisions', 'Smart TVs and LED TVs', NULL, 1, GETDATE(), GETDATE(), 'TV'),
(1, 'Headphones & Earphones', 'headphones-earphones', 'Audio devices', NULL, 1, GETDATE(), GETDATE(), 'HEAD');

-- Insert subcategories for 'Fashion'
INSERT INTO Categories (CategoryParentId, Name, Slug, Description, ImgUrl, IsActive, CreatedAt, UpdatedAt, Code)
VALUES 
(2, 'Men’s Clothing', 'mens-clothing', 'Clothing for men', NULL, 1, GETDATE(), GETDATE(), 'MCLTH'),
(2, 'Women’s Clothing', 'womens-clothing', 'Clothing for women', NULL, 1, GETDATE(), GETDATE(), 'WCLTH'),
(2, 'Kids’ Clothing', 'kids-clothing', 'Clothing for kids', NULL, 1, GETDATE(), GETDATE(), 'KCLTH'),
(2, 'Footwear', 'footwear', 'Shoes and sandals', NULL, 1, GETDATE(), GETDATE(), 'FTWR'),
(2, 'Jewelry', 'jewelry', 'Fashion jewelry and ornaments', NULL, 1, GETDATE(), GETDATE(), 'JEWL');

-- Repeat similarly for all other categories and subcategories