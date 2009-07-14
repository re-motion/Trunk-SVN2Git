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
var _dropDownMenu_menuInfos = new Object();

var _dropDownMenu_itemClassName = 'DropDownMenuItem';
var _dropDownMenu_itemDisabledClassName = 'DropDownMenuItemDisabled';
var _dropDownMenu_itemIconClassName = 'DropDownMenuItemIcon';
var _dropDownMenu_itemSeparatorClassName = 'DropDownMenuSeparator';
var _dropDownMenu_currentMenu = null;

var _dropDownMenu_menuItemIDPrefix = 'menuItem_';

var _dropDownMenu_requiredSelectionAny = 0;
var _dropDownMenu_requiredSelectionExactlyOne = 1;
var _dropDownMenu_requiredSelectionOneOrMore = 2;

var _dropDownMenu_currentItemIndex = -1;

var _dropDownMenu_itemClickHandler = null;
var _dropDownMenu_itemClicked = false;

function DropDownMenu_MenuInfo(id, itemInfos) {
    this.ID = id;
    this.ItemInfos = itemInfos;
}

function DropDownMenu_AddMenuInfo(menuInfo) {
    _dropDownMenu_menuInfos[menuInfo.ID] = menuInfo;
}

function DropDownMenu_ItemInfo(id, category, text, icon, iconDisabled, requiredSelection, isDisabled, href, target) {
    this.ID = id;
    this.Category = category;
    this.Text = text;
    this.Icon = icon;
    this.IconDisabled = iconDisabled;
    this.RequiredSelection = requiredSelection;
    this.IsDisabled = isDisabled;
    this.Href = href;
    this.Target = target;
}

function DropDownMenu_OnClick(context, menuID, getSelectionCount, evt) {
    if (_dropDownMenu_itemClicked) {
        _dropDownMenu_itemClicked = false;
        return;
    }
    if (context != _dropDownMenu_currentMenu) {
        DropDownMenu_ClosePopUp();
    }
    if (_dropDownMenu_currentMenu == null) {
        DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, evt);
        _dropDownMenu_currentMenu = context;
    }
}

function DropDownMenu_OpenPopUp(menuID, context, getSelectionCount, evt) {
    var itemInfos = _dropDownMenu_menuInfos[menuID].ItemInfos;
    var selectionCount = -1;
    if (getSelectionCount != null)
        selectionCount = getSelectionCount();

    if (itemInfos.length == 0)
        return;

    var ul = document.createElement('ul');
    ul.className = 'DropDownMenuOptions';
    $('body')[0].appendChild(ul);

    _dropDownMenu_itemClickHandler = function() {
        DropDownMenu_ClosePopUp();
        _dropDownMenu_itemClicked = true;
        try {
            eval(this.getAttribute('javascript'));
        }
        catch (e) {
        }
        setTimeout('_dropDownMenu_itemClicked = false;', 10);
    };
    setTimeout("$('body').bind('click', DropDownMenu_ClosePopUp);", 10);

    for (var i = 0; i < itemInfos.length; i++) {
        var item = DropDownMenu_CreateItem(itemInfos[i], selectionCount, true);
        if (item != null)
            ul.appendChild(item);
    }

    // move dropdown if there is not enough space to fit it on the page
    var titleDiv = $(context).children(':first');
    var space_top = Math.round(titleDiv.offset().top - $(document).scrollTop());
    var space_bottom = Math.round($(window).height() - titleDiv.offset().top - titleDiv.height() + $(document).scrollTop());
    var space_left = titleDiv.offset().left;
    var space_right = $(window).width() - titleDiv.offset().left - titleDiv.width();

    $(ul).css('top', titleDiv.offset().top + titleDiv.outerHeight());
    $(ul).css('bottom', 'auto');
    $(ul).css('right', $(window).width() - titleDiv.offset().left - titleDiv.outerWidth());
    $(ul).css('left', 'auto');

    if (($(ul).width() > space_left) && (space_left < space_right)) {
        if ($(ul).offset().left < 0) {
            $(ul).css('left', titleDiv.offset().left);
            $(ul).css('right', 'auto');
        }
    }
    if (($(ul).height() > space_bottom) && (space_top > space_bottom)) {
        $(ul).css('top', 'auto');
        $(ul).css('bottom', $(window).height() - titleDiv.offset().top - (titleDiv.outerHeight()-titleDiv.height()));
    }

    if( ClientNeedsIFrame() )
        DropDownMenu_CreateIFrame(ul);
}

function ClientNeedsIFrame() {
    var isIE = jQuery.browser.msie;
    var version = parseFloat(jQuery.browser.version);

    return isIE && (version < 7);
}

function DropDownMenu_CreateIFrame(listElement) {
    var iFrame = document.createElement('iframe');
    iFrame.setAttribute('src', 'javascript:false');
    $(iFrame).css('position', 'absolute');
    $(iFrame).css('zIndex', '999');
    $(iFrame).css('top', $(listElement).offset().top);
    $(iFrame).css('height', $(listElement).outerHeight());
    $(iFrame).css('left', $(listElement).offset().left);
    $(iFrame).css('width', $(listElement).outerWidth());
    $(listElement).parent()[0].appendChild(iFrame);
}

function DropDownMenu_ClosePopUp() {
    if (_dropDownMenu_currentMenu == null)
        return;

    var ul = $('body').children('ul');

    $('body').unbind('click', DropDownMenu_ClosePopUp);
    _dropDownMenu_currentMenu = null;

    var iframe = ul.siblings('iframe');
    iframe.hide();

    ul.remove();

    _dropDownMenu_currentItemIndex = -1;
}

