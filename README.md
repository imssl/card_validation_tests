# 🧪 CardValidation Test Suite

This project contains automated tests for the **CardValidation** API, including:

* Unit Tests with 100% Coverage

  This is verified through [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) in VSCode.
  ![image](https://github.com/user-attachments/assets/0fda925f-8c74-40fa-966a-7328330e901d)

* Integration Tests
  
Test setup uses simple `xUnit`, `HttpClient`, and `dotnet test` CLI tooling.
It is also integrated with GitHub Actions for CI, and includes test result artifacts.

---

## 📁 Project Structure

```
card_validation_tests/
├── CardValidation.UnitTests               # Unit test project
│   └── CardValidationUnitTests.cs         
├── CardValidation.IntegrationTests        # Integration test project
│   └── CardValidationIntegrationTests.cs  # End-to-end HTTP-level tests
├── .github/workflows/
│   └── docker-tests.yml                   # GitHub Actions workflow file
├── TestResults/                           # Output test logs and coverage (gitignored)
└── README.md                              # This file
```

---

## 🧪 Test Types

### ✅ Unit Tests

Located in `CardValidation.UnitTests/`, these cover:

* Card number validation
* CVV checks
* Owner name rules
* Expiry date checks

### 🌐 Integration Tests

In `CardValidation.IntegrationTests/`, covering end-to-end flow by calling:

```
POST /CardValidation/card/credit/validate
```

Tests validate:

* Card types (Visa, MasterCard, Amex)
* Bad requests for invalid input
* Multi-error responses

---

## ⚙️ GitHub Actions

Workflow: `.github/workflows/docker-tests.yml`

Triggers on push to `main`. It:

1. Builds the project
2. Runs both unit and integration tests
3. Uploads `test-results.trx` as downloadable artifact

---

## 📥 How to Run Locally

```bash
# Build the solution
$ dotnet build

# Run unit tests
$ dotnet test CardValidation.UnitTests

# Run integration tests
$ dotnet test CardValidation.IntegrationTests
```

---

## References

- [Wikipedia – Payment card number](https://en.wikipedia.org/wiki/Payment_card_number)
- [IBM Docs – Luhn Algorithm](https://www.ibm.com/docs/en/zos/2.1.0?topic=applications-luhn-algorithm)
- [Stripe Docs – CVC Verification](https://stripe.com/docs/testing#cvc-number)
- [Visa Acceptance Guide](https://usa.visa.com/dam/VCOM/download/merchants/visa-acceptance-guide-for-visa-merchants.pdf)
- [PCI UX Guidelines](https://www.pcisecuritystandards.org/pdfs/PCI-UX-Design-Guidelines.pdf)
