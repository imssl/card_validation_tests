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
    // simple integration tests for the API, no fixtures
    public class CardValidationApiTests
    {
        private readonly HttpClient _client;

        public CardValidationApiTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        // valid Visa should succeed
        [Fact]
        public async Task Visa_card_returns_Visa()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John Doe", Cvv = "123", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.True(resp.IsSuccessStatusCode);
            Assert.Equal(((int)PaymentSystemType.Visa).ToString(), await resp.Content.ReadAsStringAsync());
        }

        // valid MasterCard should succeed
        [Fact]
        public async Task MasterCard_returns_MasterCard()
        {
            var card = new CreditCard { Number = "5105105105105100", Owner = "Jane Smith", Cvv = "456", Date = "11/29" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.True(resp.IsSuccessStatusCode);
            Assert.Equal(((int)PaymentSystemType.MasterCard).ToString(), await resp.Content.ReadAsStringAsync());
        }

        // garbage number should get 400
        [Fact]
        public async Task Garbage_number_returns_BadRequest()
        {
            var card = new CreditCard { Number = "ABCDE12345", Owner = "Alice", Cvv = "999", Date = "01/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // empty owner should get 400
        [Fact]
        public async Task Empty_owner_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "", Cvv = "123", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // expired date should get 400
        [Fact]
        public async Task Expired_date_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John Doe", Cvv = "123", Date = "01/20" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // owner with digits should get 400
        [Fact]
        public async Task Owner_with_digits_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John 4539", Cvv = "123", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // wrong cvv length should get 400
        [Fact]
        public async Task Wrong_cvv_length_returns_BadRequest()
        {
            var card = new CreditCard { Number = "4539682995824395", Owner = "John Doe", Cvv = "12345", Date = "12/30" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }

        // multiple issues should get 400
        [Fact]
        public async Task Multiple_bad_fields_return_BadRequest()
        {
            var card = new CreditCard { Number = "BADNUM", Owner = "", Cvv = "1", Date = "01/10" };
            var resp = await _client.PostAsJsonAsync("/CardValidation/card/credit/validate", card);
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        }
    }
}
