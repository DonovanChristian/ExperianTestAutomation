Feature: PhotosApi
	As a user with access to the Photos API
	I want to be able to maintain the photos in my albums
	So that I have lots of pretty pictures to look at

Scenario: User must be able to create new photos
	When I submit a "Create New Photo" request to the photos API
	Then the new photo is created successfully

Scenario: User must be able to retrieve all existing photos
	Given that photos have been created previously
	When I submit a "Retrieve All Photos" request to the photos API 
	Then all of the existing photos are returned

Scenario: User must be able to retrieve a single photo
	Given a photo with an ID of "20" has been created previously
	When I submit a "Get Photo" request with an ID of "20" to the photos API 
	Then the correct information for the requested photo is returned

Scenario: Any attempt to retrieve a non-existent photo must return a Not Found status
	Given a photo with an ID of "5001" has not been created previously
	When I submit a "Get Photo" request with an ID of "5001" to the photos API 
	Then the API returns a "Not Found" status code

Scenario: User must be able to update a photo
	Given a photo with an ID of "25" has been created previously
		And I want to update the following information for photo ID "25"
		| AlbumId | Title               | Url                                | ThumbnailUrl                         |
		| 22      | Updated photo title | https://new.location.com/22/5e3a74 | https://new.thumbnail.com/150/5e3a74 |
	When I submit an "Update Photo" request with an ID of "25" to the photos API 
	Then the API returns a "Successful" status code
		And the updated photo contains the following information
		| Id | AlbumId | Title               | Url                                | ThumbnailUrl                         |
		| 25 | 22      | Updated photo title | https://new.location.com/22/5e3a74 | https://new.thumbnail.com/150/5e3a74 |

Scenario: User must be able to delete a photo
	Given a photo with an ID of "40" has been created previously
	When I submit a "Delete Photo" request with an ID of "40" to the photos API 
	Then the API returns a "Successful" status code
		And the photo is deleted from the database

Scenario Outline: User must be able to search for photos by their metadata
	Given that photos have been created previously
		And I want to search for photos where the <SearchField> field has a value of <SearchValue>
	When I submit a "Search Photos" request to the photos API
	Then the API returns a "Successful" status code
		And all of the photos that match the search parameters are returned

Examples:
	| SearchField  | SearchValue                            |
	| title        | aut magni quibusdam cupiditate ea      |
	| url          | https://via.placeholder.com/600/7644fe |
	| thumbnailUrl | https://via.placeholder.com/150/36d137 |

Scenario: User must be able to retrieve all photos from a single album
	Given an album of photos with an Album ID of "15" has been created previously
	When I submit a "Get Album Photos" request with an ID of "15" to the photos API 
	Then all of the photos within the requested album are returned

Scenario: Any attempt to retrieve photos from a non-existent album must return an empty list of results
	Given an album of photos with an Album ID of "101" has not been created previously
	When I submit a "Get Album Photos" request with an ID of "101" to the photos API 
	Then the API returns a "Successful" status code
		But no photos are returned for the requested album
