Feature: StockMarketMatchingEngineFeature3

A short summary of the feature

Scenario: TradeOrders

	Given Order 'SellOrder' Has Been Registerd
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 |


	And Order 'BuyOrder' Has Been Defined
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 1    | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 |

	When I Register The Order 'BuyOrder'


	Then The following 'Trade' will be created
		| BuyOrderId | SellOrderId | Amount | Price |
		|  2          | 1           | 5      | 100  |

	And Order 'BuyOrder' Should Be Modified  like this
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 1    | 100   | 0      | false         | 2024-02-05 09:30:26.2080000 |


	And Order 'SellOrder' Should Be Modified  like this
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 100   | 0      | false         | 2024-02-05 09:30:26.2080000 |

	
