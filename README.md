# Riverty QA Assignment Submission

Task: https://github.com/manmarriverty/qa-home-assignment/tree/main

Thank you for the opportunity. This project contains automated tests for the relevant app, including:

* Unit Tests with 100% Coverage

  This is verified through [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) in VSCode.
  
  ![image](https://github.com/user-attachments/assets/4ea208f6-c3e8-44ac-bf68-6e1f85843465)


* Integration Tests
  
Test setup uses simple `xUnit`, `HttpClient`, and `dotnet test` CLI tooling.
It is also integrated with GitHub Actions for CI, and includes test result artifacts.

---

## How to Run Locally
In project root folder:
```bash
# Build the solution
$ dotnet build

# Run unit tests
$ dotnet test CardValidation.UnitTests

# Run integration tests
$ dotnet test CardValidation.IntegrationTests
```

---

## References for followed payment card conventions

- [Stripe Docs – CVC Verification](https://stripe.com/docs/testing#cvc-number)
- [Wikipedia – Payment card number](https://en.wikipedia.org/wiki/Payment_card_number)


---

## Test Types

### Unit Tests

Located in `CardValidation.UnitTests/`, these cover:

* Card number validation
* CVV checks
* Owner name rules
* Expiry date checks

### Integration Tests

In `CardValidation.IntegrationTests/`, covering end-to-end flow by calling:

```
POST /CardValidation/card/credit/validate
```

Tests validate:

* Card types (Visa, MasterCard, Amex)
* Bad requests for invalid input
* Multi-error responses

---

## GitHub Actions

Pipeline: https://github.com/imssl/card_validation_tests/actions

Configuration: `.github/workflows/docker-tests.yml`

Triggers with any git push to main branch. Pipeline:

1. Builds the project
2. Runs both unit and integration tests
3. Uploads .trx test result files in a .zip file as a downloadable artifact

