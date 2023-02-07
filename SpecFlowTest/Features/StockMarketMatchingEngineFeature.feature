Feature: StockMarketMatchingEngineFeature2

A short summary of the feature

Scenario: Enqueue SellOrder

	Given Order 'SellOrder' Has Been Defined
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 |

	When I Register The Order 'SellOrder'

	Then Order 'SellOrder' Should Be Enqueued



	
