
$(document).ready(function () {
    var selectedPizza;
    $('#send-pizza').on('click', function () {
        

        var pizzaText = $('#pizzatext').val();
        let meId = $("meta[id='message']").attr("me-id");
        let youId = $("meta[id='message']").attr("you-id");


        if (selectedPizza) {
            $.ajax(
                {
                    type: "POST",
                    url: '/MapPage/SetRating',
                    data: {
                        fromId: meId,
                        toId: youId,
                        ratingText: pizzaText,
                        ratingValue: Number(selectedPizza.substr(selectedPizza.length - 1))
                    },
                    success: function () {
                     document.getElementById("myNav").style.width = "0%";

                    }
                    // dataType: dataType
                }
            )
        }
    })
    $('#pizza1').on('click', pizzaClicked);
    $('#pizza2').on('click', pizzaClicked);
    $('#pizza3').on('click', pizzaClicked);
    $('#pizza4').on('click', pizzaClicked);
    $('#pizza5').on('click', pizzaClicked);


    function pizzaClicked(e) {
        selectedPizza = e.target.id;
        $('#' + e.target.id).nextUntil().removeClass('checked');
        $('#' + e.target.id).prevUntil().andSelf().addClass('checked');
    };
});
function openNav() {
    document.getElementById("myNav").style.width = "100%";
    let meId = $("meta[id='message']").attr("me-id");
    let youId = $("meta[id='message']").attr("you-id");


    $.ajax({
        type: "GET",
        url: '/MapPage/UserHasRated',
        data: {
            fromId: meId,
            toId: youId
        },
        success: function (data) {
            $('#RatingFash').html(data);
        }
    })
}

function closeNav() {
    document.getElementById("myNav").style.width = "0%";
}