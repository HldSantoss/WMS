curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=b7602a2c-4210-11ed-8000-005056b69286; ROUTEID=.node4' \
--data-raw '{
 "SqlCode": "BinAbsFromBinCode",
 "SqlName": "Obtem id da posicao atraves do codigo",
 "SqlText": "SELECT T0.AbsEntry, T0.BinCode, T0.WhsCode FROM OBIN T0 WHERE T0.BinCode = :binCodeFrom OR T0.BinCode = :binCodeTo"
}'