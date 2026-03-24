using Bunit;
using BookingSystem.Client.Pages;
using FluentAssertions;
using Xunit;

namespace BookingSystem.Tests.UnitTests.Pages
{
    public class LoginTests : TestContext
    {
        [Fact]
        public void Login_ShouldShowValidationErrors_WhenSubmittedEmpty()
        {
            // Arrange (Render the Blazor component)
            var cut = RenderComponent<Login>();

            // Act (Find the form and submit it without filling in any fields)
            cut.Find("form").Submit();

            // Assert (Check that the UI rendered the correct validation error messages)
            var markup = cut.Markup;
            markup.Should().Contain("Du m&#xE5;ste fylla i en e-postadress."); // HTML encoded "å"
            markup.Should().Contain("Du m&#xE5;ste fylla i ett l&#xF6;senord."); // HTML encoded "å" and "ö"
        }

        [Fact]
        public void Login_ShouldShowEmailValidationError_WhenEmailIsInvalid()
        {
            // Arrange
            var cut = RenderComponent<Login>();

            // Act (Fill in an invalid email and a valid password)
            var emailInput = cut.Find("input[placeholder='namn@exempel.se']");
            emailInput.Change("not-an-email");

            var passwordInput = cut.Find("input[type='password']");
            passwordInput.Change("SomePassword123!");

            cut.Find("form").Submit();

            // Assert
            cut.Markup.Should().Contain("Ogiltig e-postadress.");
            cut.Markup.Should().NotContain("Du m&#xE5;ste fylla i ett l&#xF6;senord.");
        }
    }
}
