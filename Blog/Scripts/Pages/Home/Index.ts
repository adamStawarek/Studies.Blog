var photoService = new PhotoService();
var photo = photoService.getRandomPhoto();
console.log(photo);
var welcome = new PexelsResult();
$.extend(welcome, photo);
console.log(welcome);
console.log(welcome.photos[0].url);