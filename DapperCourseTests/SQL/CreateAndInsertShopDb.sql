DROP DATABASE IF EXISTS Shop;
CREATE DATABASE Shop;
USE Shop;

CREATE TABLE Categories (
                          CategoryID INT PRIMARY KEY AUTO_INCREMENT,
                          CategoryName VARCHAR(255)
);

CREATE TABLE Products (
                         ProductID INT PRIMARY KEY AUTO_INCREMENT,
                         ProductName VARCHAR(255),
                         CategoryID INT REFERENCES Categories(CategoryID)
);

INSERT INTO Categories (CategoryName) VALUES ('Fruit');
INSERT INTO Categories (CategoryName) VALUES ('Vegetable');
INSERT INTO Categories (CategoryName) VALUES ('Meat');

INSERT INTO Products (ProductName, CategoryID) VALUES ('Apple', 1);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Banana', 1);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Orange', 1);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Tomato', 2);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Potato', 2);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Carrot', 2);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Beef', 3);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Pork', 3);
INSERT INTO Products (ProductName, CategoryID) VALUES ('Chicken', 3);




