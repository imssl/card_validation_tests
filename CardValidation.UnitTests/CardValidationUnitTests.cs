using System;
using Xunit;
using CardValidation.Core.Services;
using CardValidation.Core.Enums;

namespace CardValidation.UnitTests
{
    /// <summary>
    /// Cover number validation, owner validation, CVC rules,
    /// issue-date checks, and card-type detection.
    /// </summary>
    public class CardValidationServiceTests
    {
        /// <summary>
        /// System-under-test instance reused by all cases.
        /// </summary>
        private readonly CardValidationService _service = new();

        /// <summary>
        /// Verify card-number patterns are correctly recognised or rejected.
        /// </summary>
        [Theory]
        [InlineData("4539682995824395", true)]
        [InlineData("5105105105105100", true)]
        [InlineData("2221000000000009", true)]
        [InlineData("378282246310005",  true)]
        [InlineData("6011111111111117", false)]
        [InlineData("ABCDE12345",       false)]
        public void ValidateNumber_ReturnsExpected(string number, bool expected)
        {
            Assert.Equal(expected, _service.ValidateNumber(number));
        }

        /// <summary>
        /// Ensure service maps valid numbers to correct payment system type.
        /// </summary>
        [Theory]
        [InlineData("4539682995824395", PaymentSystemType.Visa)]
        [InlineData("5105105105105100", PaymentSystemType.MasterCard)]
        [InlineData("378282246310005",  PaymentSystemType.AmericanExpress)]
        public void GetPaymentSystemType_ReturnsCorrectEnum(string number, PaymentSystemType expected)
        {
            var result = _service.GetPaymentSystemType(number);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Assert that an unsupported prefix triggers a NotImplementedException.
        /// </summary>
        [Fact]
        public void GetPaymentSystemType_InvalidNumber_Throws()
        {
            Assert.Throws<NotImplementedException>(() =>
                _service.GetPaymentSystemType("6011111111111117"));
        }

        /// <summary>
        /// Check owner name is allowed to have alphabetic names with spaces and rejects invalid cases.
        /// </summary>
        [Theory]
        [InlineData("John Doe",          true)]
        [InlineData("Anna Maria Smith",  true)]
        [InlineData("1234",              false)]
        [InlineData("John123",           false)]
        [InlineData("",                  false)]
        public void ValidateOwner_ReturnsExpected(string owner, bool expected)
        {
            Assert.Equal(expected, _service.ValidateOwner(owner));
        }

        /// <summary>
        /// Validate CVC length rules (3 digits for Visa/MC, 4 for AmEx).
        /// </summary>
        [Theory]
        [InlineData("123",  true)]
        [InlineData("9876", true)]
        [InlineData("12",   false)]
        [InlineData("12A",  false)]
        public void ValidateCvc_ReturnsExpected(string cvc, bool expected)
        {
            Assert.Equal(expected, _service.ValidateCvc(cvc));
        }

        /// <summary>
        /// Confirm that expiry dates are accepted as MM/yy or MM/yyyy.
        /// </summary>
        [Theory]
        [InlineData("01/2050")]
        [InlineData("12/50")]
        public void ValidateIssueDate_FutureDates_AreValid(string futureDate)
        {
            Assert.True(_service.ValidateIssueDate(futureDate));
        }

        /// <summary>
        /// Ensure past, malformed, or impossible dates are rejected.
        /// </summary>
        [Theory]
        [InlineData("01/10")]
        [InlineData("13/30")]
        [InlineData("invalid")]
        [InlineData("")]
        public void ValidateIssueDate_InvalidOrPast_ReturnsFalse(string input)
        {
            Assert.False(_service.ValidateIssueDate(input));
        }
    }
}
