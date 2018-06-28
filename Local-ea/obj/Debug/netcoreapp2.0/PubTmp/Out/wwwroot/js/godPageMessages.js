
$(window).ready(function(){
    let id1 =  $("#messagesList").attr("data-id1");
    let id2;

    // When selecting user to chat with from chatBuddies)
    //
    $('body').on('click','.chat-with-me',function(){

        //Clear chat box
        $('#chat-dynamic').empty();


        // Store receiver info
        id2 = $(this).attr('data-id');
        $("#messagesList").attr("data-id2",id2);

        loadPersonalChatHistory(id1,id2);
    });
});


// Tools ------------------------------------------------------------------------------------------

function loadPersonalChatHistory(id1,id2)
{   
    console.log("loading history..");
    return $.ajax({
        url: "/MapPage/GetHistoryBetweenAsync",
        method: "post",
        data: {
            id1: id1,
            id2: id2
            },
        success: function(history){
            console.log(history);
            PrintChatHistory(history, id1);
        }
    });
}

function PrintChatHistory(messages,id1){
    for (let i=0;i<messages.length;i++)
    {
        if(id1 == messages[i]["senderId"]){
            PrintMessage(true, messages[i]);
        }
        else
        {
            PrintMessage(false, messages[i]);
        }
    } 
}

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
    bubble.scrollTop = bubble.scrollHeight;

    $(bubble).scrollTop($(bubble)[0].scrollIntoView);
    
}