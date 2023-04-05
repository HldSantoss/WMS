curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=ca45d1cc-4cd9-11ed-8000-005056b69286; ROUTEID=.node3' \
--data-raw '{
 "SqlCode": "OrderForPicking",
 "SqlName": "Obtem dados do pedido para picking",
 "SqlText": "SELECT O.NumAtCard, O.CardName, R12.Carrier, R.WhsCode, R.ItemCode, R.LineNum, R.DocEntry, R.Quantity FROM RDR1 R INNER JOIN ORDR O ON R.DocEntry = O.DocEntry INNER JOIN RDR12 R12 ON R.DocEntry = R12.DocEntry WHERE R.LineStatus = '\''O'\'' AND R.DocEntry = :docEntry"
}'