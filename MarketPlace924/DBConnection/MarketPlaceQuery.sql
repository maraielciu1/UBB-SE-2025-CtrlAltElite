USE MarketPlaceDB
GO

-- Drop tables in reverse order of dependencies
DROP TABLE IF EXISTS Reviews;
DROP TABLE IF EXISTS Notifications;
DROP TABLE IF EXISTS Following;
DROP TABLE IF EXISTS BuyerWishlistItems;
DROP TABLE IF EXISTS BuyerLinkage;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Sellers;
DROP TABLE IF EXISTS Buyers;
DROP TABLE IF EXISTS BuyerAddress;
DROP TABLE IF EXISTS Users;

CREATE TABLE Users
(
    UserID       INT IDENTITY (1,1) PRIMARY KEY,
    Username     NVARCHAR(100) NOT NULL UNIQUE,
    Email        NVARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber  NVARCHAR(20)  NULL,
    Password     NVARCHAR(255) NOT NULL,
    Role         INT           NOT NULL, -- Example: 1 = Admin, 2 = Buyer, 3 = Seller
    FailedLogins INT DEFAULT 0,
    BannedUntil  DATETIME      NULL,
    IsBanned     BIT DEFAULT 0
);


CREATE TABLE BuyerAddress
(
    Id         int identity (0,1) PRIMARY KEY,
    StreetLine nvarchar(255) NOT NULL,
    City       nvarchar(255) NOT NULL,
    Country    nvarchar(255) NOT NULL,
    PostalCode nvarchar(255) NOT NULL,
);
CREATE TABLE Buyers
(
    UserId            Int            NOT NULL PRIMARY KEY,
    FirstName         nvarchar(255)  NOT NULL,
    LastName          nvarchar(255)  NOT NULL,
    BillingAddressId  int            NOT NULL,
    ShippingAddressId int            NOT NULL,
    UseSameAddress    bit,
    Badge             nvarchar(10),
    TotalSpending     NUMERIC(32, 2) NOT NULL,
    NumberOfPurchases int            not null,
    Discount          NUMERIC(32, 2) NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users (UserId),
    FOREIGN KEY (BillingAddressId) REFERENCES BuyerAddress (Id),
);


CREATE TABLE BuyerLinkage
(
    RequestingBuyerId INT NOT NULL,
    ReceivingBuyerId  INT NOT NULL,
    IsApproved        BIT NOT NULL,
    FOREIGN KEY (ReceivingBuyerId) REFERENCES Buyers (UserId),
    FOREIGN KEY (RequestingBuyerId) REFERENCES Buyers (UserId),
    PRIMARY KEY (RequestingBuyerId, ReceivingBuyerId),
    CHECK (RequestingBuyerId <> ReceivingBuyerId)
);


CREATE TABLE BuyerWishlistItems
(
    BuyerId   int not null,
    ProductId int not null,
    PRIMARY KEY (BuyerId, ProductId),
    FOREIGN KEY (BuyerId) references Buyers (UserId),
);


CREATE Table Sellers(
	UserId INT NOT NULL PRIMARY KEY,
	Username NVARCHAR(100) NOT NULL UNIQUE,
	StoreName NVARCHAR(100),
	StoreDescription NVARCHAR(255),
	StoreAddress NVARCHAR(100),
	FollowersCount INT,
	TrustScore FLOAT,
	FOREIGN KEY(UserId) REFERENCES Users(UserID),
);


CREATE TABLE Notifications(
	NotificationID INT IDENTITY(1,1) PRIMARY KEY,
	SellerID INT,
	NotificationMessage NVARCHAR(100),
	NotificationFollowerCount INT,
	FOREIGN KEY(SellerID) REFERENCES Sellers(UserId),
);


CREATE TABLE Products(
	ProductID INT IDENTITY(1,1) PRIMARY KEY,
	SellerID INT,
	ProductName NVARCHAR(100),
	ProductDescription NVARCHAR(100),
	ProductPrice FLOAT,
	ProductStock INT,
	FOREIGN KEY(SellerID) REFERENCES Sellers(UserId),
);


