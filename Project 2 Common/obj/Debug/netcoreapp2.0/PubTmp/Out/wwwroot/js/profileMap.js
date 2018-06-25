$(window).ready(function () {
    initialize();
});

function initialize() {
    let map;
    var infoWindow = new google.maps.InfoWindow();
    var markers = [];
    var mapOptions = {
        mapTypeId: 'roadmap'
    };

     

    let c; // center
    let z; // zoom
    // Check if a position has been provided.
    if ($('#lon').val()!=0 && $('#lat').val()!=0){
        c = new google.maps.LatLng($('#lat').val(),$('#lon').val());
        z = 16
    }
    // if not, center Acropolis, call geolocation findme. drop pin.
    else{
        c = new google.maps.LatLng(37.971956,23.720093);
        z=14;

        //Find my position and drop pin
        //GetPosition(map)
        
    }


    // Display a map on the page 
    map = new google.maps.Map(document.getElementById("map_canvas"), { center: c, zoom: z }, mapOptions);
    map.setTilt(45);

    // Create MyLocation Item
    var myloc = new google.maps.Marker({
        clickable: true,
        draggable: true,
        animation: google.maps.Animation.DROP,
        icon: new google.maps.MarkerImage('//maps.gstatic.com/mapfiles/mobile/mobileimgs2.png',
            new google.maps.Size(22, 22),
            new google.maps.Point(0, 18),
            new google.maps.Point(11, 11)),
        shadow: null,
        zIndex: 999,
        map: map
    });
    myloc.setPosition(c);
    geocodePosition(myloc.getPosition());


    $('#presentLocation').on('click', function (e) {
        e.preventDefault();
        GetPosition(map)
    });


    myloc.addListener('dragend', function () {
        geocodePosition(myloc.getPosition());
        infoWindow.close();
    });

    
    function geocodePosition(pos) {
        geocoder = new google.maps.Geocoder();
        geocoder.geocode
            ({
                latLng: pos
            },
            function (results, status) {
                if (status === google.maps.GeocoderStatus.OK) {
                    dLat = results[0].geometry.location.lat().toFixed(6).toString();
                    dLon = results[0].geometry.location.lng().toFixed(6).toString();
                    $("#myLocOutput").val(results[0].formatted_address);
                    $("#mapErrorMsg2").hide(100);
                    $("#myLocOutput").html(results[0].formatted_address + ' - ' + status).show(100);
                    $("#lon").val(dLon);
                    $("#lat").val(dLat);
                }
                else if (results[0].geometry.location.lat() === undefined) {
                    dLat = pos.dLat;
                    dLon = pos.dLon;
                    $("#mapErrorMsg2").hide(100);
                    $("#lon").val(dLon);
                    $("#lat").val(dLat);

                    $("#mapErrorMsg2").hide(100);
                    $("#myLocOutput").html('I assume you are in Acropolis, Athens' + status + lat + lon).show(100);
                }
                else {
                    dLat = pos.dLat;
                    dLon = pos.dLon;
                    //var dLat = 37.9718249;
                    //var dLon = 23.7264661;
                    $("#mapErrorMsg2").hide(100);
                    $("#lon").val(dLon);
                    $("#lat").val(dLat);

                    $("#mapErrorMsg2").hide(100);
                    $("#myLocOutput").html('Cannot determine address at this location.' + status + lat + lon).show(100);
                }
            }
        );
    }

    function GetPosition(map) {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var pos = {
                    lat: position.coords.latitude,
                    lng: position.coords.longitude
                };

                myloc.setPosition(pos);
                map.setZoom(16);
                map.setCenter(pos);

                geocodePosition(myloc.getPosition(pos));



            }, function () {
                handleLocationError(true, infoWindow, map.getCenter());
                pos = {
                    lat: 37.9718249,
                    lng: 23.7264661
                };
                myloc.setPosition(pos);
                map.setZoom(10);
                map.setCenter(pos);
                $("#myLocOutput").html('Cannot determine address at this location.' + status + lat + lon).show(100);
                geocodePosition(pos);
            });
        } else {
            // Browser doesn't support Geolocation
            handleLocationError(false, infoWindow, map.getCenter());

        }
        function handleLocationError(browserHasGeolocation, infoWindow, pos) {
            infoWindow.setPosition(pos);
            infoWindow.setContent(browserHasGeolocation ?
                'Error: The Geolocation service failed.' + '<br>' + 'Change browser settings and REFRESH page!' :
                'Error: Your browser doesn\'t support geolocation.');
            infoWindow.open(map, myloc);
        }
    }

}
           