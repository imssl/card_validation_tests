using System;
using Xunit;
using CardValidation.Core.Services;
using CardValidation.Core.Enums;

namespace CardValidation.UnitTests
{
    /// <summary>
    /// Contains unit tests for the <see cref="CardValidationService"/> class, validating card number format, 
    /// payment system type identification, owner name validation, CVC validation, and issue date validation.
    /// </summary>
    public class CardValidationServiceTests
    {
        private readonly CardValidationService service = new();

        /// <summary>
        /// Tests whether <see cref="CardValidationService.ValidateNumber(string)"/> returns the expected result
        /// for various card numbers, including known valid formats (Visa, MasterCard, American Express) and invalid ones.
        /// </summary>
        /// <param name="number">The credit card number to validate.</param>
        /// <param name="expected">The expected validation result.</param>
        [Theory]
        [InlineData("4539682995824395", true)]   // Visa
        [InlineData("5105105105105100", true)]   // MasterCard (51-55 range)
        [InlineData("2221000000000009", true)]   // MasterCard (2221+)
        [InlineData("378282246310005", true)]    // American Express
        [InlineData("6011111111111117", false)]  // Discover (unsupported)
        [InlineData("ABCDE12345", false)]        // Invalid format
        public void ValidateNumber_returns_expected(string number, bool expected)
        {
            Assert.Equal(expected, service.ValidateNumber(number));
        }

        /// <summary>
        /// Tests whether <see cref="CardValidationService.GetPaymentSystemType(string)"/> correctly identifies
        /// the payment system type for valid card numbers.
        /// </summary>
        /// <param name="number">The credit card number to evaluate.</param>
        /// <param name="expected">The expected <see cref="PaymentSystemType"/> value.</param>
        [Theory]
        [InlineData("4539682995824395", PaymentSystemType.Visa)]
        [InlineData("5105105105105100", PaymentSystemType.MasterCard)]
        [InlineData("378282246310005", PaymentSystemType.AmericanExpress)]
        public void GetPaymentSystemType_returns_correct_enum(string number, PaymentSystemType expected)
        {
            var result = service.GetPaymentSystemType(number);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that <see cref="CardValidationService.GetPaymentSystemType(string)"/> throws
        /// a <see cref="NotImplementedException"/> when given an unsupported card number.
        /// </summary>
        [Fact]
        public void GetPaymentSystemType_invalid_number_throws()
        {
            Assert.Throws<NotImplementedException>(() =>
                service.GetPaymentSystemType("6011111111111117"));
        }

        /// <summary>
        /// Tests whether <see cref="CardValidationService.ValidateOwner(string)"/> correctly validates
        /// card owner names, allowing only letters and spaces.
        /// </summary>
        /// <param name="owner">The owner name to validate.</param>
        /// <param name="expected">The expected validation result.</param>
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

        /// <summary>
        /// Validates that <see cref="CardValidationService.ValidateCvc(string)"/> accepts only CVC codes 
        /// that are 3 or 4 digits long.
        /// </summary>
        /// <param name="cvc">The CVC code to validate.</param>
        /// <param name="expected">The expected validation result.</param>
        [Theory]
        [InlineData("123", true)]
        [InlineData("9876", true)]   // American Express (4-digit CVC)
        [InlineData("12", false)]
        [InlineData("12A", false)]
        public void ValidateCvc_returns_expected(string cvc, bool expected)
        {
            Assert.Equal(expected, service.ValidateCvc(cvc));
        }

        /// <summary>
        /// Tests whether <see cref="CardValidationService.ValidateIssueDate(string)"/> accepts
        /// future dates in valid formats such as MM/yyyy and MM/yy.
        /// </summary>
        /// <param name="futureDate">The future expiry date to validate.</param>
        [Theory]
        [InlineData("01/2050")]
        [InlineData("12/50")]
        public void ValidateIssueDate_future_dates_are_valid(string futureDate)
        {
            Assert.True(service.ValidateIssueDate(futureDate));
        }

        /// <summary>
        /// Tests whether <see cref="CardValidationService.ValidateIssueDate(string)"/> returns false
        /// for past dates, invalid formats, or empty strings.
        /// </summary>
        /// <param name="input">The input date string to validate.</param>
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
