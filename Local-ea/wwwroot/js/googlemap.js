var markers = [];
var meId = $('meta[id="message"]').attr('me-id');
$(window).ready(function () {

    initialize();
})

function initialize() {
    
    var map;
    var bounds = new google.maps.LatLngBounds();
    //var markers=[];
    var mapOptions = {
        mapTypeId: 'roadmap'
    };

    let cookie = sessionStorage.getItem("lastCenter");
    //let cookie = Cookies.get('lastCenter');
    let c;
    let z;
    if (cookie) {
        cookie = JSON.parse(urldecode(cookie));
        c = cookie["center"];
        z = cookie["zoom"];
    }
    else {
        // default center and zoom
        c = { lat: 37.971956, lng: 23.720093 };
        z = 14;
    }

    // Display a map on the page 
    map = new google.maps.Map(document.getElementById("map_canvas"), { center: c, zoom: z }, mapOptions);
    map.setTilt(45);



    map.addListener('idle', function () {
        // Clear all previous markers.
        //for (var i = 0; i < markers.length; i++) {
        //    markers[i].setMap(null);
        //}
        //markers = [];
        //makeTheCall(map, map.getBounds(),markers);

        let c = map.getCenter();
        let z = map.getZoom();
        sessionStorage.setItem('lastCenter', JSON.stringify({ center: c, zoom: z }));
        //Cookies.set('lastCenter',{ center: c, zoom: z})
    });

    $('#SearchThisArea').on('click', function () {
        // Clear all previous markers.
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(null);
        }
        markers = [];
        makeTheCall(map, map.getBounds(), markers);
    });

    $('#presentLocation').on('click', function () {
        findMe(map);
    });

}

