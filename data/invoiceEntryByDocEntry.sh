curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=55545276-54cc-11ed-8000-005056b69286; ROUTEID=.node3' \
--data-raw '{
 "SqlCode": "invoiceEntryByDocEntry",
 "SqlName": "Obtem o docentry da nota fiscal com base no docentry do pedido",
 "SqlText": "SELECT MAX(T0.TrgetEntry) AS DocEntry FROM RDR1 T0 INNER JOIN ORDR T1 ON T0.DocEntry = T1.DocEntry WHERE T1.U_WMS_Status = 'CanCheckout' AND T0.DocEntry = :docEntry"
}'