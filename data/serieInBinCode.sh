curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=e765d48e-9688-11ed-8000-005056b69286; ROUTEID=.node4' \
--data-raw '{
 "SqlCode": "serieInBinCode",
 "SqlName": "Valida a serie no endereço",
 "SqlText": "SELECT T3.ItemCode FROM OSBQ T0 INNER JOIN OSRN T1 ON T0.SnBMDAbs = T1.AbsEntry INNER JOIN OBIN T2 ON T0.BinAbs = T2.AbsEntry INNER JOIN OITM T3 ON T0.ItemCode = T3.ItemCode WHERE T0.OnHandQty > 0 AND T1.DistNumber = :serie AND T2.BinCode = :bincode"
}'


{{sl}}/SQLQueries('suggestBinsForPicking')/List?itemCode='EM000744'&binCode='02-A-01-01-01'
{{sl}}/SQLQueries('serieInBinCode')/List?serie='202301021532MQ0003752169210'&bincode='02-C-02-04-06'