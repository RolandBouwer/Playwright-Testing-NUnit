using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


[Parallelizable(ParallelScope.Self)]
public class Tests : PageTest
{
    public static string webAppUrl;

    [OneTimeSetUp]
    public void Init()
    {
        webAppUrl = TestContext.Parameters["WebAppUrl"] 
            ?? throw new Exception("WebAppUrl is not configured as a parameter.");
    }


    [Test]
    public async Task Clicking_ContactButton_Goes_To_ContactForm()
    {
        await Page.GotoAsync(webAppUrl);
        var formButton = Page.Locator("text=Open Contact Form");
        await formButton.ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*Home/Form"));
    }

    [Test]
    public async Task Filling_And_Submitting_ContactForm_Goes_To_SuccessPage()
    {
        await Page.GotoAsync($"{webAppUrl}/Home/Form");
        await Page.Locator("text=First name").FillAsync("Néstor");
        await Page.Locator("text=Last name").FillAsync("Campos");
        await Page.Locator("text=Email address").FillAsync("nestor@gmail.com");
        await Page.Locator("text=Birth date").FillAsync("1989-03-16");
        await Page.Locator("text=Send").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*Home/Success"));
    }

    [Test]
    public async Task Filling_Invalid_Email_Should_Show_ValidationError()
    {
        await Page.GotoAsync($"{webAppUrl}/Home/Form");

        ILocator emailValidationLocator = Page.Locator("text=The Email address field is not a valid e-mail address.");
        await Expect(emailValidationLocator).Not.ToBeVisibleAsync();

        await Page.Locator("text=Email address").FillAsync("nestorgmail.com");
        await Page.Locator("text=Send").ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(".*Home/Form"));
        await Expect(emailValidationLocator).ToBeVisibleAsync();
    }

}
