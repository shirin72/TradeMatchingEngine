using System;
using TechTalk.SpecFlow;

namespace SpecFlowTest.StepDefinitions
{
    [Binding]
    public class StockMarketMatchingEngineEnqueuWithExampleStepDefinitions : Steps
    {
        private readonly FeatureContext context;

        public StockMarketMatchingEngineEnqueuWithExampleStepDefinitions(FeatureContext context)
        {
            this.context = context;
        }

        
        [Given(@"I have defined an '([^']*)' with these parameters (.*) and (.*) and (.*) and false and (.*)(.*)")]
        public void GivenIHaveDefinedAnWithTheseParametersAndAndAndFalseAnd(string sellOrder, int p1, int p2, int p3, Decimal p4, int p5)
        {
            var table1 = new Table(new string[] { "Side", "Price", "Amount", "IsFillAndKill" });
            table1.AddRow(new string[] { p1.ToString(), p2.ToString(), p3.ToString(),p4.ToString(),p5.ToString() });

            Given("Order 'SellOrder' Has Been Defined", table1);
            //When("I Register The Order 'SellOrder'");
            //Then("Order 'SellOrder' Should Be Enqueued");
        }


    }
}
