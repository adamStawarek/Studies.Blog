import Test = require("../../Shared/Test");
import Test1 = Test.Test;

class PhotoService {
    protected apiKey: string = "563492ad6f917000010000014355d4b9487041c3b4b9062a0271c40a";

    getRandomPhoto() {
        var randomPhotoNumber = Math.floor((Math.random() * 1000) + 1);
        let apiKey = this.apiKey;
        $.ajax({
            url: "https://api.pexels.com/v1/curated?per_page=10&page="+randomPhotoNumber,
            type: "GET",
            beforeSend: function (xhr) { xhr.setRequestHeader('Authorization', apiKey); },
            success: function (data) {
                return data;
            }
        });
       
    }
}

var test = new Test1();
test.printHello();