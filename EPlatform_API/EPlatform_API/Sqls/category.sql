-- Insert root categories (main categories)
INSERT INTO Categories (CategoryParentId, Name, Slug, Description, ImgUrl, IsActive, CreatedAt, UpdatedAt, Code)
VALUES 
(NULL, 'Electronics', 'electronics', 'Electronics and gadgets', 'https://sinhnguyen417.blob.core.windows.net/categories-image/electronic.webp', 1, GETDATE(), GETDATE(), 'ELEC'),
(NULL, 'Fashion', 'fashion', 'Clothing and accessories', 'https://sinhnguyen417.blob.core.windows.net/categories-image/fashion.webp', 1, GETDATE(), GETDATE(), 'FASH'),
(NULL, 'Home and Kitchen', 'home-kitchen', 'Home and kitchen essentials', 'https://sinhnguyen417.blob.core.windows.net/categories-image/home-and-kitchen.webp', 1, GETDATE(), GETDATE(), 'HOME'),
(NULL, 'Beauty and Personal Care', 'beauty-personal-care', 'Beauty and grooming products', 'https://sinhnguyen417.blob.core.windows.net/categories-image/beauty-and-personal-care.webp', 1, GETDATE(), GETDATE(), 'BEAU'),
(NULL, 'Sports and Outdoors', 'sports-outdoors', 'Sports gear and outdoor items', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Sports-and-Outdoors.webp', 1, GETDATE(), GETDATE(), 'SPRT'),
(NULL, 'Baby and Kids', 'baby-kids', 'Baby care and kids products', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Baby-and-Kids.webp', 1, GETDATE(), GETDATE(), 'BABY'),
(NULL, 'Books and Media', 'books-media', 'Books, music, and movies', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Books-and-Media.webp', 1, GETDATE(), GETDATE(), 'BOOK'),
(NULL, 'Groceries and Essentials', 'groceries-essentials', 'Daily groceries and staples', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Groceries-and-Essentials.webp', 1, GETDATE(), GETDATE(), 'GROC'),
(NULL, 'Health and Wellness', 'health-wellness', 'Health and wellness items', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Health-and-Wellness.webp', 1, GETDATE(), GETDATE(), 'HEAL'),
(NULL, 'Automotive', 'automotive', 'Car and bike accessories', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Automotive.webp', 1, GETDATE(), GETDATE(), 'AUTO'),
(NULL, 'Office Supplies', 'office-supplies', 'Office supplies and furniture', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Office-Supplies.webp', 1, GETDATE(), GETDATE(), 'OFFC'),
(NULL, 'Pets', 'pets', 'Pet care and accessories', 'https://sinhnguyen417.blob.core.windows.net/categories-image/pet.webp', 1, GETDATE(), GETDATE(), 'PETS'),
(NULL, 'Toys and Hobbies', 'toys-hobbies', 'Toys and hobby items', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Toys-and-Hobbies.webp', 1, GETDATE(), GETDATE(), 'TOYS'),
(NULL, 'Travel and Luggage', 'travel-luggage', 'Travel gear and bags', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Travel-and-Luggage.webp', 1, GETDATE(), GETDATE(), 'TRAV'),
(NULL, 'Tools and Hardware', 'tools-hardware', 'Tools and hardware items', 'https://sinhnguyen417.blob.core.windows.net/categories-image/Tools-and-Hardware.webp', 1, GETDATE(), GETDATE(), 'TOOL'),
(NULL, 'Another', 'another-class', 'Another classify', 'https://sinhnguyen417.blob.core.windows.net/public-images/600x400.png', 1, GETDATE(), GETDATE(), 'ANOTHER');


