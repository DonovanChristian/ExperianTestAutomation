using ExperianTestAutomation.Models.Requests;
using ExperianTestAutomation.Models.Responses;
using ExperianTestAutomation.Support.Interfaces;
using Newtonsoft.Json;
using System.Net;
using ExperianTestAutomation.Constants;
using ExperianTestAutomation.Support.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace ExperianTestAutomation.Steps
{
    [Binding]
    public class PhotosApiSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IPhotosApiClient _photosApiClient;

        public PhotosApiSteps(ScenarioContext scenarioContext, IPhotosApiClient photosApiClient)
        {
            _scenarioContext = scenarioContext;
            _photosApiClient = photosApiClient;
        }

        [Given(@"that photos have been created previously")]
        [Given(@"a photo with an ID of ""(?:[^""]*)"" (?:has|has not) been created previously")]
        [Given(@"an album of photos with an Album ID of ""(?:[^""]*)"" (?:has|has not) been created previously")]
        [Then(@"the photo is deleted from the database")]
        public static void NoActionRequired()
        {
            //This step is for readability purposes only
        }

        [Given(@"I want to update the following information for photo ID ""([^""]*)""")]
        public void GivenIWantToUpdateTheFollowingInformationForAPhoto(string photoId, Table table)
        {
            var updatePhotoRequest = table.CreateInstance<PhotoRequest>();

            _scenarioContext.Add(ScenarioKeys.UpdatePhotoId, int.Parse(photoId));
            _scenarioContext.Add(ScenarioKeys.UpdatePhotoRequest, updatePhotoRequest);
        }

        [Given(@"I want to search for photos where the (.*) field has a value of (.*)")]
        public void GivenIWantToSearchForPhotosUsingAFieldAndValue(string searchField, string searchValue)
        {
            _scenarioContext.Add(ScenarioKeys.SearchField, searchField);
            _scenarioContext.Add(ScenarioKeys.SearchValue, searchValue);
        }

        [When(@"I submit a ""([^""]*)"" request to the photos API")]
        public async Task WhenISubmitARequestToThePhotosApiAsync(string requestName)
        {
            switch (requestName)
            {
                case "Create New Photo":
                    await SubmitCreateAPhotoRequestAsync();
                    break;
                case "Retrieve All Photos":
                    await SubmitRetrieveAllPhotosRequestAsync();
                    break;
                case "Search Photos":
                    var searchField = _scenarioContext.Get<string>(ScenarioKeys.SearchField);
                    var searchValue = _scenarioContext.Get<string>(ScenarioKeys.SearchValue);
                    await SubmitSearchPhotosRequestAsync(searchField, searchValue);
                    break;
                default:
                    throw new ArgumentException($"Unknown name for API request: {requestName}");
            }
        }

        [When(@"I submit (?:a|an) ""([^""]*)"" request with an ID of ""([^""]*)"" to the photos API")]
        public async Task WhenISubmitARequestWithAnIdToThePhotosApi(string requestName, string id)
        {
            switch (requestName)
            {
                case "Get Photo":
                    await SubmitGetPhotoByIdRequestAsync(id);
                    break;
                case "Get Album Photos":
                    await SubmitGetAlbumByIdRequestAsync(id);
                    break;
                case "Delete Photo":
                    await SubmitDeletePhotoByIdRequestAsync(id);
                    break;
                case "Update Photo":
                    var updateRequest = _scenarioContext.Get<PhotoRequest>(ScenarioKeys.UpdatePhotoRequest);
                    await SubmitAnUpdatePhotoRequestAsync(id, updateRequest);
                    break;
                default:
                    throw new ArgumentException($"Unknown name ({requestName}) for API request with an ID");
            }
        }

        [Then(@"the new photo is created successfully")]
        public void ThenTheNewPhotoIsCreatedSuccessfully()
        {
            var expectedResult = _scenarioContext.Get<PhotoRequest>(ScenarioKeys.CreatePhotoRequest);
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);
            var actualResponse = _scenarioContext.Get<PhotoResponse>(ScenarioKeys.CreatePhotoResponseBody);

            responseStatusCode.Should().Be(HttpStatusCode.Created);
            actualResponse.AlbumId.Should().Be(expectedResult.AlbumId);
            actualResponse.Title.Should().Be(expectedResult.Title);
            actualResponse.Url.Should().Be(expectedResult.Url);
            actualResponse.ThumbnailUrl.Should().Be(expectedResult.ThumbnailUrl);
            actualResponse.Id.Should().Be(TestConstants.ExpectedNewPhotoId);
        }

        [Then(@"all of the existing photos are returned")]
        public void ThenAllOfTheExistingPhotosAreReturned()
        {
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);
            var actualResponse = _scenarioContext.Get<List<PhotoResponse>>(ScenarioKeys.GetPhotosResponseBody);

            responseStatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.Should().HaveCount(TestConstants.ExpectedNoOfExistingPhotos);
        }

        [Then(@"the correct information for the requested photo is returned")]
        public void ThenTheCorrectInformationForTheRequestedPhotoIsReturned()
        {
            var expectedResult = _scenarioContext.Get<PhotoResponse>(ScenarioKeys.ExpectedPhotoResponse);
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);
            var actualResponse = _scenarioContext.Get<PhotoResponse>(ScenarioKeys.GetPhotoResponseBody);

            responseStatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.AlbumId.Should().Be(expectedResult.AlbumId);
            actualResponse.Title.Should().Be(expectedResult.Title);
            actualResponse.Url.Should().Be(expectedResult.Url);
            actualResponse.ThumbnailUrl.Should().Be(expectedResult.ThumbnailUrl);
            actualResponse.Id.Should().Be(expectedResult.Id);
        }

        [Then(@"the API returns a ""([^""]*)"" status code")]
        public void ThenTheApiReturnsAStatusCode(string status)
        {
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);

            switch (status)
            {
                case "Successful":
                    responseStatusCode.Should().Be(HttpStatusCode.OK);
                    break;
                case "Not Found":
                    responseStatusCode.Should().Be(HttpStatusCode.NotFound);
                    break;
                default:
                    throw new ArgumentException($"Unknown API status detected: {status}");
            }
        }

        [Then(@"all of the photos within the requested album are returned")]
        public void ThenAllOfThePhotosWithinTheRequestedAlbumAreReturned()
        {
            var expectedAlbumId = _scenarioContext.Get<int>(ScenarioKeys.RequestedAlbumId);
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);
            var actualResponse = _scenarioContext.Get<List<PhotoResponse>>(ScenarioKeys.GetAlbumResponseBody);

            responseStatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            foreach (var photo in actualResponse)
            {
                photo.AlbumId.Should().Be(expectedAlbumId);
            }
            actualResponse.Should().HaveCount(TestConstants.ExpectedNoOfPhotosInAlbum);
        }

        [Then(@"no photos are returned for the requested album")]
        public void ThenNoPhotosAreReturnedForTheRequestedAlbum()
        {
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);
            var actualResponse = _scenarioContext.Get<List<PhotoResponse>>(ScenarioKeys.GetAlbumResponseBody);

            responseStatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.Should().HaveCount(TestConstants.ZeroItems);
        }

        [Then(@"the updated photo contains the following information")]
        public void ThenTheUpdatedPhotoContainsTheFollowingInformation(Table table)
        {
            var expectedResponse = table.CreateInstance<PhotoResponse>();
            var expectedPhotoId = _scenarioContext.Get<int>(ScenarioKeys.UpdatePhotoId);
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);
            var actualResponse = _scenarioContext.Get<PhotoResponse>(ScenarioKeys.UpdatePhotoResponseBody);

            responseStatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.AlbumId.Should().Be(expectedResponse.AlbumId);
            actualResponse.Title.Should().Be(expectedResponse.Title);
            actualResponse.Url.Should().Be(expectedResponse.Url);
            actualResponse.ThumbnailUrl.Should().Be(expectedResponse.ThumbnailUrl);
            actualResponse.Id.Should().Be(expectedPhotoId);
        }

        [Then(@"all of the photos that match the search parameters are returned")]
        public void ThenAllOfThePhotosThatMatchTheSearchParametersAreReturned()
        {
            var expectedSearchField = _scenarioContext.Get<string>(ScenarioKeys.SearchField);
            var expectedSearchValue = _scenarioContext.Get<string>(ScenarioKeys.SearchValue);
            var responseStatusCode = _scenarioContext.Get<HttpStatusCode>(ScenarioKeys.ResponseStatusCode);
            var actualResponse = _scenarioContext.Get<List<PhotoResponse>>(ScenarioKeys.SearchPhotosResponseBody);

            responseStatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            foreach (var photo in actualResponse)
            {
                switch (expectedSearchField)
                {
                    case "title":
                        photo.Title.Should().Be(expectedSearchValue);
                        break;
                    case "url":
                        photo.Url.Should().Be(expectedSearchValue);
                        break;
                    case "thumbnailUrl":
                        photo.ThumbnailUrl.Should().Be(expectedSearchValue);
                        break;
                    default:
                        throw new ArgumentException($"Cannot verify search parameter ({expectedSearchField})");
                }
            }
            actualResponse.Should().HaveCountGreaterThan(TestConstants.ZeroItems);
        }

        private async Task SubmitCreateAPhotoRequestAsync()
        {
            var newPhotoRequest = DataHelper.GetNewPhotoData();
            var newPhotoResponse = await _photosApiClient.CreateAPhotoAsync(newPhotoRequest);
            var responseContent = await newPhotoResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<PhotoResponse>(responseContent);

            _scenarioContext.Add(ScenarioKeys.CreatePhotoRequest, newPhotoRequest);
            _scenarioContext.Add(ScenarioKeys.ResponseStatusCode, newPhotoResponse.StatusCode);
            _scenarioContext.Add(ScenarioKeys.CreatePhotoResponseBody, responseBody);
        }

        private async Task SubmitRetrieveAllPhotosRequestAsync()
        {
            var getPhotosResponse = await _photosApiClient.RetrieveAllPhotosAsync();
            var responseContent = await getPhotosResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<List<PhotoResponse>>(responseContent);

            _scenarioContext.Add(ScenarioKeys.ResponseStatusCode, getPhotosResponse.StatusCode);
            _scenarioContext.Add(ScenarioKeys.GetPhotosResponseBody, responseBody);
        }

        private async Task SubmitGetPhotoByIdRequestAsync(string photoId)
        {
            var getPhotoResponse = await _photosApiClient.GetPhotoByIdAsync(photoId);
            var responseContent = await getPhotoResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<PhotoResponse>(responseContent);

            _scenarioContext.Add(ScenarioKeys.ExpectedPhotoResponse, DataHelper.GetExpectedPhotoDataById(photoId));
            _scenarioContext.Add(ScenarioKeys.ResponseStatusCode, getPhotoResponse.StatusCode);
            _scenarioContext.Add(ScenarioKeys.GetPhotoResponseBody, responseBody);
        }

        private async Task SubmitAnUpdatePhotoRequestAsync(string photoId, PhotoRequest updateRequest)
        {
            var updatePhotoResponse = await _photosApiClient.UpdatePhotoAsync(photoId, updateRequest);
            var responseContent = await updatePhotoResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<PhotoResponse>(responseContent);

            _scenarioContext.Add(ScenarioKeys.ResponseStatusCode, updatePhotoResponse.StatusCode);
            _scenarioContext.Add(ScenarioKeys.UpdatePhotoResponseBody, responseBody);
        }

        private async Task SubmitSearchPhotosRequestAsync(string searchField, string searchValue)
        {
            var searchPhotosResponse = await _photosApiClient.SearchPhotosAsync(searchField, searchValue);
            var responseContent = await searchPhotosResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<List<PhotoResponse>>(responseContent);

            _scenarioContext.Add(ScenarioKeys.ResponseStatusCode, searchPhotosResponse.StatusCode);
            _scenarioContext.Add(ScenarioKeys.SearchPhotosResponseBody, responseBody);
        }

        private async Task SubmitGetAlbumByIdRequestAsync(string albumId)
        {
            var getAlbumResponse = await _photosApiClient.GetAlbumByIdAsync(albumId);
            var responseContent = await getAlbumResponse.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<List<PhotoResponse>>(responseContent);

            _scenarioContext.Add(ScenarioKeys.RequestedAlbumId, int.Parse(albumId));
            _scenarioContext.Add(ScenarioKeys.ResponseStatusCode, getAlbumResponse.StatusCode);
            _scenarioContext.Add(ScenarioKeys.GetAlbumResponseBody, responseBody);
        }

        private async Task SubmitDeletePhotoByIdRequestAsync(string photoId)
        {
            var deletePhotoResponse = await _photosApiClient.DeletePhotoByIdAsync(photoId);

            _scenarioContext.Add(ScenarioKeys.ResponseStatusCode, deletePhotoResponse.StatusCode);
        }
    }
}
