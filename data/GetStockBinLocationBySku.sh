curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=cbc4f54e-3ecb-11ed-8000-005056b69286; ROUTEID=.node4' \
--data-raw '{
 "SqlCode": "stockBinLocationBySku",
 "SqlName": "Obtem estoque por endereço através do sku",
 "SqlText": "SELECT T0.ItemCode, T0.ItemName, T3.BinCode, T3.RtrictType, T2.OnHandQty FROM OITM T0  INNER JOIN OIBQ T2 ON T0.ItemCode = T2.ItemCode INNER JOIN OBIN T3 ON T2.BinAbs = T3.AbsEntry WHERE T0.ItemCode = :itemCode"
}'

curl --location --request POST 'https://cetrohdbsl.b1cloud.com.br:50000/b1s/v1/SQLQueries' \
--header 'Content-Type: application/json' \
--header 'Cookie: B1SESSION=af39bfae-411e-11ed-8000-005056b69286; ROUTEID=.node1' \
--data-raw '{
 "SqlCode": "sql_purchase_order_by_key_access",
 "SqlName": "Retorna informações do pedido de compra para o wms",
 "SqlText": "SELECT T0.U_ChaveAcesso as Keyaccess, T0.DocEntry, T0.DocNum, T0.CANCELED, T0.DocStatus, T0.U_WMS_Status AS Status, T0.Comments, T0.CardCode,T1.LineNum, T1.ItemCode, T2.CodeBars, T1.Dscription ,T1.Quantity, T1.U_WMS_Qty_Receiving as QuantityReceiving FROM OPOR T0 INNER JOIN POR1 T1 ON T0.DocEntry=T1.DocEntry INNER JOIN OITM T2 ON T1.ItemCode=T2.ItemCode WHERE T0.U_ChaveAcesso=:keyAccess"
}'