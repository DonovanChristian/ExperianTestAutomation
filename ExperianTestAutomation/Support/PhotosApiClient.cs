using System.Text;
using ExperianTestAutomation.Models;
using ExperianTestAutomation.Models.Requests;
using ExperianTestAutomation.Support.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ExperianTestAutomation.Support
{
    public class PhotosApiClient : IPhotosApiClient
    {
        private readonly UrlsConfigSettings _urlsConfig;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public PhotosApiClient(UrlsConfigSettings urlsConfig)
        {
            _urlsConfig = urlsConfig;
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public async Task<HttpResponseMessage> CreateAPhotoAsync(PhotoRequest request)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_urlsConfig.PhotosApiUrl);

            return await client.PostAsync("photos",
                new StringContent(JsonConvert.SerializeObject(request, _jsonSerializerSettings),
                    Encoding.Default,
                    "application/json"));
        }

        public async Task<HttpResponseMessage> RetrieveAllPhotosAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_urlsConfig.PhotosApiUrl);

            return await client.GetAsync("photos");
        }

        public async Task<HttpResponseMessage> GetPhotoByIdAsync(string photoId)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_urlsConfig.PhotosApiUrl);

            return await client.GetAsync($"photos/{photoId}");
        }

        public async Task<HttpResponseMessage> UpdatePhotoAsync(string photoId, PhotoRequest request)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_urlsConfig.PhotosApiUrl);

            return await client.PutAsync($"photos/{photoId}",
                new StringContent(JsonConvert.SerializeObject(request, _jsonSerializerSettings),
                    Encoding.Default,
                    "application/json"));
        }

        public async Task<HttpResponseMessage> DeletePhotoByIdAsync(string photoId)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_urlsConfig.PhotosApiUrl);

            return await client.DeleteAsync($"photos/{photoId}");
        }

        public async Task<HttpResponseMessage> SearchPhotosAsync(string requestParam, string paramValue)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_urlsConfig.PhotosApiUrl);

            return await client.GetAsync($"photos?{requestParam}={paramValue}");
        }

        public async Task<HttpResponseMessage> GetAlbumByIdAsync(string albumId)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(_urlsConfig.PhotosApiUrl);

            return await client.GetAsync($"album/{albumId}/photos");
        }
    }
}
