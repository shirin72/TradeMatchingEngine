Feature: StockMarketMatchingEngineFeature2

A short summary of the feature

Scenario Outline: Enqueue SellOrder

	Given Order 'SellOrder' Has Been Defined
		| Side   | Price   | Amount   | IsFillAndKill   | ExpireTime   |
		| <Sell> | <Price> | <Amount> | <IsFillAndKill> | <ExpireTime> |

	When I Register The Order 'SellOrder'

	Then Order 'SellOrder' Should Be Enqueued

Examples:
	| Sell | Price | Amount | IsFillAndKill | ExpireTime |
	| 0    | 300   | 15     | false         | 2024-02-05 |
	| 0    | 400   | 10     | false         | 2024-02-05 |
	| 0    | 500   | 10     | false         | 2024-02-05 |


Scenario Outline: TradeOrders

	Given Order 'SellOrder' Has Been Registerd
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 |


	And Order 'BuyOrder' Has Been Defined
		| Side  | Price   | Amount   | IsFillAndKill   | ExpireTime   |
		| <Buy> | <Price> | <Amount> | <IsFillAndKill> | <ExpireTime> |

	When I Register The Order 'BuyOrder'


	Then The following 'Trade' will be created
		| BuyOrderId   | SellOrderId   | Amount        | Price        |
		| <BuyOrderId> | <SellOrderId> | <TradeAmount> | <TradePrice> |

		
	And Order 'BuyOrder' Should Be Modified  like this
		| Side  | Price   | Amount           | IsFillAndKill | ExpireTime                  |
		| <Buy> | <Price> | <ModifiedAmount> | false         | 2024-02-05 09:30:26.2080000 |

Examples:
	| Buy | Price | Amount | IsFillAndKill | ExpireTime                  | BuyOrderId | SellOrderId | TradeAmount | TradePrice | ModifiedAmount |
	| 1   | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 | 2          | 1           | 5           | 100        | 0              |



Scenario Outline: ModifyOrder
	Given Order 'SellOrder' Has Been Registerd
		| Side   | Price   | Amount   | IsFillAndKill   | ExpireTime   |
		| <Sell> | <Price> | <Amount> | <IsFillAndKill> | <ExpireTime> |
	
	And Order 'ModifiedOrder' Has Been Defined
		| Side | Price | Amount | IsFillAndKill | ExpireTime                  |
		| 0    | 1000  | 10     | false         | 2024-02-05 09:30:26.2080000 |
	
	When I Modify The Order 'SellOrder' to 'ModifiedOrder'


	Then The order 'SellOrder'  Should Be Found like 'ModifiedOrder'


Examples:
	| Sell | Price | Amount | IsFillAndKill | ExpireTime                  |
	| 0    | 1000  | 1000   | false         | 2024-02-05 09:30:26.2080000 |



Scenario Outline: CancelOrder
	Given Order 'SellOrder' Has Been Registerd
		| Side   | Price   | Amount   | IsFillAndKill   | ExpireTime   |
		| <Sell> | <Price> | <Amount> | <IsFillAndKill> | <ExpireTime> |
	
	#And Order 'ModifiedOrder' Has Been Defined
	#	| Side   | Price   | Amount   | IsFillAndKill   | ExpireTime   | IsCanceled |
	#	| <Sell> | <Price> | <Amount> | <IsFillAndKill> | <ExpireTime> | true       |

	When I cancel 'SellOrder'

	Then The order 'SellOrder'  Should Be Cancelled


Examples:
	| Sell | Price | Amount | IsFillAndKill | ExpireTime                  |
	| 0    | 100   | 5      | false         | 2024-02-05 09:30:26.2080000 |
	| 0    | 200   | 5      | false         | 2024-02-05 09:30:26.2080000 |
	| 0    | 300   | 5      | false         | 2024-02-05 09:30:26.2080000 |




	
