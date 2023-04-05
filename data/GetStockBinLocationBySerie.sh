curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=d2e036fa-44ba-11ed-8000-005056b69286; ROUTEID=.node4' \
--data-raw '{
 "SqlCode": "stockBinLocationBySerie",
 "SqlName": "Obtem estoque por endereço através da serie",
 "SqlText": "SELECT T3.ItemCode, T3.ItemName, T2.BinCode, T2.RtrictType, T0.OnHandQty FROM OSBQ T0  INNER JOIN OSRN T1 ON T0.SnBMDAbs = T1.AbsEntry INNER JOIN OBIN T2 ON T0.BinAbs = T2.AbsEntry INNER JOIN OITM T3 ON T0.ItemCode = T3.ItemCode WHERE T0.OnHandQty > 0 AND T1.DistNumber = :serie"
}'