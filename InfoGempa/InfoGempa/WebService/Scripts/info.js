"use strict";

var chat = $.connection.chatHub;

chat.client.addNewMessageToPage = function (name, message) {
    // Add the message to the page. 
    $('#discussion').append('<li><strong>' + htmlEncode(name)
        + '</strong>: ' + htmlEncode(message) + '</li>');
};

$.connection.hub.start().done(function () {
    $('#sendmessage').click(function () {
        // Call the Send method on the hub. 
        chat.server.send($('#displayname').val(), $('#message').val());
        // Clear text box and reset focus for next comment. 
        $('#message').val('').focus();
    });
});