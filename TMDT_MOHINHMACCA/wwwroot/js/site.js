$(function () {
    $("#btn-show-emojis").click(function () {
        $("#emojis-container").toggleClass("d-none");
    });
    $('#emojis-container').on('click', 'button', function () {
        var emojiValue = $(this).data("value");
        var input = $('#message-input');
        input.val(input.val() + emojiValue + " ");
        input.focus();
        input.change();
    });
    $("#message-input, .messages-container, #btn-send-message, #emojis-container button").click(function () {
        $("#emojis-container").addClass("d-none");
    });
});