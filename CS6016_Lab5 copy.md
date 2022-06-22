# Lab 5
1. SELECT Name, count(*) FROM Patrons NATURAL JOIN CheckedOut GROUP BY Name ORDER BY count(*) DESC LIMIT 1;  
2. SELECT Author, count(*) as c FROM Titles GROUP BY Author HAVING c > 1;  
3. SELECT AUthot, count(*) as c FROM Titles LEFT JOIN Inventory WHERE Serial IS NOT NULL GROUP BY Author;  
4. SELECT Name, count(*), CASE WHEN count(*) > 2 THEN "Platinum" WHEN count(*) = 2 THEN "Gold" WHEN count(*) = 1 THEN "Silver" WHEN count(*) = 0 THEN "Bronze" Else "Unknown" END AS "Rank" FROM Patrons NATURAL LEFT JOIN CheckedOut GROUP BY Name;