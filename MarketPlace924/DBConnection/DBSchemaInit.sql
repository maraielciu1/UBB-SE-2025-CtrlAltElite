DROP TABLE IF EXISTS Users;

CREATE TABLE Users
(
    UserID       INT IDENTITY (1,1) PRIMARY KEY,
    Username     NVARCHAR(100) NOT NULL UNIQUE,
    Email        NVARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber  NVARCHAR(20)  NULL,
    Password     NVARCHAR(255) NOT NULL,
    Role         INT           NOT NULL, -- Example: 1 = Admin, 2 = Seller, 3 = Buyer
    FailedLogins INT DEFAULT 0,
    BannedUntil  DATETIME      NULL,
    IsBanned     BIT DEFAULT 0
);

DROP TABLE IF EXISTS BuyerLinkage;
DROP TABLE IF EXISTS BuyerWishlistItems;
DROP TABLE IF EXISTS Buyers;
DROP TABLE IF EXISTS BuyerAddress;

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
    FOREIGN KEY (BillingAddressId) REFERENCES BuyerAddress (Id),
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

-- Inserare mai mulți Utilizatori
INSERT INTO Users (Username, Email, PhoneNumber, Password, Role, FailedLogins, BannedUntil, IsBanned)
VALUES ('ion_popescu', 'ion.popescu@example.com', '0722-123-456', 'plain:parola123', 2, 0, NULL, 0),
       ('maria_ionescu', 'maria.ionescu@example.com', '0744-987-654', 'plain:florilemeledragi', 2, 0, NULL, 0),
       ('vasile_mihai', 'vasile.mihai@example.com', '0766-654-321', 'plain:sarmale123', 2, 1, '2025-06-01', 1),
       ('elena_georgescu', 'elena.georgescu@example.com', '0733-222-333', 'plain:pisicamea123', 2, 0, NULL, 0),
       ('daniel_mocanu', 'daniel.mocanu@example.com', '0755-444-555', 'plain:contabil2024', 2, 0, NULL, 0),
       ('cristina_matei', 'cristina.matei@example.com', '0788-666-777', 'plain:proiecteIT', 2, 0, null, 0);

-- Inserare mai multe Adrese ale Cumpărătorilor
INSERT INTO BuyerAddress (StreetLine, City, Country, PostalCode)
VALUES ('N/A', 'N/A', 'N/A', 'N/A'),
       ('Strada Mihai Eminescu 24', 'București', 'România', '010001'),
       ('Strada Avram Iancu 35', 'Cluj-Napoca', 'România', '400012'),
       ('Bulevardul Unirii 15', 'Timișoara', 'România', '300123'),
       ('Strada Mărășești 48', 'Iași', 'România', '700456'),
       ('Strada Horea 22', 'Brașov', 'România', '500007');

-- Inserare mai mulți Cumpărători
INSERT INTO Buyers (UserId, FirstName, LastName, BillingAddressId, ShippingAddressId, UseSameAddress, Badge,
                    TotalSpending, NumberOfPurchases, Discount)
VALUES (1, 'Ion', 'Popescu', 1, 2, 1, 'Bronze', 1700.00, 40, 6.00),
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
