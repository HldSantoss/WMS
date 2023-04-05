curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=0aed4bb6-501c-11ed-8000-005056b69286; ROUTEID=.node3' \
--data-raw '{
 "SqlCode": "BinsForPicking",
 "SqlName": "Obtem dados de posições para picking",
 "SqlText": "SELECT T3.WhsCode, T3.AbsEntry, T3.BinCode, T0.OnHandQty, T1.ItemCode, T1.ItemName, T1.ManSerNum, T1.ManBtchNum FROM OIBQ T0  INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode INNER JOIN RDR1 T2 ON T1.ItemCode = T2.ItemCode INNER JOIN OBIN T3 ON T0.BinAbs = T3.AbsEntry WHERE T2.DocEntry = :docEntry AND T3.RtrictType IN (0,2) AND  T0.OnHandQty > 0"
}'