CREATE TABLE Following (
    FollowerID INT NOT NULL,
    FollowedID INT NOT NULL,
    PRIMARY KEY (FollowerID, FollowedID),
    FOREIGN KEY (FollowerID) REFERENCES Buyers(UserId),
    FOREIGN KEY (FollowedID) REFERENCES Sellers(UserId)
);

CREATE INDEX idx_users_email ON Users (Email);


-- Inserare mai mulți Utilizatori (Atât Cumpărători, cât și Vânzători)
INSERT INTO Users (Username, Email, PhoneNumber, Password, Role, FailedLogins, BannedUntil, IsBanned)
VALUES 
    -- Buyers
    ('ion_popescu', 'ion.popescu@example.com', '+40722123456', 'Ctcwb4sLby/TWhvYGiwtX0g0ZmnyhzE6ZitdwvBasfs=', 2, 0, NULL, 0), -- Parola123!
    ('maria_ionescu', 'maria.ionescu@example.com', '+40744987654', 'lKI5kNt7dmvv5ce2y2SOlxNwKwmfnWvPxt3vsuI0Tc8=', 2, 0, NULL, 0), --Flori123!
    ('vasile_mihai', 'vasile.mihai@example.com', '+40766654321', 'C/OvJ2LdQTiJKVRqXG6mHaAYpgrMkxJHq7X4QE/oJL8=', 2, 1, '2025-06-01', 1), --Sarmale123!
    ('elena_georgescu', 'elena.georgescu@example.com', '+40733222333', 'fyfsnf1pkujB1uCjIjWI3G5UrqtuQJNwDgEyQafC5zE=', 2, 0, NULL, 0), --Pisica123!
    ('daniel_mocanu', 'daniel.mocanu@example.com', '+40755444555', 'pC4o/0wFC4CYGV/xDmJ2SVnYm0ghfMr2WTnMKK9eW1M=', 2, 0, NULL, 0), --Conta123!
    ('cristina_matei', 'cristina.matei@example.com', '+40788666777', 'Tty5SkN1j7eweWRQpp9l42MHOm6pwUUoT5oWq/KY64M=', 2, 0, NULL, 0), --Proiect123!

    -- Sellers
    ('andrei_vasile', 'andrei.vasile@example.com', '+40711222333', 'CQNo0x06klJSW9cFSwQtHU2bFcPo8tOCTBNUtb4eomk=', 3, 0, NULL, 0), --Tech123!
    ('florentina_petre', 'florentina.petre@example.com', '+40723456789', 'lKI5kNt7dmvv5ce2y2SOlxNwKwmfnWvPxt3vsuI0Tc8=', 3, 0, NULL, 0), --Flori123!
    ('mihai_dumitru', 'mihai.dumitru@example.com', '+40734567890', '5lWQKR5CXo6H8TGc2LK9G+zT6ytV255Tk2LLjqcPOIk=', 3, 0, NULL, 0), --Gadgets123!
    ('ana_marinescu', 'ana.marinescu@example.com', '+40745678901', 'LqGI41pLxLBJ0iAaFDsqp3IkIVAZ7fySZNonV3Py1Oc=', 3, 0, NULL, 0), --Fashion123!
    ('bogdan_radu', 'bogdan.radu@example.com', '+40756789012', 'NDfzi9sxFOG82y2Vfxw5CyI4+8gADb1zi5nkbI8Kq/E=', 3, 0, NULL, 0), --Soft123!
    ('camelia_stan', 'camelia.stan@example.com', '+40767890123', '5g7+q9D7pRlGEN2qEeb3C8ZVWk44pqX5eO7pJgNcBGY=', 3, 0, NULL, 0), --Home123!

	-- Admins
	('admin romania', 'admin@example.ro', '+40123456789', 'UriPdLj2JbgULItEJ8HrHxBJ41j0WTrnUfTrbFBT/Uk=', 1, 0, NULL, 0), --Admin1!a
	('admin america', 'admin@example.com', '+40987654321', 'FDYYrYTEKQ1DBu88XcBJ+wdUEqXPPSLoG8REcoHwyQg=', 1, 0, NULL, 0); -- AdminUS123!



