DROP TABLE IF EXISTS Users;

CREATE TABLE Users
(
    UserID       INT IDENTITY (1,1) PRIMARY KEY,
    Username     NVARCHAR(100) NOT NULL UNIQUE,
    Email        NVARCHAR(255) NOT NULL UNIQUE,
    PhoneNumber  NVARCHAR(20)  NULL,
    Password     NVARCHAR(255) NOT NULL,
    Role         INT NOT NULL, -- Example: 1 = Admin, 2 = Seller, 3 = Buyer
    FailedLogins INT DEFAULT 0,
    BannedUntil  DATETIME NULL,
    IsBanned     BIT DEFAULT 0
);

-- Insert some mock data
INSERT INTO Users (Username, Email, PhoneNumber, Password, Role, FailedLogins, BannedUntil, IsBanned)
VALUES ('john_doe', 'john.doe@example.com', '123-456-7890', '123', 1, 0, NULL, 0),
       ('alice_smith', 'alice.smith@example.com', '987-654-3210', '456', 2, 2, '2025-04-01', 1),
       ('bob_johnson', 'bob.johnson@example.com', '555-123-4567', '789', 3, 0, NULL, 0);


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
    Discount          NUMERIC(32, 2) NOT NULL,
    FOREIGN KEY (BillingAddressId) REFERENCES BuyerAddress (Id),
    FOREIGN KEY (BillingAddressId) REFERENCES BuyerAddress (Id),
);


INSERT INTO BuyerAddress(StreetLine, City, Country, PostalCode)
VALUES ('N/A', 'N/A', 'N/A', 'N/A');
INSERT INTO Buyers(UserId, FirstName, LastName, BillingAddressId, ShippingAddressId, UseSameAddress, Badge,
                   TotalSpending, Discount)
VALUES (1, 'John', 'Doe', 0, 0, 1, 'Bronze', 0, 0);
