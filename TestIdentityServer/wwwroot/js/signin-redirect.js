var goToRedirectURl = document.querySelector("meta[http-equiv=refresh]").getAttribute("data-url");

window.location.href = goToRedirectURl;

var redirectButton = document.getElementById("myButton")
if (!!myButton) {
    redirectButton.onclick = myFunction;
}

function myFunction() {
    window.location.href = goToRedirectURl;
}


