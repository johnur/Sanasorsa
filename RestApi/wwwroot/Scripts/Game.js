const maxRounds = 3;
const apiUrl = "https://sanasorsa.azurewebsites.net/api/sorsa";
//const apiUrl = "https://localhost:44334/api/sorsa";
var tallenna;


const input = document.getElementById('phraseDiv')
let words = []

localStorage.setItem('phraseDiv', JSON.stringify(words))
const data = JSON.parse(localStorage.getItem('phraseDiv'))

var p = document.getElementById('phraseDiv');

var timeoutID;


// TAIMER ALKAA TÄSTÄ ja sen jälkeen sanat näkyville 
$("#startRecognizeOnceAsyncButton").click(function () {
    var counter = 10;
    $.ajax({
        url: apiUrl,
        success: function (data) {
            console.log(data)
            $("#prompt").empty();
            $("#prompt").append(data);
            $("#prompt").removeClass("hidden");
            $("#buttons").html("");
            setInterval(function () {
                counter--;
                if (counter >= 0) {

                    span = document.getElementById("count");
                    span.innerHTML = "Aikaa jäljellä: " + counter;
                }
                if (counter === 0) {
                    span.innerHTML = ('Aika loppui, pisteitä lasketaan...');
                }
                if (counter === -2) {
                    clearInterval(counter);
                    SanatNäkyville();
                }
            }, 1000);
        },
        error: function (xhr, status, errorThrown) {
            $("#scores").append($("<div/>", {
                "class": "warning",
                text: "Hups, sanojen hakemisessa näyttää olevan jotain pielessä, yritä uudelleen."
            }));
            $("#buttons").html($("<button/>", {
                "id": "reloadButton",
                text: "Yritä uudelleen",
                "click": function () {
                    location.reload();
                }
            }))
        },
        dataType: "text"
    });
    // $("#buttons").html("");
    // setInterval(function () {
    //     counter--;
    //     if (counter >= 0) {

    //         span = document.getElementById("count");
    //         span.innerHTML = "Aikaa jäljellä: " + counter;
    //     }
    //     if (counter === 0) {
    //         span.innerHTML = ('Aika loppui, pisteitä lasketaan...');
    //     }
    //     if (counter === -2) {
    //         clearInterval(counter);
    //         SanatNäkyville();
    //     }
    // }, 1000);
});

function SanatNäkyville() {
    GetScores();
}

// SPEECH TO TEXT ALKAA TÄSTÄ

// tekstikenttä ja starttinappula
var phraseDiv;
var startRecognizeOnceAsyncButton;

// azuren hommat
var subscriptionKey;
var serviceRegion;
var authorizationToken;
var SpeechSDK;
var recognizer;
var authorizationEndpoint = apiUrl + "/getToken";

function RequestAuthorizationToken() {
    console.log(authorizationEndpoint);
    if (authorizationEndpoint) {
        var a = new XMLHttpRequest();
        a.open("GET", authorizationEndpoint, false);
        a.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        a.setRequestHeader("Access-Control-Allow-Origin", "*");

        a.onload = function () {
            var token = JSON.parse(atob(this.responseText.split(".")[1]));
            serviceRegion = token.region;
            authorizationToken = this.responseText; //oli ennen responseText
            subscriptionKey.disabled = true;
            subscriptionKey.value = "using authorization token (hit F5 to refresh)";
            console.log("Got an authorization token: " + token);
        }
        a.send();
        console.log(a);
    }
}


