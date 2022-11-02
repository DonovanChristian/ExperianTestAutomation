using ExperianTestAutomation.Models.Requests;

namespace ExperianTestAutomation.Support.Interfaces
{
    public interface IPhotosApiClient
    {
        Task<HttpResponseMessage> CreateAPhotoAsync(PhotoRequest request);

        Task<HttpResponseMessage> RetrieveAllPhotosAsync();

        Task<HttpResponseMessage> GetPhotoByIdAsync(string photoId);

        Task<HttpResponseMessage> UpdatePhotoAsync(string photoId, PhotoRequest request);

        Task<HttpResponseMessage> DeletePhotoByIdAsync(string photoId);

        Task<HttpResponseMessage> SearchPhotosAsync(string requestParam, string paramValue);

        Task<HttpResponseMessage> GetAlbumByIdAsync(string albumId);
    }
}
