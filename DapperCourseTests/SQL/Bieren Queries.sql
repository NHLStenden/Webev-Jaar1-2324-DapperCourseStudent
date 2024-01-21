CREATE VIEW BrouwerMetAantalBieren AS
(
    SELECT br.brouwcode, br.naam, COUNT(b.naam) AS aantal
    FROM bier b
             JOIN brouwer br on b.brouwcode = br.brouwcode
    GROUP BY br.brouwcode, br.naam
    ORDER BY aantal DESC
);

SELECT brouwcode, naam, aantal
FROM
    BrouwerMetAantalBieren
WHERE
    aantal > 10


SELECT k.naam as Kroegnaam, COUNT(b.naam) as Aantal, br.naam as Brouwer
FROM bier as b
    JOIN schenkt as s on b.biercode = s.biercode
        JOIN kroeg as k on s.kroegcode = k.kroegcode
            JOIN brouwer as br on b.brouwcode = br.brouwcode
GROUP BY k.naam, br.naam
ORDER BY Aantal DESC

SELECT type, COUNT(1) AS aantalPerType
FROM bier
GROUP BY type
ORDER BY aantalPerType DESC