function BocDateTimeValue() {
}

BocDateTimeValue.AdjustPositions = function(control) {
    var date = $(control).children(':eq(0)');
    var button = $(control).children(':eq(1)');
    var time = $(control).children(':eq(2)');

    var totalWidth = $(control).innerWidth();
    var inputWidth = totalWidth - button.outerWidth(true);
    var buttonMargin = button.outerWidth(true) - button.outerWidth(false);
    var marginLeft = parseInt(button.css('margin-left'));
    var marginRight = parseInt(button.css('margin-right'));
    if (!marginLeft)
        marginLeft = buttonMargin / 2;
    if (!marginRight)
        marginRight = buttonMargin / 2;

    var inputBordersDate = date.outerWidth(true) - date.innerWidth() + 2;
    var inputBordersTime = time.outerWidth(true) - time.innerWidth() + 2;

    var dateWidth = 0.6;
    var timeWidth = 0.4;
    if (date.length == 0)
        timeWidth = 1;

    if (time.length == 0)
        dateWidth = 1;

    date.width(Math.round(dateWidth * inputWidth) - inputBordersDate);
    time.width(Math.round(timeWidth * inputWidth) - inputBordersTime);

    button.css('left', date.width() + marginLeft);
};