-- Inserare mai multe Adrese pentru Cumpărători
INSERT INTO BuyerAddress (StreetLine, City, Country, PostalCode)
VALUES 
    ('N/A', 'N/A', 'N/A', 'N/A'),
    ('Strada Mihai Eminescu 24', 'București', 'România', '010001'),
    ('Strada Avram Iancu 35', 'Cluj-Napoca', 'România', '400012'),
    ('Bulevardul Unirii 15', 'Timișoara', 'România', '300123'),
    ('Strada Mărășești 48', 'Iași', 'România', '700456'),
    ('Strada Horea 22', 'Brașov', 'România', '500007');

-- Inserare Cumpărători în Buyers
INSERT INTO Buyers (UserId, FirstName, LastName, BillingAddressId, ShippingAddressId, UseSameAddress, Badge, TotalSpending, NumberOfPurchases, Discount)
VALUES 
    (1, 'Ion', 'Popescu', 1, 2, 1, 'Bronze', 1700.00, 40, 6.00),
    (2, 'Maria', 'Ionescu', 2, 2, 1, 'Gold', 2800.00, 60, 9.00),
    (3, 'Vasile', 'Mihai', 3, 2, 1, 'Silver', 900.00, 22, 3.00),
    (4, 'Elena', 'Georgescu', 2, 3, 1, 'Silver', 7000.00, 120, 18.00),
    (5, 'Daniel', 'Mocanu', 1, 1, 1, 'Bronze', 2500.00, 50, 7.00),
    (6, 'Cris', 'Matei', 0, 0, 1, 'Bronze', 50.00, 1, 0.00);

-- Inserare mai multe Conexiuni între Cumpărători
INSERT INTO BuyerLinkage (RequestingBuyerId, ReceivingBuyerId, IsApproved)
VALUES (1, 2, 1),
       (1, 3, 1),
       (1, 4, 1),
       (2, 3, 1),
       (2, 5, 1),
       (2, 4, 1),
       (3, 4, 0),
       (4, 5, 1),
       (5, 1, 0);

-- Inserare mai multe Produse în Wishlist-ul Cumpărătorilor
INSERT INTO BuyerWishlistItems (BuyerId, ProductId)
VALUES (1, 6),
       (1, 1),
       (1, 2),
       (1, 3),
       (2, 4),
       (2, 8),
       (2, 2),
       (2, 3),
       (2, 18),
       (2, 17),
       (3, 9),
       (3, 10),
       (3, 11),
       (3, 12),
       (4, 13),
       (4, 14),
       (4, 15),
       (4, 16),
       (5, 11),
       (5, 12),
       (5, 13);


-- Inserare Vânzători în Sellers
INSERT INTO Sellers (UserId, Username, StoreName, StoreDescription, StoreAddress, FollowersCount, TrustScore)
VALUES 
    (7, 'andrei_vasile', 'Tech Shop', 'Gadget-uri de ultimă generație.', 'Strada IT 45, București', 300, 4.9),
    (8, 'florentina_petre', 'Florăria Petre', 'Flori proaspete zilnic.', 'Strada Rozelor 22, Cluj-Napoca', 120, 4.7),
    (9, 'mihai_dumitru', 'Gadget Market', 'Telefoane și accesorii moderne.', 'Bulevardul Tehnologiei 10, Timișoara', 180, 4.6),
    (10, 'ana_marinescu', 'Fashion Boutique', 'Haine de designer pentru orice ocazie.', 'Strada Modei 8, Iași', 250, 4.8),
    (11, 'bogdan_radu', 'SoftDev Solutions', 'Servicii de dezvoltare software.', 'Strada Programatorilor 5, Brașov', 150, 4.5),
    (12, 'camelia_stan', 'Home Deco', 'Mobilă și decorațiuni pentru casă.', 'Strada Decorului 12, Sibiu', 100, 4.4);

