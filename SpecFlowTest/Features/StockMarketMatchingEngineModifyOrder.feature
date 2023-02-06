Feature: StockMarketMatchingEngineModifyOrder

A short summary of the feature

@tag1
Scenario: ModifyOrder
	Given Order 'SellOrder' Has Been Registerd
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 |
	
	When I Will Try To Modify The Order 'SellOrder' with Another Orde 'ModifiedOrder'
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 100   | 10     | false         | 2024-02-05 09:30:26.2080000 |

	Then The order 'ModifiedOrder' Should Be Found
