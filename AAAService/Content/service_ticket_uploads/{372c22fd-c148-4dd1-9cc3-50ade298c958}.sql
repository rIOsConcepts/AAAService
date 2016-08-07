create view [dbo].[ExportToCF]
as
select top 100 st.job_number as 'Job Number', '000' as 'C/O EXT', 'C' as 'C/T', convert(varchar(10), st.order_datetime, 1) as 'Job Date', st.cust_po_num as 'CUST. NO.', '' as 'Cust Location No', l.cf_location_num as 'Job Location Code', l.addressline1 as 'JOB LOCATION LINE 1', l.addressline2 as 'JOB LOCATION LINE 2', '' as 'JOB LOCATION LINE 3', '' as 'JOB LOCATION LINE 4', '' as 'JOB REFERENCE', st.location_contact_name as 'JOB CONTACT', '' as 'PROJECT MANAGER', st.cust_po_num as 'CUSTOMER PO#', 0 as 'SUBMITTED AMOUNT', '' as 'C/O Status', '' as 'STATUS DATE', 0 as 'APPROVED AMOUNT', 0 as 'ESTIMATED TAX INCLUDED', 0 as 'ESTIMATED TAX ADDITIONAL', 0 as 'RET.', upper(l.state) as 'TAX STATE', '' as 'COUNTY', '99' as 'SALES REP', 'N' as 'Sales Tax', 'N' as 'Labor Tax', 'N' as 'Use Tax', 'N' as 'Condense Costs', 'N' as 'Job Complete', '05' as 'Alt G/L#', '01' as 'Std Phs/Cat', 'S' as 'Markups', '' as 'SORT DATE', convert(varchar(10), st.accepted_datetime, 1) as 'JOB START DATE', '' as 'JOB END DATE', '' as 'USER DEFINED 1', '' as 'USER DEFINED 2', '' as 'USER DEFINED 3', '' as 'USER DEFINED 4'
from dbo.service_tickets st
INNER JOIN dbo.locationinfo as l on st.service_location_guid = l.guid 
order by st.job_number desc

go