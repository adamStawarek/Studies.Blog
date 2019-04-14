var photoService = new PhotoService();
var currentPhotoIndex = 0;
var photos: Photo[] = [];
var numberOfLoadedPhotos = 15;

function beforeSubmit() {
    $('#modelTags').val($('#tagInput').val());
    $('#hiddenImg').val($('#displayedImg').attr("src"));
}

function setUpPhotos(updateImg: boolean=true) {
    var keyword = $("#keywordInput").val();
    var pexelsResult = new PexelsResult();
    if (keyword == "") {
        var result1 = photoService.getRandomPhotos(numberOfLoadedPhotos);
        $.extend(pexelsResult, result1);
    } else {
        var result2 = photoService.getPhotosByName(numberOfLoadedPhotos, keyword.toString());
        $.extend(pexelsResult, result2);
    }
    photos = pexelsResult.photos;

    console.log(photos);
    currentPhotoIndex = Math.floor(photos.length / 2);
    if (updateImg) {
        updateImage();
    }    
}

function updateImage() {
    $('#displayedImg').attr("src", photos[currentPhotoIndex].src.medium);
}

function previousImage() {
    if (currentPhotoIndex <= 0) {
        currentPhotoIndex = photos.length - 1;
    } else {
        currentPhotoIndex--;
    }
    updateImage();
}

function nextImage() {
    if (currentPhotoIndex >= photos.length - 1) {
        currentPhotoIndex = 0;
    } else {
        currentPhotoIndex++;
    }
    updateImage();
}