using ExperianTestAutomation.Models.Requests;
using ExperianTestAutomation.Models.Responses;

namespace ExperianTestAutomation.Support.Helpers
{
    public static class DataHelper
    {
        public static PhotoRequest GetNewPhotoData()
        {
            return new PhotoRequest
            {
                AlbumId = 10,
                Title = "This is a new photo for the album",
                Url = @"https://via.placeholder.com/600/5e912b",
                ThumbnailUrl = @"https://via.placeholder.com/150/5e912b"
            };
        }

        public static PhotoResponse GetExpectedPhotoDataById(string photoId)
        {
            return new PhotoResponse
            {
                Id = int.Parse(photoId),
                AlbumId = 1,
                Title = "assumenda voluptatem laboriosam enim consequatur veniam placeat reiciendis error",
                Url = @"https://via.placeholder.com/600/8985dc",
                ThumbnailUrl = @"https://via.placeholder.com/150/8985dc"
            };
        }
    }
}