function makeTheCall(map, bounds, markers) {
    var ne = bounds.getNorthEast();
    var sw = bounds.getSouthWest();
    let data = {
        latTopRight: ne.lat().toFixed(6),
        lonTopRight: ne.lng().toFixed(6),
        latBotLeft: sw.lat().toFixed(6),
        lonBotLeft: sw.lng().toFixed(6)
    };
    console.log("MapFrame:");
    console.log(data);
    $.ajax({
        url: "/MapPage/SearchThisArea",
        method: "post",
        data: data,
        success: function (data) {
            RefreshMap(map, data, markers);
            RefreshLocalsList(data);
        }

    });

};
var anim;
var animsource;
function RefreshMap(map, userDb, markers) {

    // Info Window Content 
    var infoWindowContent = [];
    
    console.log("Returned users:");
    for (var i = 0; i < userDb.length; i++) {
        console.log(userDb[i]);
        animsource = userDb[i]["avatar"];
        anim = '<span id="move"><img src="\\img\\Avatars\\' + userDb[i]["avatar"] + '" class="avatar avatar-image rounded-circle img-fluid" alt="Avatar"/></span>';
        let snipet;
        if (userDb[i]["id"] == meId) {
            snipet =
                ['<div class= "info_content ">                    \
                <div style="width=20%"><h3><a href target="_blank" data-id="' + userDb[i]["id"] + '" onclick="GoToProfile(this);">' + userDb[i]["userNameStr"] + '</a></h3><p>Average rating ' + userDb[i]["overallRating"] + '(from ' + userDb[i]["receivedRatingsCount"] +' )</p></div><br><hr>     \
            <div style="20%"><img src="\\img\\Avatars\\'+ userDb[i]["avatar"] + '" class="avatar avatar-image rounded-circle img-fluid" /></div >      \
                </div > ']
        }
        //' + anim + '   ---   onclick="mvsanimation()"
        else {
            snipet =
                ['<div class= "info_content">                    \
                <div style="width=20%"><h3><a href target="_blank" data-id="' + userDb[i]["id"] + '" title="Visit Profile" onclick="GoToProfile(this);">' + userDb[i]["userNameStr"] + '</a></h3><p> Rating ' + userDb[i]["overallRating"] + '(' + userDb[i]["receivedRatingsCount"] +')</p></div><br><hr>     \
                <div id="info-avatar" style="20%"><img src="\\img\\Avatars\\'+ userDb[i]["avatar"] +'" class="avatar avatar-image rounded-circle img-fluid" /><a id="but" onclick="mvsanimation(this)" title="Send Message Now!" class= "btn btn-outline-danger btn-sm chat-with-me-map info-box"  data-avatar="' + userDb[i]["avatar"] + '" data-name="' + userDb[i]["userNameStr"] + '" data-id="' + userDb[i]["id"] + '" > Chat</a ></div >      \
                </div > ']
        }

        infoWindowContent.push(snipet);
    }

    var infoWindow = new google.maps.InfoWindow();

  
    var iconBase = '/img/application-images/';

    var icons = {
        me: {
            url: iconBase + 'me.png',
            size: new google.maps.Size(31, 85),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(25, 75),
        },
        visitor: {
            url: iconBase + 'localee.png',
            size: new google.maps.Size(31, 85),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(25, 75),
        },
        localea: {
            url: iconBase + 'localee.png',
            size: new google.maps.Size(31, 85),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(25, 75),
        },
        admin: {
            url: iconBase + 'admin.png',
            size: new google.maps.Size(31, 85),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(25, 65),
        },
        localee1: {
            url: iconBase + 'localee1.png',
            size: new google.maps.Size(31, 90),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(17, 34),
            scaledSize: new google.maps.Size(31, 90),
        }
    };

    var shape = {
        coords: [1, 1, 1, 84, 30, 84, 30, 1],
        type: 'poly'
    };
    //------------------------------------------------------------------------------------------------
    var largest = 0;
    var mark = 0;
    var loc = 0;

      for (i = 0; i <= userDb.length-1; i++) {
        if (userDb[i]["overallRating"] == null || userDb[i]["receivedRatingsCount"] == null) {
            mark = 0;
        }
        else {
            mark = userDb[i]["overallRating"] * 8 + userDb[i]["receivedRatingsCount"] * 2;
            if (mark > largest) {
                largest = mark;
                loc = userDb[i]["id"];
            }
        }
        
    }
    // Loop through our array of markers & place each one on the map
    var icon1;
    for (i = 0; i < userDb.length; i++) {
        var position = new google.maps.LatLng(parseFloat(userDb[i]["lat"]), parseFloat(userDb[i]["lon"]));

        if (userDb[i]["id"] == meId) {
            icon1 = icons.me;
        }
        else if (userDb[i]["id"] == loc) {
            icon1 = icons.localee1;
        }
        else if (userDb[i]["localee"] == "admin") {
            icon1 = icons.admin;
        }
        else if (userDb[i]["localee"] == 'localee') {
            icon1 = icons.localea;
        }
        else {
            icon1 = icons.localea;
        }

        marker = new google.maps.Marker({
            position: position,
            map: map,
            title: userDb[i]["UserNameStr"],
            icon: icon1,
            markerId: userDb[i]['id']
        });
        markers.push(marker);

        // Allow each marker to have an info window     
        google.maps.event.addListener(marker, 'click', (function (marker, i) {
            return function () {
                infoWindow.setContent(infoWindowContent[i][0]);
                infoWindow.open(map, marker);
            };
        })(marker, i));

        //google.maps.event.addListener("#but", "click", function (marker, i) {
        //    mvsanimation();
        //});

        //$('#but').click(function () {
        //    mvsanimation();
        //};

        google.maps.event.addListener(marker, 'click', (function (marker, i) {
            return function () {
                infoWindow.setContent(infoWindowContent[i][0]);
                infoWindow.open(map, marker);
            };
        })(marker, i));

        //google.maps.event.addListener(marker, 'mouseover', (function (marker, i) {
        //    return function () {
        //        infoWindow.setContent(infoWindowContent[i][0]);
        //        infoWindow.open(map, marker);
        //    };
        //})(marker, i));

        //google.maps.event.addListener(map, 'mouseout', function (marker, i) {
        //    infoWindow.close();
        //});

        google.maps.event.addListener(map, "click", function (marker, i) {
            infoWindow.close();
        });
    }

}