-- Inserare Produse
INSERT INTO Products (SellerID, ProductName, ProductDescription, ProductPrice, ProductStock)
VALUES 
    (7, 'Laptop ASUS', 'Laptop performant pentru birou.', 3500.00, 10),
    (7, 'Mouse Wireless', 'Mouse ergonomic pentru utilizare zilnică.', 150.00, 30),
    (8, 'Buchet de trandafiri', 'Buchet elegant cu 15 trandafiri roșii.', 120.00, 20),
    (8, 'Lalele galbene', 'Set de 10 lalele proaspete.', 75.00, 25),
    (9, 'Smartphone Samsung', 'Telefon inteligent cu ecran AMOLED.', 2800.00, 15),
    (9, 'Căști Bluetooth', 'Căști fără fir cu autonomie de 10h.', 300.00, 40),
    (10, 'Rochie de seară', 'Rochie elegantă pentru evenimente speciale.', 600.00, 8),
    (10, 'Geantă din piele', 'Geantă premium din piele naturală.', 850.00, 12),
    (11, 'Servicii de web design', 'Creare site-uri personalizate.', 2000.00, 5),
    (11, 'Software CRM', 'Soluție CRM pentru afaceri mici.', 5000.00, 3),
    (12, 'Vază decorativă', 'Vază din ceramică pictată manual.', 180.00, 20),
    (12, 'Tablou abstract', 'Tablou modern pentru living.', 400.00, 7);

-- Inserare relații de "Following" (Cine pe cine urmărește)
INSERT INTO Following (FollowerID, FollowedID)
VALUES
    (1, 7),  -- Ion Popescu -> Tech Shop
    (1, 8),  -- Ion Popescu -> Florăria Petre
    (2, 9),  -- Maria Ionescu -> Gadget Market
    (3, 10), -- Vasile Mihai -> Fashion Boutique
    (4, 7),  -- Elena Georgescu -> Tech Shop
    (4, 11), -- Elena Georgescu -> SoftDev Solutions
    (5, 12), -- Daniel Mocanu -> Home Deco
    (6, 8);  -- Cristina Matei -> Florăria Petre

-- ultima notificare sa aiba nr de followers ca userul

INSERT INTO Notifications (SellerID, NotificationMessage, NotificationFollowerCount)
VALUES 
	(7, 'You gained 17 followers!', 292),
	(7, 'You have reached 300 followers!', 300),

	(8, 'You gained 3 followers!', 105),
    (8, 'You gained 10 followers!', 115),
    (8, 'You gained 4 followers!', 119),
    (8, 'You have reached 120 followers!', 120),

    (9, 'You gained 5 followers! Total followers: 185', 185),
    (9, 'You have reached 180 followers!', 180),

	(10, 'You gained 30 followers!', 240),
    (10, 'You have reached 250 followers!', 250),

	(11, 'You gained 23 followers!', 145),
    (11, 'You have reached 150 followers!', 150),

    (12, 'You gained 8 followers!', 94),
    (12, 'You have reached 100 followers!', 100);


CREATE TABLE Reviews 
(
	ReviewId	INT IDENTITY (1,1) PRIMARY KEY, 
	SellerId	INT NOT NULL, 
	Score		FLOAT ,
	Foreign key (SellerId) REFERENCES Sellers(UserId)
)

-- Mock data for Reviews table
INSERT INTO Reviews (SellerId, Score) VALUES 
(7, 4.9), (7, 5.0), (7, 4.8), (7, 4.7), (7, 4.9), (7, 5.0), -- Avg ~4.9
(8, 4.8), (8, 4.7), (8, 4.6), (8, 4.7), (8, 4.8), (8, 4.9), -- Avg ~4.7
(9, 4.5), (9, 4.6), (9, 4.7), (9, 4.5), (9, 4.7), (9, 4.6), -- Avg ~4.6
(10, 4.9), (10, 4.8), (10, 4.7), (10, 4.8), (10, 4.9), (10, 4.9), -- Avg ~4.8
(11, 4.4), (11, 4.5), (11, 4.6), (11, 4.5), (11, 4.4), (11, 4.5), -- Avg ~4.5
(12, 4.3), (12, 4.4), (12, 4.5), (12, 4.2), (12, 4.3), (12, 4.4); -- Avg ~4.4

