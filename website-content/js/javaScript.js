function clickSmallNavMenu() {
  var smallNavBar = document.getElementById("small-nav-bar");
    
  if(smallNavBar.className.indexOf("w3-show") != -1) {
      smallNavBar.className = smallNavBar.className.replace(" w3-show", "");
  }
  else {
      smallNavBar.className += " w3-show";
  }
}

// Gallery slides
var currentImage = 1;
showImage(currentImage);

function plusDivs(n) {
  showImage(currentImage += n);
}

function currentDiv(n) {
  showImage(currentImage = n);
}

function showImage(n) {
  var galleryImage = document.getElementsByClassName("galleryImageSlides");
    
  //var dots = document.getElementsByClassName("demo");
  if (n > galleryImage.length) {currentImage = 1}
  
  if (n < 1) {currentImage = galleryImage.length}
  
  var i;
  for (i = 0; i < galleryImage.length; i++) {
    galleryImage[i].style.display = "none";  
  }
    
  //for (i = 0; i < dots.length; i++) {
  //dots[i].className = dots[i].className.replace(" w3-white", "");
  //}
    
  galleryImage[currentImage-1].style.display = "block";  
  //dots[currentImage-1].className += " w3-white";
}