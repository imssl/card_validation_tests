using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using CardValidation.ViewModels;
using CardValidation.Core.Enums;

namespace CardValidation.IntegrationTests
{
    /// <summary>
    /// Send HTTP requests to the card validation API.
    /// Covers valid and invalid cases for different card inputs and expected responses.
    /// </summary>
    public class CardValidationIntegrationTests
    {
        /// <summary>
        /// Client used to send HTTP requests to the API during tests.
        /// </summary>
        private readonly HttpClient _client;

        /// <summary>
        /// Initialize the test server and HTTP client instance.
        /// </summary>
        public CardValidationIntegrationTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        /// <summary>
        /// Send a valid Visa card and expects a successful response with the Visa identifier.
        /// </summary>
        [Fact]
        public async Task Visa_card_returns_Visa()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John Doe", Cvv = "123", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.True(resp.IsSuccessStatusCode);
            Assert.Equal(((int)PaymentSystemType.Visa).ToString(), await resp.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Send a valid MasterCard and expects a successful response with the MasterCard identifier.
        /// </summary>
        [Fact]
        public async Task MasterCard_returns_MasterCard()
        {
            var card = new CreditCard { Number = "5105105105105100", Owner = "Jane Smith", Cvv = "456", Date = "11/29" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.True(resp.IsSuccessStatusCode);
            Assert.Equal(((int)PaymentSystemType.MasterCard).ToString(), await resp.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Send a card with a non-numeric number and expects a bad request response.
        /// </summary>
        [Fact]
        public async Task Garbage_number_returns_BadRequest()
        {
            var card = new CreditCard { Number = "ABCDE12345", Owner = "Alice", Cvv = "999", Date = "01/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        /// <summary>
        /// Send a card with an empty owner name and expects a bad request response.
        /// </summary>
        [Fact]
        public async Task Empty_owner_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "", Cvv = "123", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        /// <summary>
        /// Send a card with an expired date and expects a bad request response.
        /// </summary>
        [Fact]
        public async Task Expired_date_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John Doe", Cvv = "123", Date = "01/20" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        /// <summary>
        /// Send a card where the owner's name contains digits and expects a bad request response.
        /// </summary>
        [Fact]
        public async Task Owner_with_digits_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John 4539", Cvv = "123", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        /// <summary>
        /// Send a card with an invalid CVC length and expects a bad request response.
        /// </summary>
        [Fact]
        public async Task Wrong_cvv_length_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John Doe", Cvv = "12345", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        /// <summary>
        /// Send a card with multiple invalid fields and expects a bad request response.
        /// </summary>
        [Fact]
        public async Task Multiple_bad_fields_return_BadRequest()
        {
            var card = new CreditCard { Number = "BADNUM", Owner = "", Cvv = "1", Date = "01/10" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }
}
