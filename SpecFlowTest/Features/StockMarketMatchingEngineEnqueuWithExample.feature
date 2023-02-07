Feature: StockMarketMatchingEngineEnqueuWithExample

A short summary of the feature

@tag1
Scenario: Using example for enqueue
	Given I have defined an 'SellOrder' with these parameters <Side> and <Price> and <Amount> and <IsFillAndKill> and <ExpireTime>
	When I Register The Order 'SellOrder'
	Then Order 'SellOrder' Should Be Enqueued

Examples:
	| Side | Price | Amount | IsFillAndKill | ExpireTime |
	| 0    | 100   | 5      | false         | 2024-02-05 |