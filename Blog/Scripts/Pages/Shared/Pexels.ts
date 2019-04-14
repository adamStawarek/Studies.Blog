class PexelsResult {
    total_results: number;
    page: number;
    per_page: number;
    photos: Photo[];
    next_page: string;
}

class Photo {
    id: number;
    width: number;
    height: number;
    url: string;
    photographer: string;
    photographer_url: string;
    src: Src;
}

class Src {
    original: string;
    large2x: string;
    large: string;
    medium: string;
    small: string;
    portrait: string;
    landscape: string;
    tiny: string;
}

class PhotoService {
    protected apiKey: string = "563492ad6f917000010000014355d4b9487041c3b4b9062a0271c40a";

    getRandomPhoto() {
        var randomPhotoNumber = Math.floor((Math.random() * 1000) + 1);
        let apiKey = this.apiKey;
        return $.ajax({
            url: "https://api.pexels.com/v1/search?query=berry&per_page=15&page=1",
            type: "GET",
            async: false,
            beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', apiKey); },
            dataType: "json",
            success: function (data) {
                return data;
            }
        }).responseJSON;
    }
}
