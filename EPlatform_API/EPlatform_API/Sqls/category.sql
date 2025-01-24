-- Insert root categories (main categories)
INSERT INTO Categories (CategoryParentId, Name, Slug, Description, ImgUrl, IsActive, CreatedAt, UpdatedAt)
VALUES 
(NULL, 'Electronics', 'electronics', 'Electronics and gadgets', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Fashion', 'fashion', 'Clothing and accessories', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Home and Kitchen', 'home-kitchen', 'Home and kitchen essentials', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Beauty and Personal Care', 'beauty-personal-care', 'Beauty and grooming products', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Sports and Outdoors', 'sports-outdoors', 'Sports gear and outdoor items', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Baby and Kids', 'baby-kids', 'Baby care and kids products', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Books and Media', 'books-media', 'Books, music, and movies', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Groceries and Essentials', 'groceries-essentials', 'Daily groceries and staples', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Health and Wellness', 'health-wellness', 'Health and wellness items', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Automotive', 'automotive', 'Car and bike accessories', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Office Supplies', 'office-supplies', 'Office supplies and furniture', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Pets', 'pets', 'Pet care and accessories', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Toys and Hobbies', 'toys-hobbies', 'Toys and hobby items', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Travel and Luggage', 'travel-luggage', 'Travel gear and bags', NULL, 1, GETDATE(), GETDATE()),
(NULL, 'Tools and Hardware', 'tools-hardware', 'Tools and hardware items', NULL, 1, GETDATE(), GETDATE());