document.addEventListener("DOMContentLoaded", function () {
    var round = sessionStorage.getItem("round");
    if (round == null || round == 0 || round > 3) {
        sessionStorage.setItem("total", 0);
        sessionStorage.setItem("round", 1);
    }
    $("#totalScore").append(sessionStorage.getItem("total"));
    $("#roundNumber").append(sessionStorage.getItem("round"));
    startRecognizeOnceAsyncButton = document.getElementById("startRecognizeOnceAsyncButton");

    subscriptionKey = "";
    serviceRegion = "";

    phraseDiv = document.getElementById("phraseDiv");
    startRecognizeOnceAsyncButton.addEventListener("click", function () {

        startRecognizeOnceAsyncButton.disabled = true;
        phraseDiv.innerHTML = "";

        var speechConfig;
        //Token alkaa
        if (authorizationToken) {
            speechConfig = SpeechSDK.SpeechConfig.fromAuthorizationToken(authorizationToken, serviceRegion);

        } else {
            if (subscriptionKey.value === "" || subscriptionKey.value === "subscription") {
                alert("Please enter your Microsoft Cognitive Services Speech subscription key!");
                return;
            }
            speechConfig = SpeechSDK.SpeechConfig.fromSubscription(subscriptionKey, serviceRegion);
        }

        speechConfig.speechRecognitionLanguage = "fi-FI";
        var audioConfig = SpeechSDK.AudioConfig.fromDefaultMicrophoneInput();
        recognizer = new SpeechSDK.SpeechRecognizer(speechConfig, audioConfig);

        recognizer.recognizeOnceAsync(
            function (result) {
                startRecognizeOnceAsyncButton.disabled = false;
                if (result.text !== undefined) {
                    phraseDiv.innerHTML += result.text;
                }
                window.console.log(result);

                recognizer.close();
                recognizer = undefined;
            },
            function (err) {
                startRecognizeOnceAsyncButton.disabled = false;
                // phraseDiv.innerHTML += err;
                window.console.log(err);
                $("#scores").append($("<div/>", {
                    "class": "warning",
                    text: "Puheen kuunteleminen ei juuri nyt onnistu. Oletko sallinut mikrofonin käytön?"
                }));
                recognizer.close();
                recognizer = undefined;
            });
    });

    if (!!window.SpeechSDK) {
        SpeechSDK = window.SpeechSDK;
        startRecognizeOnceAsyncButton.disabled = false;

        document.getElementById('content').style.display = 'block';
        document.getElementById('warning').style.display = 'none';

        // in case we have a function for getting an authorization token, call it.
        if (typeof RequestAuthorizationToken === "function") {
            RequestAuthorizationToken();
        }
    }
});

//Arpoo käyttäjälle sanat

// function GetWords() {
//     $.ajax({
//         url: apiUrl,
//         success: function (data) {
//             console.log(data)
//             $("#prompt").empty();
//             $("#prompt").append(data);
//         },
//         error: function (xhr, status, errorThrown) {
//             $("#scores").append($("<div/>", {
//                 "class": "warning",
//                 text: "Hups, sanojen hakemisessa näyttää olevan jotain pielessä, yritä uudelleen."
//             }));
//             $("#buttons").html($("<button/>", {
//                 "id": "reloadButton",
//                 text: "Yritä uudelleen",
//                 "click": function () {
//                     location.reload();
//                 }
//             }))
//         },
//         dataType: "text"
//     });
// }

//Pisteet sanoille
function GetScores() {
    var input = $("#phraseDiv").val().toLowerCase().split(" ");
    post = {
        "original": $("#prompt").text(),
        "guesses": input
    };
    console.log(post);
    $.ajax({
        type: "POST",
        url: apiUrl,
        data: JSON.stringify(post),
        contentType: "application/json; charset=UTF-8",
        success: function (data) {
            $("#scores").removeClass("hidden");
            console.dir(data);
            $("#scores").append($("<div/>", {
                "class": "original",
                text: data["original"]
            }));
            for (var i = 0; i < data["list"].length; i++) {
                $("#scores").append($("<div/>", {
                    "class": "score",
                    text: data["list"][i]["word"] + ": " + data["list"][i]["score"]
                }
                ))
            }
            var total = parseInt(data["kokonaispisteet"]);
            var sessionTotal = parseInt(sessionStorage.getItem("total"));
            sessionStorage.setItem("total", sessionTotal + total);
            var round = parseInt(sessionStorage.getItem("round")) + 1;
            sessionStorage.setItem("round", round);
            $("#totalScore").html("Pisteet: " + (sessionTotal + total));
            $("#phraseDiv").val("");
            ChangeButtons();
        },
        error: function (xhr, status, errorThrown) {
            $("#scores").append($("<div/>", {
                "class": "warning",
                text: "Hups, pisteiden laskussa meni jotain pieleen, yritä uudelleen."
            }));
            $("#buttons").html($("<button/>", {
                "id": "reloadButton",
                "class": "btn",
                text: "Yritä uudelleen",
                "click": function () {
                    location.reload();
                }
            }))
        },
        dataType: "json"
    });
}

