// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

function BocDateTimeValue() {
}

BocDateTimeValue.AdjustPositions = function(control) {
    console.log(control.attr('id'));
    var pixelsPerCharacter = 7;

    var date = control.children(':eq(0)');
    var button = control.children(':eq(1)');
    var time = control.children(':eq(2)');

    var dateMinWidth = parseInt(date.attr('maxLength')) * pixelsPerCharacter;
    var timeMinWidth = parseInt(time.attr('maxLength')) * pixelsPerCharacter;

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

    var totalWidth = control.innerWidth();
    var controlWidthBefore = control.width();
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

    var cell = control.parents('td.bocListDataCellOdd, td.bocListDataCellEven');
    var cellWidth = cell.width();
    console.log('cell width before: ' + cellWidth + ', controlWidthBefore: ' + controlWidthBefore + ', parentWidthIncrease: ' + parentWidthIncrease);
    control.width(controlWidthBefore + parentWidthIncrease);

    console.log('cell width after: ' + cellWidth + ', parentWidthIncrease: ' + parentWidthIncrease);
    cell.width(cellWidth);

    date.width(dateWidth);
    time.width(timeWidth);

    var dateOuterWidth = date.outerWidth();
    button.css('left', dateOuterWidth);
    button.css('zIndex', '2');
};