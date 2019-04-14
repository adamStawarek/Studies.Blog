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

    getRandomPhotos(n: number) {
        var randomPhotoNumber = Math.floor((Math.random() * 1000) + 1);
        let apiKey = this.apiKey;
        return $.ajax({
            url: "https://api.pexels.com/v1/curated?per_page=" + n + "&page=1",
            type: "GET",
            async: false,
            beforeSend(xhr) { xhr.setRequestHeader('Authorization', apiKey); },
            dataType: "json",
            success(data) {
                return data;
            }
        }).responseJSON;
    }

    getPhotosByName(n: number,name: string) {
        let apiKey = this.apiKey;
        return $.ajax({
            url:"https://api.pexels.com/v1/search?query="+name+"&per_page="+n+"&page=1",
            type: "GET",
            async: false,
            beforeSend(xhr) { xhr.setRequestHeader('Authorization', apiKey); },
            dataType: "json",
            success(data) {
                return data;
            }
        }).responseJSON;
    }
}
