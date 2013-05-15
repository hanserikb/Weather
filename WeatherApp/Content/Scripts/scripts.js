$(function () {
    console.log('test');
    var myOptions = {
        zoom: 2,
        center: new google.maps.LatLng(0, 0),
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        disableDefaultUI: true,
        draggable: false,
        disableDoubleClickZoom: true,
        scrollwheel: false

    };

    var map = new google.maps.Map(document.getElementById('map_canvas'), myOptions);


    renderRecentSearchList();
});


var bindBookmarkLink = function () {
    $("#addBookmark").on('click', function (e) {
        e.preventDefault();
        if (Modernizr.localstorage) {
            var country = $(this).attr('data-country');
            var county = $(this).attr('data-county');
            var name = $(this).attr('data-locationname');

            var href = country + "/" + county + "/" + name;

            var bookmarks = [];

            // Hämtar tidigare bokmärken om dem finns
            if (localStorage.getItem('bookmarks') !== null) {
                bookmarks = JSON.parse(localStorage.getItem('bookmarks'));
            }

            // Om elementet redan finns i arrayen, ta bort den
            var bookmarkIndex = $.inArray(href, bookmarks);

            // Om arrayen är längre 10 och sökningen inte redan finns i arrayen, ta bort första elementet
            if (bookmarkIndex >= 0) {
                bookmarks.splice(bookmarkIndex, 1);
            }
            console.log(bookmarks);

            // Lägg till sökvägen i arrayen
            bookmarks.unshift(href);

            // Sätt arrayen till localstorage
            localStorage.setItem('bookmarks', JSON.stringify(bookmarks));

            // Rendera bokmärkslistan på nytt
            renderBookmarkList();
        };
    });
};
var renderBookmarkList = function () {

    // jättefullösning för att hitta roten till sidan
    var root = $("form#form0").attr("data-ajax-url");


    // Sparar referens till listan
    var bookmarkList = $("#bookmarks ul");

    // Tömmer listan
    bookmarkList.empty();

    // Hoppar ur om det inte finns några bokmärken
    if (localStorage.getItem('bookmarks') === null) {
        return false;
    }

    var bookmarks = JSON.parse(localStorage.getItem('bookmarks'));

    // Itererar genom arrayen, skriver ut en länk i listan.
    $.each(bookmarks, function (index, bookmarkHref) {
        var bookmarkListItem = $("<li></li>");
        var bookmarkLink = $("<a class='bookmarkLink' data-ajax='true' data-ajax-mode='replace' data-ajax-success='renderMapMarkers' data-ajax-update='#location' href='" + root +"Forecast/" + bookmarkHref + "'>" + bookmarkHref + "</a>");
        var removeLink = $("<a href='#' class='removeBookmark label' title='Remove bookmark'>X</a>").on('click', function (event) {
            var bookmarkIndex = $.inArray(bookmarkHref, bookmarks);
            removeBookmark(bookmarkIndex);
            renderBookmarkList();
            event.preventDefault();
        });
        bookmarkListItem.append(removeLink).append(bookmarkLink);
        bookmarkList.append(bookmarkListItem);
    });
};

var removeBookmark = function (index) {
    var bookmarks = JSON.parse(localStorage.getItem('bookmarks'));
    bookmarks.splice(index, 1);
    if (bookmarks.length === 0) {
        localStorage.removeItem('bookmarks');
    } else {
        localStorage.setItem('bookmarks', bookmarks);
    }
    renderBookmarkList();
};

/*
* Rensar bokmärken ifrån localstorage samt tömmer lista
*/
var clearBookmarks = function () {
    if (localStorage.getItem('bookmarks') !== null) {
        localStorage.removeItem("bookmarks");
        $("#bookmarks ul").empty();
    }
}

