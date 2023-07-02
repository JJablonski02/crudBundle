using FluentAssertions;
using System.Net.Http;
using Xunit;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace crudTests
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
        #region Index
        [Fact]
        public async Task Index_ToReturnView()
        {
            //Arrange
            

            //Act
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            //Assert
            response.Should().BeSuccessful(); //2xx
            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);

            var document = html.DocumentNode;
            document.QuerySelectorAll("table.persons").Should().NotBeNull(); // Css class with class of persons (located in Index table class= "... persons")
        }
        #endregion
    }
}
