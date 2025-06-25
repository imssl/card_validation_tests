using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using CardValidation.ViewModels;
using CardValidation.Core.Enums;

namespace CardValidation.IntegrationTests
{
    public class CardValidationApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        // Test HTTP client from the application factory
        public CardValidationApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        // Send a valid Visa card and expects Visa in the response
        [Fact]
        public async Task Visa_card_should_return_Visa()
        {
            var card = new CreditCard
            {
                Number = "4539682995824395",
                Owner = "John Doe",
                Cvv = "123",
                Date = "12/30"
            };

            var response = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.True(response.IsSuccessStatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal(((int)PaymentSystemType.Visa).ToString(), responseBody);
        }

        // Send a valid MasterCard and expects MasterCard in the response
        [Fact]
        public async Task MasterCard_should_return_MasterCard()
        {
            var card = new CreditCard
            {
                Number = "5105105105105100",
                Owner = "Jane Smith",
                Cvv = "456",
                Date = "11/29"
            };

            var response = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.True(response.IsSuccessStatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal(((int)PaymentSystemType.MasterCard).ToString(), responseBody);
        }

        // Send a card with garbage number and expects a 400 Bad Request
        [Fact]
        public async Task Invalid_card_number_should_return_BadRequest()
        {
            var card = new CreditCard
            {
                Number = "invalid_number",
                Owner = "Alice",
                Cvv = "999",
                Date = "01/30"
            };

            var response = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        // Send a card with an empty number and expects a 400 Bad Request
        [Fact]
        public async Task Empty_card_number_should_return_BadRequest()
        {
            var card = new CreditCard
            {
                Number = "",
                Owner = "Bob",
                Cvv = "123",
                Date = "05/31"
            };

            var response = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
