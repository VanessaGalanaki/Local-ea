$(window).ready(function(){

    // prevent notification appearing in map page
    let curentURL = window.location.pathname
    if (curentURL != "/MapPage" && curentURL != "/MapPage/Index"){

        // clear latestSenderName every 5 seconds so after a while the same name will apear as notification.
        let clearSender = window.setInterval(function(){
            window.latestSenderName = "";
        }, 5000);


        // When closing page remove setInterval
        $(window).bind('beforeunload', function(){
            window.clearInterval(clearSender);
            window.latestSenderName = null;
        });


        // Establish chathub connection
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub")
            .build();

        connection.on("Notify", (senderName,avatar) => {
            // Check if latest cotification was the same user so we don't spam currentUser with notifications
            if (senderName != window.latestSenderName){
                
                $.notify({
                        icon: `\\img\\mnm.png`,
                        title:`<strong>New message</strong>`,
                        message: ` by ${senderName}`,
                        url: `/MapPage/Index`,
                        target: "_self",
                        
                }, {
                        placement: {
                            from: "bottom",
                            align: "right"
                        },
                        animate: {
                            enter: 'animated lightSpeedIn',
                            exit: 'animated lightSpeedOut'
                        },
                        icon_type: 'image',
                        template: '<div data-notify="container" class="col-xs-11 col-sm-3 alert alert-{0}" role="alert">' +
                            '<img data-notify="icon" class="img-circle pull-left">' +
                            '<span data-notify="title">{1}</span>' +
                            '<span data-notify="message">{2}</span>' +
                            '<a href="{3}" target="{4}" data-notify="url"></a>' +
                            '</div>'
                        
                    });
               
                window.latestSenderName = senderName;
            }
        });

        // Open chathub connection
        connection.start().catch(err => console.error(err.toString()));
    }
});
