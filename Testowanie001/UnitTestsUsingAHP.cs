using HtmlAgilityPack;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net;

namespace Testowanie001
{
    [TestFixture]
    public class AgilityHtmlTest
    {
        private HtmlDocument? _contactPageDoc;
        private HttpClient? _client;
        private HttpClientHandler? _handler;
        private const string ContactPageUrl = "https://localhost:7201/Contact/Index";
        private const string ContactPagePostUrl = "https://localhost:7201/Contact/Index";
        private const string ConfirmationPageUrl = "https://localhost:7201/Contact/Confirmation";


        [SetUp]
        public async Task Setup()
        {
            _handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
                AllowAutoRedirect = false
            };
            _client = new HttpClient(_handler);

            try
            {
                var html = await _client.GetStringAsync(ContactPageUrl);
                _contactPageDoc = new HtmlDocument();
                _contactPageDoc.LoadHtml(html);
            }
            catch (HttpRequestException ex)
            {
                _contactPageDoc = null;
                Assert.Fail($"Failed to retrieve HTML from {ContactPageUrl}. Ensure the web application is running and accessible. Details: {ex.Message}");
            }
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _handler?.Dispose();
        }

        // Test 1: Check if the contact form and its main elements exist
        [Test]
        public void ContactForm_Should_Exist_With_Required_Elements()
        {
            Assert.IsNotNull(_contactPageDoc, "HtmlDocument for contact page was not loaded in Setup.");
            if (_contactPageDoc == null) return;

            var form = _contactPageDoc.DocumentNode.SelectSingleNode("//form[@action='/Contact/Index' or @action='/Contact']");
            var nameInput = _contactPageDoc.DocumentNode.SelectSingleNode("//input[@id='Name' and @name='Name']");
            var emailInput = _contactPageDoc.DocumentNode.SelectSingleNode("//input[@id='Email' and @name='Email']");
            var messageTextarea = _contactPageDoc.DocumentNode.SelectSingleNode("//textarea[@id='Message' and @name='Message']");
            var submitButton = _contactPageDoc.DocumentNode.SelectSingleNode("//button[@type='submit']");
            var antiForgeryToken = _contactPageDoc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']");

            Assert.IsNotNull(form, "Formularz kontaktowy nie istnieje lub ma nieprawidłowy atrybut action.");
            Assert.IsNotNull(nameInput, "Pole 'Imię i nazwisko' (Name) nie istnieje.");
            Assert.IsNotNull(emailInput, "Pole 'Adres email' (Email) nie istnieje.");
            Assert.IsNotNull(messageTextarea, "Pole 'Wiadomość' (Message) nie istnieje.");
            Assert.IsNotNull(submitButton, "Przycisk 'Wyślij Wiadomość' nie istnieje.");
            Assert.IsNotNull(antiForgeryToken, "Brak tokenu AntiForgery (__RequestVerificationToken).");
        }

        // Test 2: Check if input elements have expected validation attributes
        [Test]
        public void Inputs_Should_Have_Validation_Attributes()
        {
            Assert.IsNotNull(_contactPageDoc, "HtmlDocument for contact page was not loaded in Setup.");
            if (_contactPageDoc == null) return;

            var nameInput = _contactPageDoc.DocumentNode.SelectSingleNode("//input[@id='Name']");
            var emailInput = _contactPageDoc.DocumentNode.SelectSingleNode("//input[@id='Email']");
            var messageTextarea = _contactPageDoc.DocumentNode.SelectSingleNode("//textarea[@id='Message']");

            Assert.IsNotNull(nameInput?.Attributes["data-val-required"]?.Value, "Pole 'Name' powinno mieć atrybut 'data-val-required'.");
            Assert.IsNotNull(emailInput?.Attributes["data-val-required"]?.Value, "Pole 'Email' powinno mieć atrybut 'data-val-required'.");
            Assert.IsNotNull(messageTextarea?.Attributes["data-val-required"]?.Value, "Pole 'Message' powinno mieć atrybut 'data-val-required'.");

            Assert.IsNotNull(emailInput?.Attributes["data-val-email"]?.Value, "Pole 'Email' powinno mieć atrybut 'data-val-email'.");

            Assert.IsNotNull(nameInput?.Attributes["data-val-length-max"]?.Value, "Pole 'Name' powinno mieć atrybut 'data-val-length-max'.");
            Assert.That(nameInput?.Attributes["data-val-length-max"]?.Value, Is.EqualTo("100"), "Max length for Name is incorrect.");

            Assert.IsNotNull(messageTextarea?.Attributes["data-val-length-max"]?.Value, "Pole 'Message' powinno mieć atrybut 'data-val-length-max'.");
            Assert.That(messageTextarea?.Attributes["data-val-length-max"]?.Value, Is.EqualTo("2000"), "Max length for Message is incorrect.");
        }

