DROP DATABASE IF EXISTS Books;
CREATE DATABASE Books;
USE Books;

CREATE TABLE Authors (
                         Id INT NOT NULL AUTO_INCREMENT,
                         FirstName VARCHAR(50) NOT NULL,
                         LastName VARCHAR(50) NOT NULL,
                         PRIMARY KEY (Id)
);

CREATE TABLE Books (
                       Id INT NOT NULL AUTO_INCREMENT,
                       Title VARCHAR(100) NOT NULL,
                       AuthorId INT NOT NULL,
                       PRIMARY KEY (Id),
                       FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
);

INSERT INTO Authors (FirstName, LastName) VALUES ('Jane', 'Austen');
INSERT INTO Books (Title, AuthorId) VALUES ('Pride and Prejudice', 1);
INSERT INTO Books (Title, AuthorId) VALUES ('Emma', 1);
INSERT INTO Books (Title, AuthorId) VALUES ('Persuasion', 1);
INSERT INTO Books (Title, AuthorId) VALUES ('Northanger Abbey', 1);

INSERT INTO Authors (FirstName, LastName) VALUES ('Charles', 'Dickens');
INSERT INTO Books (Title, AuthorId) VALUES ('Oliver Twist', 2);
INSERT INTO Books (Title, AuthorId) VALUES ('Nicholas Nickleby', 2);
INSERT INTO Books (Title, AuthorId) VALUES ('David Copperfield', 2);
INSERT INTO Books (Title, AuthorId) VALUES ('A Tale of Two Cities', 2);

INSERT INTO Authors (FirstName, LastName) VALUES ('Miguel', 'de Cervantes');
INSERT INTO Books (Title, AuthorId) VALUES ('Don Quixote', 3);
INSERT INTO Books (Title, AuthorId) VALUES ('The Exemplary Novels of Cervantes', 3);
INSERT INTO Books (Title, AuthorId) VALUES ('The Journey of Parnassus', 3);

INSERT INTO Authors (FirstName, LastName) VALUES ('Antoine', 'de Saint-Exuper');
INSERT INTO Books (Title, AuthorId) VALUES ('The Little Prince', 4);
INSERT INTO Books (Title, AuthorId) VALUES ('Wind, Sand and Stars', 4);
INSERT INTO Books (Title, AuthorId) VALUES ('Night Flight', 4);
INSERT INTO Books (Title, AuthorId) VALUES ('Flight to Arras', 4);
INSERT INTO Books (Title, AuthorId) VALUES ('The Wisdom of the Sands', 4);
INSERT INTO Books (Title, AuthorId) VALUES ('Southern Mail', 4);

INSERT INTO Authors (FirstName, LastName) VALUES ('Jane', 'Austen');
INSERT INTO Books (Title, AuthorId) VALUES ('Pride and Prejudice', 5);
INSERT INTO Books (Title, AuthorId) VALUES ('Emma', 5);
INSERT INTO Books (Title, AuthorId) VALUES ('Persuasion', 5);
INSERT INTO Books (Title, AuthorId) VALUES ('Northanger Abbey', 5);