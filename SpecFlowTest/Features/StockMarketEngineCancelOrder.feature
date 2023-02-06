Feature: StockMarketEngineCancelOrder

A short summary of the feature

@tag1
Scenario: CancelOrder
	Given Order 'SellOrder' Has Been Registerd
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 |

	When I Send 'SellOrder' orderId And Ask For Cancel 

	Then The 'SellOrder' OrderState Should Be Equal to Cancel 
