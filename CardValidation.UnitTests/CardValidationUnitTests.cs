using System;
using Xunit;
using CardValidation.Core.Services;
using CardValidation.Core.Enums;

namespace CardValidation.UnitTests
{
    public class CardValidationServiceTests
    {
        private CardValidationService service = new();

        // Check if the card number format matches any known type (Visa, MasterCard, American Express).
        [Theory]
        [InlineData("4539682995824395", true)]   // Visa
        [InlineData("5105105105105100", true)]   // MasterCard (51-55 range)
        [InlineData("2221000000000009", true)]   // MasterCard (2221+)
        [InlineData("378282246310005", true)]    // American Express
        [InlineData("6011111111111117", false)]  // Discover
        [InlineData("ABCDE12345", false)]        // Invalid
        public void ValidateNumber_returns_expected(string number, bool expected)
        {
            Assert.Equal(expected, service.ValidateNumber(number));
        }

        // Identify and return the specific card type based on its prefix and length.
        [Theory]
        [InlineData("4539682995824395", PaymentSystemType.Visa)]
        [InlineData("5105105105105100", PaymentSystemType.MasterCard)]
        [InlineData("378282246310005", PaymentSystemType.AmericanExpress)]
        public void GetPaymentSystemType_returns_correct_enum(string number, PaymentSystemType expected)
        {
            var result = service.GetPaymentSystemType(number);
            Assert.Equal(expected, result);
        }

        // Verify that unsupported card numbers throw a NotImplementedException.
        [Fact]
        public void GetPaymentSystemType_invalid_number_throws()
        {
            Assert.Throws<NotImplementedException>(() =>
                service.GetPaymentSystemType("6011111111111117"));
        }

        // Check if the card owner's name contains only letters and spaces.
        [Theory]
        [InlineData("John Doe", true)]
        [InlineData("Anna Maria Smith", true)]
        [InlineData("1234", false)]
        [InlineData("John123", false)]
        [InlineData("", false)]
        public void ValidateOwner_returns_expected(string owner, bool expected)
        {
            Assert.Equal(expected, service.ValidateOwner(owner));
        }

        // Verify if the card's CVC (security code) is either 3 or 4 digits long.
        [Theory]
        [InlineData("123", true)]
        [InlineData("9876", true)]   // American Express
        [InlineData("12", false)]
        [InlineData("12A", false)]
        public void ValidateCvc_returns_expected(string cvc, bool expected)
        {
            Assert.Equal(expected, service.ValidateCvc(cvc));
        }

        // Confirm that future expiry dates are accepted in MM/yy or MM/yyyy format.
        [Theory]
        [InlineData("01/2050")]
        [InlineData("12/50")]
        public void ValidateIssueDate_future_dates_are_valid(string futureDate)
        {
            Assert.True(service.ValidateIssueDate(futureDate));
        }

        // Test various invalid dates.
        [Theory]
        [InlineData("01/10")]
        [InlineData("13/30")]
        [InlineData("invalid")]
        [InlineData("")]
        public void ValidateIssueDate_false_for_invalid_or_past(string input)
        {
            Assert.False(service.ValidateIssueDate(input));
        }
    }
}
