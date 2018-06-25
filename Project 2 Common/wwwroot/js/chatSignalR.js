$(window).ready(function(){
    let messageArea = $('#message-to-send');
    messageArea.on("keyup", function(event) {
        if (event.keyCode === 13 && !event.shiftKey) {
            event.preventDefault();
            $('#sendButton').click();
        }
    });


    //initialize (global) properties
    let me={};
    me.id =  $("meta[id='message']").attr("me-id");
    me.name = $("meta[id='message']").attr("me-name");
    let you = {};

    // clicking text area mark messages as read
    $('#message-to-send').on('click', function(){
        if (you.id){
            // clear notification
            let buddy = $('#chat-buddies').find(`[data-id="${you.id}"]`)[0];
            $(buddy).find('#buddy-avatar').stop(true); 
            $(buddy).find('#buddy-avatar').css({ 'border': '0px solid' });
            $(buddy).find('#unread').val('0');
            $(buddy).find('#unread').hide().text('');    

            //mark conversation as read
            $.ajax({
            url: "/MapPage/MarkConversationRead",
            method: "post",
            data: {
                fromId: you.id,
                toId: me.id
                }
            });
        }
    });
    

    // when selecting user to chat with form map
    $('body').on('click','.chat-with-me-map',function(){
        // Store receiver info
        $("meta[id='message']").attr("you-id",$(this).attr('data-id'));
        $("meta[id='message']").attr("you-name",$(this).attr('data-name'));
        you.id = $(this).attr('data-id');
        you.name = $(this).attr('data-name');
        you.avatar = $(this).attr('data-avatar');


        //Check if in chatBuddies
        let buddy = $('#chat-buddies').find(`[data-id="${you.id}"]`)[0];
        if (!buddy){
            // add chat buddy block
            PrependChatBuddy(you);
            buddy = $('#chat-buddies').find(`[data-id="${you.id}"]`)[0];
          }     

        $(buddy).trigger('click');
        
        
    });
    

    // When selecting user to chat with from chatBuddies)
    //
    $('body').on('click','.chat-with-me',function(){
        $('#message-area').css('visibility', 'visible');
        $('#message-area').css('background-color', '#F2F5F8');
   

        //Clear chat box
        $('#chat-dynamic').empty();


        // Store receiver info
        $("meta[id='message']").attr("you-id",$(this).attr('data-id'));
        $("meta[id='message']").attr("you-name",$(this).attr('data-name'));
        you.id = $(this).attr('data-id');
        you.name = $(this).attr('data-name');
        you.avatar = $(this).attr('data-avatar');



        // Check if chat history has loaded
        if (!sessionStorage.getItem(you.id)){
            $.when(loadPersonalChatHistory(me.id,you.id)).done(function(){
                PrintChatHistory(JSON.parse(sessionStorage.getItem(you.id)),me.id); 
            });
        } else {
            PrintChatHistory(JSON.parse(sessionStorage.getItem(you.id)),me.id); 
        }


        // Make call to mark messages as read
        $.ajax({
            url: "/MapPage/MarkConversationRead",
            method: "post",
            data: {
                fromId: you.id,
                toId: me.id
                }
            });
        

        // if unread messages clear notification color
        $(this).find('#buddy-avatar').stop(true); 
        $(this).find('#buddy-avatar').css({ 'border': '0px solid' });
        $(this).find('#unread').val('0');
        $(this).find('#unread').hide().text('');


        // Display username and avatar on chat header
        $('#chating-with-name').find('a').text(you.name);    
        $('#chating-with-name').find('a').attr('href', "/Manage/VisitorView/?id=" + you.id);
        $('#chating-with-avatar').attr('src',"\\img\\Avatars\\" + you.avatar);


        // make message textarea active
        $('#message-to-send').prop('disabled', false);

    });
    // Establish chathub connection
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();


    // Handle Incoming messages
    connection.on("ReceiveMessage", (message) => {

        // check if user in chatbudies
        let buddy = $('#chat-buddies').find(`[data-id="${message['senderId']}"]`)[0];
        if (!buddy) {
            // fetch user info from db
            $.ajax({
                url: "/MapPage/GetUserInfo",
                method: "post",
                data: { id: message["senderId"] },
                success: function (data) {
                    let userModel = {
                        id: data['id'],
                        name: data['userNameStr'],
                        avatar: data['avatar']
                    };
                    // add chat buddy block
                    PrependChatBuddy(userModel);
                    buddy = $('#chat-buddies').find(`[data-id="${message['senderId']}"]`)[0];
                    doStuff(buddy);
                }
            });
        } else {
            doStuff(buddy);
        }


        function doStuff(buddy){
            // move box to top
            MoveChatBuddyTop(buddy);

            // mark it for notification
            $(buddy).find('#buddy-avatar').ready(function () {
                function loop(buddy) {
                    $(buddy).find('#buddy-avatar').css({ 'border': '4px solid cornflowerblue' });
                    $(buddy).find('#buddy-avatar').animate({
                        'borderWidth': '0px',
                    },1000, function () {
                        loop(buddy);
                    });
                }

                loop(buddy);})
             // Configure notification style for new message
            // mark unread messages
            $(buddy).find('#unread').show().val(1+Number($(buddy).find('#unread').val()));
            $(buddy).find('#unread').text(`(${$(buddy).find('#unread').val()})`);
            

            //check if that chat is focused
            if (you.id == message['senderId']){
                 //if yes it is implied history is already loaded and printed

                //write to history
                if (!AlreadyInStorage(message)){
                    WriteToStorage(false, message);
                }

                //print message
                PrintMessage(false, message);
            }
            else{
                // if not just write to storage

                // if history is not loaded, load it and then write message to storage
                if (!sessionStorage.getItem(message["senderId"])){
                    $.when(loadPersonalChatHistory(me.id, message["senderId"])).done(function(){
                        if (!AlreadyInStorage(message)){
                            WriteToStorage(false, message);
                        } 
                    });
                } else {
                    if (!AlreadyInStorage(message)){
                        WriteToStorage(false, message);
                    }  
                }
            }
        }
    });


    // Outgoing message
    $('#sendButton').on("click", event => {
        if($("#message-to-send").val()){

            // Message object to send to server
            let message = {
                senderId: me.id,
                senderName: me.name,
                receiverId: you.id,
                receiverName: you.name,
                body: $("#message-to-send").val(),
                date : new Date(Date.now())
            }

            //Clear message field
            $("#message-to-send").val("");

            PrintMessage(true, message)

            // Add message to personalChatHisotry in storage
            WriteToStorage(true, message)

             // move box to top
            let buddyBox = $('#chat-buddies').find(`[data-id="${message['receiverId']}"]`)[0];
            MoveChatBuddyTop(buddyBox);

            // Send message to chatHub
            connection.invoke("SendMessage", message).catch(err => console.error(err.toString()));
        }
        event.preventDefault();
    });

    // Open chathub connection
    connection.start().catch(err => console.error(err.toString()));
});


