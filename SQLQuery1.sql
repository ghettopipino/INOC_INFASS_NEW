CREATE TABLE Users
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(255) NOT NULL
);


INSERT INTO Users (FullName, Email, Username, Password)
VALUES ('Earl', 'Cosido@gmail.com', 'earlcosido', '123123');

INSERT INTO Users (FullName, Email, Username, Password)
VALUES ('Earl', 'Cosido@gmail.com', 'earlcosido', '123123');


UPDATE Users
SET FullName = 'Ahearl',
    Email = 'earlbahogtae',
    Username = 'cosidogwapo',
    Password = 'ahearl'
WHERE Id = 1;