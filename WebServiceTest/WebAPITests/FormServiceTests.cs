using ClassLibrary;
using System.IO;
using System.Net.Http.Headers;

namespace WebAPITests
{
    [TestClass]
    public class FormServiceTests
    {
        [TestMethod]
        public async Task Can_GetForms_Test()
        {
            // arrange
            HttpClient client = new HttpClient();
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://localhost:7224/");
            // Sets the Accept header to "application/json".Setting this header tells the server to send data in JSON format.
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // act
            IEnumerable<Form> forms = null;
            HttpResponseMessage response = await client.GetAsync(@"Forms/GetForm");
            if (response.IsSuccessStatusCode)
            {
                forms = await response.Content.ReadAsAsync<IEnumerable<Form>>();
            }

            // assert
            Assert.IsNotNull(forms);
        }
    }
}