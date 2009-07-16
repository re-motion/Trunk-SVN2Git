function BocDateTimeValue() {
}

BocDateTimeValue.AdjustPositions = function(control) {
    var pixelsPerCharacter = 7;

    var date = control.children(':eq(0)');
    var button = control.children(':eq(1)');
    var time = control.children(':eq(2)');

    var dateMinWidth = parseInt(date.attr('maxLength')) * pixelsPerCharacter;
    var timeMinWidth = parseInt(time.attr('maxLength')) * pixelsPerCharacter;

    var totalWidth = control.innerWidth();

    var buttonMargin = button.outerWidth(true) - button.outerWidth(false);
    var marginLeft = parseInt(button.css('margin-left'));
    var marginRight = parseInt(button.css('margin-right'));
    if (!marginLeft)
        marginLeft = buttonMargin / 2;
    if (!marginRight)
        marginRight = buttonMargin / 2;

    var inputBordersDate = date.outerWidth(false) - date.innerWidth() + 2;
    var inputBordersTime = time.outerWidth(false) - time.innerWidth() + 2;

    var dateWidthPart = 0.6;
    var timeWidthPart = 0.4;
    if (date.length == 0) {
        timeWidthPart = 1;
        marginLeft = 0;
    }

    if (time.length == 0) {
        dateWidthPart = 1;
        marginRight = 0;
    }

    var inputWidth = totalWidth - button.outerWidth(false) - marginLeft - marginRight;
    var dateWidth = Math.round(dateWidthPart * inputWidth) - inputBordersDate;
    var timeWidth = Math.round(timeWidthPart * inputWidth) - inputBordersTime;

    var parentWidthIncrease = 0;
    if (date.length == 1 && dateWidth < dateMinWidth) {
        parentWidthIncrease += (dateMinWidth - dateWidth);
        dateWidth = dateMinWidth;
    }
    if (time.length == 1 && timeWidth < timeMinWidth) {
        parentWidthIncrease += (timeMinWidth - timeWidth);
        timeWidth = timeMinWidth;
    }

    if (parentWidthIncrease > 0)
        control.width(control.width() + parentWidthIncrease);
    date.width(dateWidth);
    time.width(timeWidth);

    button.css('left', date.outerWidth());
    button.css('zIndex', '2');
};