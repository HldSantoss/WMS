curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=0026a2a6-53df-11ed-8000-005056b69286; ROUTEID=.node3' \
--data-raw '{
 "SqlCode": "suggestBinsForPicking",
 "SqlName": "Sugestão de endereço para picking",
 "SqlText": "SELECT T3.WhsCode, T3.AbsEntry, T3.BinCode, T0.OnHandQty, T1.ItemCode, T1.ItemName, T1.ManSerNum, T1.ManBtchNum FROM OIBQ T0 INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode INNER JOIN OBIN T3 ON T0.BinAbs = T3.AbsEntry WHERE T3.RtrictType IN (0,2) AND T0.OnHandQty > 0 AND T1.ItemCode = :itemCode AND T3.BinCode <> :binCode"
}'