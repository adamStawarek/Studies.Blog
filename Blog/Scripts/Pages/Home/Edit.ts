var photoService = new PhotoService();
var currentPhotoIndex = 0;
var photos: Photo[] = [];
var numberOfLoadedPhotos = 50;

window.onload = () => {  

    setUpPhotos(false);

    $("#add_tag").on('keyup', e => {
        if (e.keyCode == 13) {
            $('#modelTags').val($('#tagInput').val());
        }
    });

    $("#btnPrev").click(() => {
        previousImage();
    });

    $("#btnNext").click(() => {
        nextImage();
    });

    $("#btnKeyword").click(() => {
        setUpPhotos();
    });
}