var geoLocation = function () {

    // jättefullösning för att hitta roten till sidan
    var root = $("form#form0").attr("data-ajax-url");

    $("#getGeoLocation").on('click', function () {
        
        if (navigator.geolocation) {
            showSpinner();
            navigator.geolocation.getCurrentPosition(function (position) {
                
                $.post(root + 'Geo', { Lat: position.coords.latitude, Lng: position.coords.longitude }, function (data) {
                    hideSpinner();
                    $("#location").empty();
                    $("#location").append(data);
                });
            }, function (error) {
                hideSpinner();
                renderModal('Error', 'An error occurred while getting your position. Try again later');
            });
            
        } else {
            renderModal('Notice', 'It seems like Geolocation, which is required for this page, is not enabled in your browser. Please use a browser which supports it.');
        }
    });

}



var renderModal = function (header, message) {
    $('#modal h3').html(header);
    $('#modal p').html(message);
    $('#modal').modal('toggle');
}

var bindToolTips = function () {
    $("div.weatherIcon").hover(function () {
        $(this).parent().find(".extraData").fadeIn();
    }, function () {
        $(this).parent().find(".extraData").fadeOut();
    })
};


var showSpinner = function () { 
    // jättefullösning för att hitta roten till sidan
    var root = $("form#form0").attr("data-ajax-url");
      
    var html = $("<div id='spinner' class='spinner' style='text-align: center'><img id='img-spinner' src='"+root+"content/img/spinner.gif' alt='Loading'/><span>Loading..</span></div>");
    $("body").append(html);
}

var hideSpinner = function(){
    $("#spinner").remove();
}

/*
         * Skriver ut kartmarkeringar
         */
        var renderMapMarkers = function () {
            handleAjaxSuccess();

            // Om inga länkar hittats hoppa ur funktionen
            if($("a.weatherLink").length === 0) { return false; }

            // Inställningar till Google Maps
            var myOptions = {
                disableDefaultUI: true,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                scrollwheel: false,
                disableDoubleClickZoom: true,
            };

            var map = new google.maps.Map(document.getElementById('map_canvas'), myOptions);
            var bounds = new google.maps.LatLngBounds();
            
            // Binder en funktion som körs för varje resultatlänk som visas
            $("a.weatherLink").each(function (index, value) {

                // Sparar undan en referens till länken samt dess position
                var that = $(this);
                var lng = that.attr('data-lng');
                var lat = that.attr('data-lat');

                // Klonar länken så jag kan lägga den i markörens infoWindow
                var newLink = that.clone().html('Go to forecast!');

                // Delar upp länkens href-attribut så man kan får ut parametrar för att söka på prognoser
                var hrefArr = that.attr('href').split('/');
                console.log(that);
                var country = hrefArr[hrefArr.length-3];
                var county = hrefArr[hrefArr.length-2];
                var location = hrefArr[hrefArr.length-1];
                console.log(county);
                // Skapar ett nytt positionsobjekt till Google Maps.
                var myLatLng = new google.maps.LatLng(lat, lng);

                // Lägger till positionen i bounds
                bounds.extend(myLatLng);

                // Skapar en markör till kartan med tillhörande position
                var marker = new google.maps.Marker({
                    position: myLatLng,
                    map: map
                });

                // När länken klickas, manipulera adressfältet samt lägg till data i localstorage
                
                // jättefullösning för att hitta roten till sidan
                var root = $("form#form0").attr("data-ajax-url");

                that.click(function () {
                    window.history.pushState(root +  "/Forecast/" + country + "/" + county + "/" + location);
                });

                // Bygger en sträng till markörens infoWindow
                var contentString = 'Country: ' + country + '<br /> County: ' + county + '<br />City: ' + location + '<br /><div id="links"></div>' + newLink[0].outerHTML;

                // Skapar infoWindowObjekt
                var infowindow = new google.maps.InfoWindow({
                    content: contentString
                });

                // Binder en click-funktion till markören, som öppnar tillhörande infowindow
                google.maps.event.addListener(marker, 'click', function () {
                    infowindow.open(map, marker);
                });

                // Om man hovrar över länken, panoreras kartan till den positionen
                that.hover(function () {
                    map.panTo(new google.maps.LatLng(lat, lng));
                    map.setZoom(8);
                    marker.setAnimation(google.maps.Animation.BOUNCE);
                }, function () {
                    map.fitBounds(bounds)
                    marker.setAnimation(null);
                });
            });
            map.fitBounds(bounds);
        }