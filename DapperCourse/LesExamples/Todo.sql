DROP DATABASE IF EXISTS Todo;
CREATE DATABASE Todo;

USE Todo;

DROP TABLE IF EXISTS Todo;
CREATE TABLE Todo
(
    Id        INT          NOT NULL AUTO_INCREMENT PRIMARY KEY ,
    Name      VARCHAR(255) NOT NULL,
    Completed BOOLEAN      NOT NULL DEFAULT 0
);
    
INSERT INTO Todo (Name, Completed) VALUES ('Buy milk', 0);
INSERT INTO Todo (Name, Completed) VALUES ('Buy eggs', 1);
INSERT INTO Todo (Name, Completed) VALUES ('Create Assignment', 0);
INSERT INTO Todo (Name, Completed) VALUES ('Check Assignment', 1);


