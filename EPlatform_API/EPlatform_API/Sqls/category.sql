-- Insert root categories (main categories)
INSERT INTO Categories (CategoryParentId, Name, Slug, Description, ImgUrl, IsActive, CreatedAt, UpdatedAt, Code)
VALUES 
(NULL, 'Electronics', 'electronics', 'Electronics and gadgets', 'https://hoanghuystorage.blob.core.windows.net/public-images/electronic.webp', 1, GETDATE(), GETDATE(), 'ELEC'),
(NULL, 'Fashion', 'fashion', 'Clothing and accessories', 'https://hoanghuystorage.blob.core.windows.net/public-images/fashion.webp', 1, GETDATE(), GETDATE(), 'FASH'),
(NULL, 'Home and Kitchen', 'home-kitchen', 'Home and kitchen essentials', 'https://hoanghuystorage.blob.core.windows.net/public-images/home-and-kitchen.webp', 1, GETDATE(), GETDATE(), 'HOME'),
(NULL, 'Beauty and Personal Care', 'beauty-personal-care', 'Beauty and grooming products', 'https://hoanghuystorage.blob.core.windows.net/public-images/beauty-and-personal-care.webp', 1, GETDATE(), GETDATE(), 'BEAU'),
(NULL, 'Sports and Outdoors', 'sports-outdoors', 'Sports gear and outdoor items', 'https://hoanghuystorage.blob.core.windows.net/public-images/Sports-and-Outdoors.webp', 1, GETDATE(), GETDATE(), 'SPRT'),
(NULL, 'Baby and Kids', 'baby-kids', 'Baby care and kids products', 'https://hoanghuystorage.blob.core.windows.net/public-images/Baby-and-Kids.webp', 1, GETDATE(), GETDATE(), 'BABY'),
(NULL, 'Books and Media', 'books-media', 'Books, music, and movies', 'https://hoanghuystorage.blob.core.windows.net/public-images/Books-and-Media.webp', 1, GETDATE(), GETDATE(), 'BOOK'),
(NULL, 'Groceries and Essentials', 'groceries-essentials', 'Daily groceries and staples', 'https://hoanghuystorage.blob.core.windows.net/public-images/Groceries-and-Essentials.webp', 1, GETDATE(), GETDATE(), 'GROC'),
(NULL, 'Health and Wellness', 'health-wellness', 'Health and wellness items', 'https://hoanghuystorage.blob.core.windows.net/public-images/Health-and-Wellness.webp', 1, GETDATE(), GETDATE(), 'HEAL'),
(NULL, 'Automotive', 'automotive', 'Car and bike accessories', 'https://hoanghuystorage.blob.core.windows.net/public-images/Automotive.webp', 1, GETDATE(), GETDATE(), 'AUTO'),
(NULL, 'Office Supplies', 'office-supplies', 'Office supplies and furniture', 'https://hoanghuystorage.blob.core.windows.net/public-images/Office-Supplies.webp', 1, GETDATE(), GETDATE(), 'OFFC'),
(NULL, 'Pets', 'pets', 'Pet care and accessories', 'https://hoanghuystorage.blob.core.windows.net/public-images/pet.webp', 1, GETDATE(), GETDATE(), 'PETS'),
(NULL, 'Toys and Hobbies', 'toys-hobbies', 'Toys and hobby items', 'https://hoanghuystorage.blob.core.windows.net/public-images/Toys-and-Hobbies.webp', 1, GETDATE(), GETDATE(), 'TOYS'),
(NULL, 'Travel and Luggage', 'travel-luggage', 'Travel gear and bags', 'https://hoanghuystorage.blob.core.windows.net/public-images/Travel-and-Luggage.webp', 1, GETDATE(), GETDATE(), 'TRAV'),
(NULL, 'Tools and Hardware', 'tools-hardware', 'Tools and hardware items', 'https://hoanghuystorage.blob.core.windows.net/public-images/Tools-and-Hardware.webp', 1, GETDATE(), GETDATE(), 'TOOL'),
(NULL, 'Another', 'another-class', 'Another classify', 'https://hoanghuystorage.blob.core.windows.net/public-images/600x400.png', 1, GETDATE(), GETDATE(), 'ANOTHER');