function ChangeButtons() {
    $("#count").replaceWith();
    if (parseInt(sessionStorage.getItem("round")) > maxRounds) {
        $("#buttons").html($("<button/>", {
            "id": "saveScoreButton",
            text: "Tallenna",
            class: "open2 btn",
            "click": function () {
                $(".popup-overlay2, .popup-content2").addClass("active");
                $('.popupCloseButton2').click(function () {
                    $(".popup-overlay2, .popup-content2").removeClass("active");
                });
                $("#totalScoreForDB").append(sessionStorage.getItem("total"));
            }
        }));
        $("#buttons").append($("<button/>", {
            "id": "newGameButton",
            "class": "btn",
            text: "Pelaa uudestaan",
            click: function () {
                sessionStorage.setItem("total", 0);
                sessionStorage.setItem("round", 0);
                location.reload();
            }
        }))
    } else {
        $("#buttons").html($("<button/>", {
            "id": "nextWordButton",
            "class": "btn",
            text: "Seuraava sana",
            "click": function () {
                location.reload();
            }
        }))
    };
}

// Tietojen tallennus

function SaveScore() {
    var post = {
        "nickname": $("#nickname").val(),
        "scores": parseInt(sessionStorage.getItem("total"))
    }
    $.ajax({
        type: "POST",
        url: apiUrl + "/Tallennus",
        data: JSON.stringify(post),
        contentType: "application/json; charset=UTF-8",
        success: function () {
            $("#saveForm").empty();
            $("#saveForm").append($("<div/>", {
                text: "Pisteet tallennettu, muista tarkistaa, pääsitkö Top 10 -listalle!"
            }))
        },
        error: function () {
            $("#saveForm").empty();
            $("#saveForm").append($("<div/>", {
                text: "Jotain meni vikaan, pahoittelumme!"
            }))
        }
    })
}

// Pelin ohjeet
$(".open").on("click", function () {
    $(".popup-overlay, .popup-content").addClass("active");
});

$('.popupCloseButton').click(function () {
    $('.hover_bkgr_fricc').hide();
});
$(".close, .popup-overlay").on("click", function () {
    $(".popup-overlay, .popup-content").removeClass("active");
});

// top scores
$(".open3").on("click", function () {
    $(".popup-overlay3, .popup-content3").addClass("active");
    console.log("AJAX")
    $.ajax({
        url: apiUrl + "/Top10",
        success: function (data) {
            $("#topScores").empty();
            for (var score of data) {
                $("#topScores").append($("<tr/>", {
                    html: "<td class=\"nickname\">" + score["nickname"] + "</td><td class=\"topscore\">" + score["scores"] + "</td>"
                }))
            };
        },
        error: function (data) {
            console.dir(data)
            $("#topscores").replaceWith("Parhaita pisteitä ei juuri nyt saatu näkyville, kokeile myöhemmin uudelleen.");
        },
        dataType: "json"
    })
});

$('.popupCloseButton').click(function () {
    $('.hover_bkgr_fricc').hide();
});
$(".close, .popup-overlay3").on("click", function () {
    $(".popup-overlay3, .popup-content3").removeClass("active");
});