// Tools ------------------------------------------------------------------------------------------

function PrintMessage(amItheSender, message){
    let bubble;
    if (amItheSender){
        bubble = $('#message-template').children(":first")[0].cloneNode(true);
    }
    else{
        bubble = $('#message-response-template').children(":first")[0].cloneNode(true);
    }
    $(bubble).find('#SenderName').text(message['senderName']);
    $(bubble).find('#date').text(new Date(message['date']).toLocaleString());
    $(bubble).find('#message-text').text(message['body']);

    $(bubble).appendTo("#chat-dynamic");

    $(bubble).scrollTop($(bubble)[0].scrollIntoView);

}


function loadPersonalChatHistory(meId,youId)
{   
    console.log("loading history..");
    return $.ajax({
        url: "/MapPage/GetHistoryBetweenAsync",
        method: "post",
        data: {
            id1: meId,
            id2: youId
            },
        success: function(history){
            console.log(history);
            sessionStorage.setItem(youId, JSON.stringify(history));
        }
    });
}

function PrintChatHistory(messages,meId){
        // Toggle review button
    if (messages.length ==0) {
        return;
    }
        let initiator = messages[0]["senderId"];
        if (meId == initiator)
        {
            $('#buttonCrown').css('display','');
        }else
        {
            $('#buttonCrown').css('display','none');
        }
    
    for (let i=0;i<messages.length;i++)
    {
        if(meId == messages[i]["senderId"]){
            PrintMessage(true, messages[i]);
        }
        else
        {
            PrintMessage(false, messages[i]);
        }
    } 
}

function WriteToStorage(amItheSender,message){
    let id = (amItheSender)?(message["receiverId"]):(message["senderId"])
    let parsedHisotry = JSON.parse(sessionStorage.getItem(id));
    parsedHisotry.push(message);
    sessionStorage.setItem(id, JSON.stringify(parsedHisotry));
}

function PrependChatBuddy(userModel){
    let bubble = $('#chatBuddy-template').children(":first")[0].cloneNode(true);

    $(bubble).attr('data-id',userModel.id);
    $(bubble).attr('data-name',userModel.name);
    $(bubble).attr('data-avatar',userModel.avatar);

    $(bubble).find('#buddy-avatar').attr('src',"\\img\\Avatars\\" + userModel.avatar);
    $(bubble).find('#buddy-name').text(userModel.name);

    $(bubble).prependTo("#chat-buddies");
}

function MoveChatBuddyTop(buddyBox){
    $(buddyBox).detach();
    $(buddyBox).prependTo("#chat-buddies");
};

function AlreadyInStorage(message){
    let parsedHisotry = JSON.parse(sessionStorage.getItem(message['senderId']));
    for (let i = 0; i < parsedHisotry.length; i++) {
        if (parsedHisotry[i]['messageId'] == message['messageId']) {
            return true;
        }
    }
    return false;
}
