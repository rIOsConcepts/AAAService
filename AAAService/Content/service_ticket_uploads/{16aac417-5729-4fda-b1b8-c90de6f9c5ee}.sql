/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [guid]
      ,[name]
      ,[addressline1]
      ,[addressline2]
      ,[city]
      ,[state]
      ,[zip]
      ,[cf_company_num]
      ,[cf_location_num]
      ,[region]
      ,[email_all_members]
      ,[active]
      ,[AcctMgrEmail]
  FROM [aaahelp].[dbo].[Company]
where guid = '510789e0-7c7a-4596-8ca7-12b644eb446f' or guid = '0191321b-8fdc-4cb2-9b94-9a3a6d64da83' or guid = '7eefd742-a705-49ad-bd76-f622a6dd5e80'
  where name like '%affordable%'