        // Test 3: Simulate a successful form POST and check the confirmation page
        [Test]
        public async Task Successful_Form_Post_Should_Redirect_To_Confirmation_With_SuccessMessage()
        {
            Assert.IsNotNull(_contactPageDoc, "HtmlDocument for contact page was not loaded in Setup.");
            if (_contactPageDoc == null) { Assert.Fail("Setup failed to load contact page doc."); return; }
            Assert.IsNotNull(_client, "HttpClient was not initialized in Setup.");

            // Arrange: Prepare form data
            var antiForgeryToken = ExtractAntiForgeryToken(_contactPageDoc.DocumentNode.OuterHtml);
            Assert.IsNotEmpty(antiForgeryToken, "AntiForgeryToken could not be extracted from the contact page.");
           
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "Test User From NUnit"),
                new KeyValuePair<string, string>("Email", "test.nunit@example.com"),
                new KeyValuePair<string, string>("Message", "This is a test message from an NUnit test."),
                new KeyValuePair<string, string>("__RequestVerificationToken", antiForgeryToken!)
            });

            // Act: Post the form
            HttpResponseMessage? postResponse = null;
            string responseContentOnError = string.Empty;
            try
            {
                postResponse = await _client!.PostAsync(ContactPagePostUrl, formData);
                if (postResponse.StatusCode != HttpStatusCode.Redirect && postResponse.StatusCode != HttpStatusCode.Found)
                {
                    responseContentOnError = await postResponse.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                Assert.Fail($"Failed to POST to {ContactPagePostUrl}. Ensure the web application is running and accessible. Details: {ex.Message}");
            }

            Assert.IsNotNull(postResponse, "POST response is null.");
            if (postResponse == null) return;

            bool wasInitialRedirectToConfirmation = (postResponse.StatusCode == HttpStatusCode.Redirect || postResponse.StatusCode == HttpStatusCode.Found) &&
                                                  postResponse.Headers.Location != null &&
                                                  postResponse.Headers.Location.ToString().EndsWith("/Contact/Confirmation");

            Assert.IsTrue(wasInitialRedirectToConfirmation,
                $"POST should initially redirect to Confirmation page. Actual Status: {postResponse.StatusCode}, Location: {postResponse.Headers.Location?.ToString() ?? "N/A"}. " +
                $"{(string.IsNullOrEmpty(responseContentOnError) ? "" : "Response Content: " + responseContentOnError)}");

            string? confirmationPageUrlFromRedirect = null;
            if (postResponse.Headers.Location != null)
            {
                confirmationPageUrlFromRedirect = postResponse.Headers.Location.IsAbsoluteUri
                                    ? postResponse.Headers.Location.ToString()
                                    : new System.Uri(new System.Uri(ContactPagePostUrl), postResponse.Headers.Location).ToString();
            }
            else
            {
                Assert.Fail("Location header was null after POST, cannot determine confirmation page URL.");
                return;
            }
            Assert.IsNotNull(confirmationPageUrlFromRedirect, "confirmationPageUrlFromRedirect is null");
            if (confirmationPageUrlFromRedirect == null) return;

            using var getHandler = new HttpClientHandler { UseCookies = true, CookieContainer = _handler!.CookieContainer, AllowAutoRedirect = true };
            using var getClient = new HttpClient(getHandler);
            var confirmationResponse = await getClient.GetAsync(confirmationPageUrlFromRedirect);

            string? finalLandedPathAfterGet = confirmationResponse.RequestMessage?.RequestUri?.LocalPath.TrimEnd('/');
            string expectedConfirmationPath = new Uri(ConfirmationPageUrl).LocalPath.TrimEnd('/');

            Assert.That(finalLandedPathAfterGet, Is.EqualTo(expectedConfirmationPath),
                $"Expected to land on the Confirmation page ('{expectedConfirmationPath}'), but landed on '{finalLandedPathAfterGet ?? "UNKNOWN"}'. This might indicate TempData loss leading to a second redirect from Confirmation to Index.");

            confirmationResponse.EnsureSuccessStatusCode();
            var confirmationHtml = await confirmationResponse.Content.ReadAsStringAsync();
            var confirmationDoc = new HtmlDocument();
            confirmationDoc.LoadHtml(confirmationHtml);

            var successMessageParagraph = confirmationDoc.DocumentNode.SelectSingleNode("//p[contains(@class, 'text-gray-600') and contains(@class, 'text-lg') and contains(@class, 'mb-8')]");

            if (successMessageParagraph == null)
            {
                TestContext.WriteLine("---- DEBUG: Confirmation Page HTML ----");
                TestContext.WriteLine(confirmationHtml);
                TestContext.WriteLine("---- END DEBUG: Confirmation Page HTML ----");
            }
            Assert.IsNotNull(successMessageParagraph, "Paragraf z komunikatem o sukcesie nie został znaleziony. Sprawdź HTML w teście output. Być może struktura HTML strony potwierdzenia jest inna niż oczekiwana, lub TempData nie zadziałało poprawnie.");
            if (successMessageParagraph == null) return;

            Assert.That(WebUtility.HtmlDecode(successMessageParagraph.InnerText), Does.Contain("Twoje zapytanie zostało wysłane pomyślnie").IgnoreCase, "Komunikat o sukcesie nie pasuje.");
        }

        // Test 4: Check the basic structure of the Confirmation page when accessed directly (no TempData)
        [Test]
        public async Task ConfirmationPage_Should_Redirect_To_Index_When_Accessed_Directly_Without_TempData()
        {
            Assert.IsNotNull(_client, "HttpClient was not initialized in Setup.");
            Assert.IsNotNull(_handler, "HttpClientHandler was not initialized in Setup.");


            using var redirectFollowingHandler = new HttpClientHandler { UseCookies = true, CookieContainer = _handler!.CookieContainer, AllowAutoRedirect = true };
            using var redirectFollowingClient = new HttpClient(redirectFollowingHandler);

            HttpResponseMessage? response = null;
            try
            {
                response = await redirectFollowingClient.GetAsync(ConfirmationPageUrl);
            }
            catch (HttpRequestException ex)
            {
                Assert.Fail($"Failed to retrieve HTML from {ConfirmationPageUrl}. Ensure the web application is running and accessible. Details: {ex.Message}");
            }

            Assert.IsNotNull(response, "Response from Confirmation page is null.");
            if (response == null) return;

            string? finalLandedPath = response.RequestMessage?.RequestUri?.LocalPath.TrimEnd('/');
            string expectedIndexPath1 = "/Contact/Index";
            string expectedIndexPath2 = "/Contact";

            bool landedOnIndexPage = string.Equals(finalLandedPath, expectedIndexPath1, StringComparison.OrdinalIgnoreCase) ||
                                     string.Equals(finalLandedPath, expectedIndexPath2, StringComparison.OrdinalIgnoreCase);

            Assert.IsTrue(landedOnIndexPage,
                $"When Confirmation page is accessed without TempData, it should redirect to Index ('{expectedIndexPath1}' or '{expectedIndexPath2}'). Instead, landed on '{finalLandedPath ?? "UNKNOWN"}'.");

            if (landedOnIndexPage)
            {
                response.EnsureSuccessStatusCode();
                var htmlString = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlString);

                var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
                Assert.IsNotNull(titleNode, "Title tag not found on the landed Index page.");
                if (titleNode == null) return;

                string decodedTitle = WebUtility.HtmlDecode(titleNode.InnerText);
                Assert.That(decodedTitle, Does.Contain("Skontaktuj się z Nami - TechSolutions").IgnoreCase, "Title on landed Index page is incorrect.");
            }
        }


        // Helper to extract AntiForgeryToken
        private string? ExtractAntiForgeryToken(string? htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                return null;
            }
            var tempDoc = new HtmlDocument();
            tempDoc.LoadHtml(htmlContent);
            var tokenNode = tempDoc.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']");
            return tokenNode?.GetAttributeValue("value", null);
        }
    }
}