function DropDownMenu_CreateItem(itemInfo, selectionCount) {
    if (itemInfo == null)
        return null;

    var item;
    if (itemInfo.Text == '-')
        item = DropDownMenu_CreateSeparatorItem();
    else
        item = DropDownMenu_CreateTextItem(itemInfo, selectionCount);

    return item;
}

function DropDownMenu_CreateTextItem(itemInfo, selectionCount) {
    var isEnabled = DropDownMenu_GetItemEnabled(itemInfo, selectionCount);

    var item = document.createElement("li");

    var className = _dropDownMenu_itemClassName
    if (!isEnabled)
        className = _dropDownMenu_itemDisabledClassName;

    item.className = className;

    if (isEnabled)
        $(item).bind('click', _dropDownMenu_itemClickHandler);

    var anchor = document.createElement("a");
    anchor.setAttribute('href', '#');
    item.appendChild(anchor);
    if (isEnabled && itemInfo.Href != null) {
        var isJavaScript = itemInfo.Href.toLowerCase().indexOf('javascript:') >= 0;
        if (isJavaScript) {
            item.setAttribute('javascript', itemInfo.Href);
        }
        else {
            var href = itemInfo.Href;
            var target;
            if (itemInfo.Target != null && itemInfo.Target.length > 0)
                target = itemInfo.Target;
            else
                target = '_self';
            item.setAttribute('javascript', 'window.open (\'' + href + '\', \'' + target + '\');');
        }
    }

    if (itemInfo.Icon != null) {
        var icon = document.createElement('img');
        if (isEnabled || itemInfo.IconDisabled == null)
            icon.src = itemInfo.Icon;
        else
            icon.src = itemInfo.IconDisabled;

        icon.className = _dropDownMenu_itemIconClassName;
        icon.style.verticalAlign = 'middle';
        anchor.appendChild(icon);
    }
    else {
        var iconPlaceholder = document.createElement('span');
        iconPlaceholder.className = _dropDownMenu_itemIconClassName;
        anchor.appendChild(iconPlaceholder);
    }

    var text = ''
    if (itemInfo.Text != null)
        text = itemInfo.Text;

    var textNode = document.createTextNode(text);
    anchor.appendChild(textNode);

    return item;
}

function DropDownMenu_CreateSeparatorItem() {
    var item = document.createElement('li');

    var textPane = document.createElement('div');
    textPane.className = _dropDownMenu_itemSeparatorClassName;
    textPane.innerHTML = '&nbsp;';

    item.appendChild(textPane);

    return textPane;
}

function DropDownMenu_OnHeadControlClick() {
    _dropDownMenu_itemClicked = true;
}

function DropDownMenu_OnKeyDown(event, dropDownMenu, getSelectionCount) {
    // alert(event.keyCode + ', ' + dropDownMenu.nodeName + '#' + dropDownMenu.id);

    var itemInfos = _dropDownMenu_menuInfos[dropDownMenu.id].ItemInfos;
    var oldIndex = _dropDownMenu_currentItemIndex;
    var selectionCount = -1;
    if (getSelectionCount != null)
        selectionCount = getSelectionCount();

    switch (event.keyCode) {
        case 13: //enter
        case 32: //space
            if (_dropDownMenu_currentItemIndex >= 0) {
                var itemAnchor = $($(dropDownMenu).find('ul').children()[_dropDownMenu_currentItemIndex]).children('a');
                itemAnchor.click();
            }

            if (dropDownMenu != _dropDownMenu_currentMenu)
                DropDownMenu_OnClick(dropDownMenu, dropDownMenu.id, getSelectionCount, null);
            else
                DropDownMenu_ClosePopUp();
            break;
        case 27: //escape
            DropDownMenu_ClosePopUp();
            break;
        case 39: // right arrow
        case 40: // down arrow
            do {
                if (_dropDownMenu_currentItemIndex < itemInfos.length - 1)
                    _dropDownMenu_currentItemIndex++;
                else
                    _dropDownMenu_currentItemIndex = 0;
            } while (!DropDownMenu_IsSelectableItem(itemInfos, _dropDownMenu_currentItemIndex, selectionCount))
            break;
        case 37: // left arrow
        case 38: // up arrow
            do {
                if (_dropDownMenu_currentItemIndex > 0)
                    _dropDownMenu_currentItemIndex--;
                else
                    _dropDownMenu_currentItemIndex = itemInfos.length - 1;
            } while (!DropDownMenu_IsSelectableItem(itemInfos, _dropDownMenu_currentItemIndex, selectionCount))
            break;
    }
    if (0 <= _dropDownMenu_currentItemIndex && _dropDownMenu_currentItemIndex < itemInfos.length) {
        ul = $(dropDownMenu).find('ul');
        if (ul.length > 0) {
            var liGainSelected = ul.children()[_dropDownMenu_currentItemIndex];
            liGainSelected.className += " selected";

            if (oldIndex >= 0) {
                var liLoseSelected = ul.children()[oldIndex];
                liLoseSelected.className = _dropDownMenu_itemClassName;
            }
        }
    }
}

function DropDownMenu_IsSelectableItem(itemInfos, index, selectionCount) {
    var isSeparator = (itemInfos[_dropDownMenu_currentItemIndex].ID == -1);

    return !isSeparator && DropDownMenu_GetItemEnabled(itemInfos[index], selectionCount);
}

function DropDownMenu_GetItemEnabled(itemInfo, selectionCount) {
    var isEnabled = true;
    if (itemInfo.IsDisabled) {
        isEnabled = false;
    }
    else {
        if (itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionExactlyOne
        && selectionCount != 1) {
            isEnabled = false;
        }
        if (itemInfo.RequiredSelection == _dropDownMenu_requiredSelectionOneOrMore
        && selectionCount < 1) {
            isEnabled = false;
        }
    }
    return isEnabled;
}