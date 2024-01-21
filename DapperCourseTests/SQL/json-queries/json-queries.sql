DROP VIEW IF EXISTS filmAndActorAsJson;
CREATE VIEW filmAndActorAsJson AS
SELECT
    f.film_id as film_id,
    JSON_OBJECT('FilmId', f.film_id, 'Title', f.title, 'Description', f.description, 'Actors',
                JSON_ARRAYAGG(
                        JSON_OBJECT(
                                'ActorId', a.actor_id,
                                'FistName', a.first_name,
                                'LastName', a.last_name
                        )
                )
    ) as FilmActorAsJson
FROM film f
    JOIN film_actor fm ON f.film_id = fm.film_id
         JOIN actor a ON fm.actor_id = a.actor_id
GROUP BY f.film_id;

DROP VIEW IF EXISTS customersAsJson;
CREATE VIEW customersAsJson AS
SELECT c.customer_id, c.store_id, a.address_id, JSON_OBJECT('CustomerId', c.customer_id,
                                                            'FirstName', c.first_name,
                                                            'LastName', c.last_name,
                                                            'Email', c.email,
                                                            'AddressId', c.address_id,
                                                            'StoreId', c.store_id,
                                                            'Active', c.active,
                                                            'CreateDate', c.create_date,
                                                            'LastUpdate', c.last_update,
                                                            'Address', JSON_OBJECT('address', a.address,
                                                                                   'address2', a.address2,
                                                                                   'district', a.district,
                                                                                   'cityId', a.city_id,
                                                                                   'city', JSON_OBJECT('cityId', ci.city_id,
                                                                                                       'city', ci.city,
                                                                                                       'countryId', ci.country_id,
                                                                                                       'country', JSON_OBJECT('countryId', co.country_id,
                                                                                                                              'country', co.country,
                                                                                                                              'lastUpdate', co.last_update)
                                                                                           ),
                                                                                   'postalCode', a.postal_code,
                                                                                   'phone', a.phone)
                                                ) as CutsomerObject
FROM customer c
        JOIN address a ON c.address_id = a.address_id
            JOIN city ci ON a.city_id = ci.city_id
                JOIN country co ON ci.country_id = co.country_id;

SELECT JSON_MERGE_PATCH(c.CutsomerObject,
                        JSON_OBJECT('movies',
                                    JSON_ARRAYAGG(DISTINCT f.FilmActorAsJson ORDER BY JSON_EXTRACT(f.FilmActorAsJson, '$.Title')) )) as CustomerObject
FROM rental r
         JOIN customersAsJson c on r.customer_id = c.customer_id
         JOIN inventory  i ON r.inventory_id = i.inventory_id
         JOIN filmAndActorAsJson f ON i.film_id = f.film_Id
WHERE c.customer_id = 19
GROUP BY c.customer_id;




SELECT JSON_OBJECT('StoreId', s.store_id,
                   'ManagerStaffId', s.manager_staff_id,
                   'Customers',
                   JSON_ARRAYAGG(
                           JSON_OBJECT('CustomerId', c.customer_id,
                                       'FirstName', c.first_name,
                                       'LastName', c.last_name,
                                       'Email', c.email,
                                       'AddressId', c.address_id,
                                       'StoreId', c.store_id,
                                       'Address', JSON_OBJECT('address', a.address,
                                                              'address2', a.address2,
                                                              'district', a.district,
                                                              'cityId', a.city_id,
                                                              'city', JSON_OBJECT('cityId', ci.city_id,
                                                                                  'city', ci.city,
                                                                                  'countryId', ci.country_id,
                                                                                  'country', JSON_OBJECT('countryId', co.country_id,
                                                                                                         'country', co.country,
                                                                                                         'lastUpdate', co.last_update)
                                                                      ),
                                                              'postalCode', a.postal_code,
                                                              'phone', a.phone)
                           ) ORDER BY c.last_name  LIMIT 2
                   )
       )
FROM store s
         JOIN customer c ON s.store_id = c.store_id
         JOIN address a ON c.address_id = a.address_id
         JOIN city ci ON a.city_id = ci.city_id
         JOIN country co ON ci.country_id = co.country_id
GROUP BY s.store_id
ORDER BY s.store_id
LIMIT 1




WITH customerCte AS
         (SELECT customer_id, first_name, last_name, email, address_id, store_id FROM (
                                                                                          SELECT c.customer_id, c.first_name, c.last_name, c.email, c.address_id, c.store_id,
                                                                                                 ROW_NUMBER() over (PARTITION BY c.store_id ORDER BY c.last_name) as row_num
                                                                                          FROM customer as c ) as cc WHERE row_num <= 2)
SELECT JSON_OBJECT('StoreId', s.store_id,
                   'ManagerStaffId', s.manager_staff_id,
                   'Customers',
                   JSON_ARRAYAGG(
                           JSON_OBJECT('CustomerId', c.customer_id,
                                       'FirstName', c.first_name,
                                       'LastName', c.last_name,
                                       'Email', c.email,
                                       'AddressId', c.address_id,
                                       'StoreId', c.store_id
                           )
                   )
       )
FROM store s
         JOIN customerCte as c ON s.store_id = c.store_id
GROUP BY s.store_id
ORDER BY s.store_id


SELECT JSON_OBJECT('StoreId', s.store_id,
                   'ManagerStaffId', s.manager_staff_id,
                   'Customers',
                   JSON_ARRAYAGG(
                           JSON_OBJECT('CustomerId', c.customer_id,
                                       'FirstName', c.first_name,
                                       'LastName', c.last_name,
                                       'Email', c.email,
                                       'AddressId', c.address_id,
                                       'StoreId', c.store_id
                           )
                   )
       )
FROM store s
         JOIN customer c ON s.store_id = c.store_id
GROUP BY s.store_id
ORDER BY s.store_id


SELECT JSON_OBJECT('StoreId', s.store_id,
                   'ManagerStaffId', s.manager_staff_id,
                   'Customers',
                   JSON_ARRAYAGG(
                           JSON_OBJECT('CustomerId', c.customer_id,
                                       'FirstName', c.first_name,
                                       'LastName', c.last_name,
                                       'Email', c.email,
                                       'AddressId', c.address_id,
                                       'StoreId', c.store_id
                           ) ORDER BY c.last_name  LIMIT 3
                   )
       )
FROM store s
         JOIN customer c ON s.store_id = c.store_id
GROUP BY s.store_id
ORDER BY s.store_id
LIMIT 1


SELECT JSON_MERGE_PATCH(c.CutsomerObject,
                        JSON_OBJECT('movies', 
                                    JSON_ARRAYAGG(DISTINCT f.FilmActorAsJson ORDER BY JSON_EXTRACT(f.FilmActorAsJson, '$.Title')) )) as CustomerObject
FROM rental r
         JOIN customersAsJson c on r.customer_id = c.customer_id
         JOIN inventory  i ON r.inventory_id = i.inventory_id
         JOIN filmAndActorAsJson f ON i.film_id = f.film_Id
WHERE c.customer_id = 19
GROUP BY c.customer_id;
