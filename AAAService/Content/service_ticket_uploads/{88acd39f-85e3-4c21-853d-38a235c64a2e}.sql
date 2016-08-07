/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP 1000 [Id]
      ,[guid]
      ,[Email]
      ,[EmailConfirmed]
      ,[PasswordHash]
      ,[SecurityStamp]
      ,[PhoneNumber]
      ,[PhoneNumberConfirmed]
      ,[TwoFactorEnabled]
      ,[LockoutEndDateUtc]
      ,[LockoutEnabled]
      ,[AccessFailedCount]
      ,[UserName]
      ,[lname]
      ,[fname]
      ,[account_status]
      ,[salt]
      ,[is_manager]
      ,[title]
  FROM [aaahelp].[dbo].[AspNetUsers]
  where email like '%riosc%'
  order by email desc

918147fc-a113-49c5-b11b-a641dbcb2897