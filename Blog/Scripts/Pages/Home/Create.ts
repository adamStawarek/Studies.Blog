
var currentPic = Math.floor((Math.random() * 1084) + 1);
changePic();

$("#add_tag").on('keyup',
    function (e) {
        if (e.keyCode == 13) {
            addTags();
        }
    });

$("#postContent").on('keyup',
    function (e) {
        if (e.keyCode == 13) {
            var newValue = $("#postContent").val() + "\n";
            $("#postContent").val(newValue);
        }
    });

$("#btnChangePic").click(function (e) {
    changePic();
});

function changePic() {
    var photoService = new PhotoService();
    var photo = photoService.getRandomPhoto();
    var result = new PexelsResult();
    $.extend(result, photo);
    console.log(result);
    console.log(result.photos[0].src.medium);
    $('#postImg').attr("src", result.photos[0].src.medium);
}

function addTags() {
    $('#modelTags').val($('#tagInput').val());
    $('#image').val($('#postImg').attr("src"));
}