function RefreshLocalsList(locals) {
    $('#LocalsList').empty(); 

    if (locals.length == 0){
        let snipet = "<div style='color:lightslategray'><i>No results for this area</i></div>";
        $(snipet).appendTo("#LocalsList");      
        return;
    }


    for (let i = 0; i < locals.length; i++) {
        let meId = $("meta[id='message']").attr("me-id");
        if (locals[i]['id'] == meId){
            continue;
        }

        let template = $('#local-template').children(":first")[0].cloneNode(true);
        $(template).attr('local-id', locals[i]['id']);
        $(template).find('#local-name').text(locals[i]["userNameStr"]);
        $(template).find('#local-avatar').attr('src', "\\img\\Avatars\\" + locals[i]["avatar"]);
        $(template).find('#local-rating').text(locals[i]["overallRating"]);
        $(template).find('#local-countRatings').text(locals[i]["receivedRatingsCount"]);
        //for (var x = 0; x < locals[i].overallRating ; x++) {
        //    $(template).find('#pizzaImageRated').attr('src',"../../images/pizzaAfter.png");
        //}
        if (locals[i].overallRating >= 4.5 ) {
            $(template).find('#pizzaImageRated').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated1').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated2').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated3').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated4').attr('src', "../../images/pizzaAfter.png");


        } else if (locals[i].overallRating == 0) {
            $(template).find('#noratings').text("No ratings!");

        }
        else if (locals[i].overallRating >= 0 && locals[i].overallRating <= 1.4 ) {
            $(template).find('#pizzaImageRated').attr('src', "../../images/pizzaAfter.png");

        }
        else if (locals[i].overallRating >= 1.5 && locals[i].overallRating < 2.5) {
            $(template).find('#pizzaImageRated').attr('src', "../../images/pizzaAfter.png");
            //$(template).find('#pizzaImageRated2').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated1').attr('src', "../../images/pizzaAfter.png");
        }
        else if (locals[i].overallRating >= 2.5 && locals[i].overallRating < 3.5) {
            $(template).find('#pizzaImageRated').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated2').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated1').attr('src', "../../images/pizzaAfter.png");
        }
        else if (locals[i].overallRating >= 3.5 && locals[i].overallRating <= 4.5) {
            $(template).find('#pizzaImageRated').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated1').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated2').attr('src', "../../images/pizzaAfter.png");
            $(template).find('#pizzaImageRated3').attr('src', "../../images/pizzaAfter.png");

        }
       
        else if (locals[i].overallRating <= 1.4) {
            $(template).find('#pizzaImageRated').attr('src', "../../images/pizzaAfter.png");

        }
        else if (locals[i].overallRating ==0) {
            $(template).find('#pizzaImageRated').attr('src', "");

        }

        
       
               
        $(template).appendTo("#LocalsList");
        $(template).click(function () {
            for (let i = 0; i < markers.length; i++) {
                if (markers[i]['markerId'] == $(this).attr('local-id')) {
                    google.maps.event.trigger(markers[i], 'click');
                    break;
                }
            }
        });
    }

}

function mvsanimation(e){
    $('#moveDiv').remove();

    let x = $('#chating-with-avatar').offset().left;
    let y = $('#chating-with-avatar').offset().top;
    let xi = $('#info-avatar').offset().left;
    let yi = $('#info-avatar').offset().top;
    console.log(`From (${xi},${yi}) to (${x},${y})`);
    //var anim = '<span id="move"><img src="\\img\\Avatars\\' + userDb[i]["avatar"] + '" class="avatar avatar-image rounded-circle img-fluid" alt="Avatar"/></span>';
    let element = document.createElement('div');
    let img = $(e).prev().clone();
    element.id = 'moveDiv';
    $(element).append(img);
    element.style.position = 'absolute';
    element.style.width = '100px';
    element.style.height = '100px';
    element.style.left = xi + 'px';
    element.style.top = yi + 'px';
    element.style.zIndex = 20000;
    document.body.appendChild(element);

    let xx = document.getElementById('row');
    //document.getElementById('move').appendChild(xx);
    
    //$('#move').css('left', xi).css('top', yi);
    //$('#but').click(function () {
       // $('#move').css("display","block");
    //document.getElementById('move').style.left = xi + 'px';
    //document.getElementById('move').style.top = yi + 'px';
    //// set their position to the target position
    //// the animation is a simple css transition
    //document.getElementById('move').style.left = x + 'px';
    //document.getElementById('move').style.top = y + 'px';

    $("#moveDiv").animate({
        left: x + 8,
        top: y + 5
    }, 2000, 'swing', function () {
            $("#moveDiv").fadeOut('slow', function () {
                $("#moveDiv").remove();
            });
        });
    //$('#move').css("display", "none");
    //})
};

function findMe(map) {
    let myloc = new google.maps.Marker({
        clickable: false,
        icon: new google.maps.MarkerImage('//maps.gstatic.com/mapfiles/mobile/mobileimgs2.png',
            new google.maps.Size(22, 22),
            new google.maps.Point(0, 18),
            new google.maps.Point(11, 11)),
        shadow: null,
        zIndex: 999,
        map: map
    });


    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };

            myloc.setPosition(pos);
            map.setZoom(16);
            map.setCenter(pos);
        }, function () {
            handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        handleLocationError(false, infoWindow, map.getCenter());

    }

    function handleLocationError(browserHasGeolocation, infoWindow, pos) {
        infoWindow.setPosition(pos);
        infoWindow.setContent(browserHasGeolocation ?
            'Error: The Geolocation service failed.' :
            'Error: Your browser doesn\'t support geolocation.');
        infoWindow.open(map);
    }
}

function urldecode(str) {
    return decodeURIComponent((str + '').replace(/\+/g, '%20'));
}


function GoToProfile(item){
    let userId = $(item).attr('data-id');
    let meId = $('meta[id="message"]').attr('me-id');
    if (userId==meId){
        window.open('../Manage/Index');
     }
    else {
        window.open('../Manage/VisitorView/?id='+ userId);

    }
}
