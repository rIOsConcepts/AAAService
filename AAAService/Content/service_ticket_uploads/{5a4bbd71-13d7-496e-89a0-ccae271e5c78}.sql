SELECT user_guid, count(*)
FROM [aaahelp].[dbo].[user_to_location]
group by user_guid
having count(user_guid) > 1


SELECT * FROM [aaahelp].[dbo].[locationinfo]


SELECT c.name, c.guid
FROM [aaahelp].[dbo].[user_to_location] utl
join [aaahelp].[dbo].locationinfo l on l.guid = utl.location_guid
join [aaahelp].[dbo].Company c on c.guid = l.parentguid
where user_guid = '12F236B9-A186-4469-A38D-D42D6D0408E7'
where guid = '510789e0-7c7a-4596-8ca7-12b644eb446f' or guid = '0191321b-8fdc-4cb2-9b94-9a3a6d64da83' or guid = '7eefd742-a705-49ad-bd76-f622a6dd5e80'