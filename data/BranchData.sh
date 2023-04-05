curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=2aadffac-9457-11ed-8000-005056b69286; ROUTEID=.node4' \
--data-raw '{
 "SqlCode": "branchData",
 "SqlName": "Obtem dados da filial",
 "SqlText": "SELECT BPLName, TaxIdNum AS CNPJ, TaxIdNum2 AS IE, AddrType, Street, StreetNo, ZipCode, Block, City, State FROM OBPL WHERE BPLId = :bplId"
}'