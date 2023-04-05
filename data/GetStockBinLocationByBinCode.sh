curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=d2e036fa-44ba-11ed-8000-005056b69286; ROUTEID=.node4' \
--data-raw '{
 "SqlCode": "stockBinLocationByBinCode",
 "SqlName": "Obtem estoque por endereço através do endereço",
 "SqlText": "SELECT T0.ItemCode, T0.ItemName, T2.BinCode, T2.RtrictType, T1.OnHandQty FROM OITM T0  INNER JOIN OIBQ T1 ON T0.ItemCode = T1.ItemCode INNER JOIN OBIN T2 ON T1.BinAbs = T2.AbsEntry WHERE T2.BinCode = :bincode"
}'