--------------------------------------------------------------------------------------------------------------------------------
-- Data: 19-10-2022
-- Autor: FF
-- Regra: Não permite alterar para picking se não estiver como CanPicking
--------------------------------------------------------------------------------------------------------------------------------
IF :object_type='17' AND :transaction_type='U'
THEN
	DECLARE num_reg int;
	SELECT COUNT(*) INTO num_reg
	FROM ORDR T0
	WHERE T0."DocEntry" = :list_of_cols_val_tab_del
		AND T0."U_WMS_Status" = 'Picking';

	IF num_reg > 0 
	THEN
		error := '999';
  		error_message := 'Outro cliente já está separando esse pedido';
	END IF;

end if;