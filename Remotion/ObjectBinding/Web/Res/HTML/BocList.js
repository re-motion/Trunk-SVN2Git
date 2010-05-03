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
//  BocListe.js contains client side scripts used by BocList.

//  The css classes used for rows in their selected and unselected state.
var _bocList_TrClassName = '';
var _bocList_TrClassNameSelected = '';

//  Associative array: <BocList ID>, <BocList_SelectedRows>
var _bocList_selectedRows = new Object();

//  A flag that indicates that the OnClick event for a selection selectorControl has been raised
//  prior to the row's OnClick event.
var _bocList_isSelectorControlClick = false;
  
//  A flag that indicates that the OnClick event for an anchor tag (command) has been raised
//  prior to the row's OnClick event.
var _bocList_isCommandClick = false;

//  A flag that indicates that the OnClick event for a selectorControl label has been raised
//  prior to the row's OnClick event.
var _bocList_isSelectorControlLabelClick = false;

var _bocList_rowSelectionUndefined = -1;
var _bocList_rowSelectionDisabled = 0;
var _bocList_rowSelectionSingleCheckBox = 1;
var _bocList_rowSelectionSingleRadioButton = 2;
var _bocList_rowSelectionMultiple = 3;

function BocList_SelectedRows (selection)
{
  this.Selection = selection;
  //  Associative Array: <SelectorControl ID>, <BocList_RowBlock>
  this.Length = 0;
  this.Rows = new Object();
  this.Clear = function()
  {
    this.Length = 0;
    this.Rows = new Object();
  }
}

function BocList_RowBlock (row, selectorControl)
{
  this.Row = row;
  this.SelectorControl = selectorControl;
}

//  Initializes the class names of the css classes used to format the table cells.
//  Call this method once in a startup script.
function BocList_InitializeGlobals (trClassName, trClassNameSelected)
{
  _bocList_TrClassName = trClassName;
  _bocList_TrClassNameSelected = trClassNameSelected;
}

//  Initalizes an individual BocList's List. The initialization synchronizes the selection state 
//  arrays with the BocList's selected rows.
//  Call this method once for each BocList on the page.
//  bocList: The BocList to which the row belongs.
//  selectorControlPrefix: The common part of the selectorControles' ID (everything before the index).
//  count: The number of data rows in the BocList.
//  selection: The RowSelection enum value defining the selection mode (disabled/single/multiple)
//  hasClickSensitiveRows: true if the click event handler is bound to the data rows.
//  listMenu: The BocList's ListMenu, which has to be notified of the new selection count
function BocList_InitializeList(bocList, selectorControlPrefix, count, selection, hasClickSensitiveRows, listMenu)
{
  var selectedRows = new BocList_SelectedRows (selection);
  if (   selectedRows.Selection != _bocList_rowSelectionUndefined
      && selectedRows.Selection != _bocList_rowSelectionDisabled)
  {
    for (var i = 0; i < count; i++)
    {
      var selectorControlID = selectorControlPrefix + i;
      var selectorControl = document.getElementById (selectorControlID);
      if (selectorControl == null)
        continue;
      var row = selectorControl.parentNode.parentNode;
  
      if (hasClickSensitiveRows)
        BocList_BindRowClickEventHandler(bocList, row, selectorControl, listMenu);
  
      if (selectorControl.checked)      
      {
        var rowBlock = new BocList_RowBlock (row, selectorControl);
        selectedRows.Rows[selectorControl.id] = rowBlock;
        selectedRows.Length++;
      }
    }
  }
  _bocList_selectedRows[bocList.id] = selectedRows;

  var tableBlock = $(bocList).children().filter('.bocListTableBlock');
  var hasDimensions = false;
  if ($.browser.msie)
  {
    if ($(bocList).css('height') != 'auto' || $(bocList).css('width') != 'auto')
    {
      $(bocList).addClass('hasDimensions');
      hasDimensions = true;
    }
  }
  else
  {
    var isTableBlockBiggerThanBocList = $(bocList).css('height') < tableBlock.children().eq(0).children().eq(0).css('height');
    if (isTableBlockBiggerThanBocList)
    {
      $(bocList).addClass('hasDimensions');
      hasDimensions = true;
    }
  }

  if (hasDimensions)
  {
    BocList_FixUpScrolling(tableBlock);
  }

  BocList_syncCheckboxes(bocList);
}

function BocList_FixUpScrolling(tableBlock)
{
  //activateTableHeader only on first scroll
  var scrollTimer = null;
  var container = tableBlock.children().eq(0);
  var horizontalScroll = 0;

  container.bind('scroll', function (event)
  {
    // return if is horizontal scrolling
    var currentHorizontalScroll = $(this).scrollLeft();
    if (currentHorizontalScroll != horizontalScroll && currentHorizontalScroll > 0)
    {
      horizontalScroll = currentHorizontalScroll;
      return;
    }

    var currentBocList = $(this);
    BocList_activateTableHeader(currentBocList);
    if (scrollTimer) clearTimeout(scrollTimer);
    scrollTimer = setTimeout(function () { BocList_fixHeaderPosition(currentBocList) }, 50);
  });

  //activateTableHeader on window resize
  var resizeTimer = null;
  $(window).bind('resize', function ()
  {
    if (resizeTimer) clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function () { BocList_activateTableHeader(container); }, 50);
  });
}



function BocList_BindRowClickEventHandler(bocList, row, selectorControl, listMenu)
{
  $(row).click( function(evt)
  {
    BocList_OnRowClick(evt, bocList, row, selectorControl);
    ListMenu_Update(listMenu, function() { return BocList_GetSelectionCount(bocList.id); });
  });
}

//  Event handler for a table row in the BocList. 
//  Selects/unselects a row/all rows depending on its selection state,
//      whether CTRL has been pressed and if _bocList_isSelectorControlClick is true.
//  Aborts the execution if _bocList_isCommandClick or _bocList_isSelectorControlClick is true.
//  evt: The jQuery event object representing the click event
//  bocList: The BocList to which the row belongs.
//  currentRow: The row that fired the click event.
//  selectorControl: The selection selectorControl in this row.
function BocList_OnRowClick (evt, bocList, currentRow, selectorControl)
{
  if (_bocList_isCommandClick)
  {
    _bocList_isCommandClick = false;
    return;
  }  
  
  if (_bocList_isSelectorControlLabelClick)
  {
    _bocList_isSelectorControlLabelClick = false;
    return;
  }  

  var currentRowBlock = new BocList_RowBlock (currentRow, selectorControl);
  var selectedRows = _bocList_selectedRows[bocList.id];
  var isCtrlKeyPress = false;
  if (evt)
    isCtrlKeyPress = evt.ctrlKey;
    
  if (   selectedRows.Selection == _bocList_rowSelectionUndefined
      || selectedRows.Selection == _bocList_rowSelectionDisabled)
  {
    return;
  }
    
  if (isCtrlKeyPress || _bocList_isSelectorControlClick)
  {
    //  Is current row selected?
    if (selectedRows.Rows[selectorControl.id] != null)
    {
      //  Remove currentRow from list and unselect it
      BocList_UnselectRow (bocList, currentRowBlock);
    }
    else
    {
      if (  (   selectedRows.Selection == _bocList_rowSelectionSingleCheckBox
             || selectedRows.Selection == _bocList_rowSelectionSingleRadioButton)
          && selectedRows.Length > 0)
      {
        //  Unselect all rows and clear the list
        BocList_UnselectAllRows (bocList);
      }
      //  Add currentRow to list and select it
      BocList_SelectRow (bocList, currentRowBlock);
    }
  }
  else // cancel previous selection and select a new row
  {
    if (selectedRows.Length > 0)
    {
      //  Unselect all rows and clear the list
      BocList_UnselectAllRows (bocList);
    }
    //  Add currentRow to list and select it
    BocList_SelectRow (bocList, currentRowBlock);
  }
  try
  {
    selectorControl.focus();
  }
  catch (e)
  {
  }  
  _bocList_isSelectorControlClick = false;
}

//  Selects a row.
//  Adds the row to the _bocList_selectedRows array and increments _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  rowBlock: The row to be selected.
function BocList_SelectRow (bocList, rowBlock)
{
  //  Add currentRow to list  
  var selectedRows = _bocList_selectedRows[bocList.id];
  selectedRows.Rows[rowBlock.SelectorControl.id] = rowBlock;
  selectedRows.Length++;
    
  // Select currentRow
  rowBlock.Row.className = _bocList_TrClassNameSelected;
  rowBlock.SelectorControl.checked = true;
}

//  Unselects all rows in a BocList.
//  Clears _bocList_selectedRows array and sets _bocList_selectedRowsLength to zero.
//  bocList: The BocList whose rows should be unselected.
function BocList_UnselectAllRows (bocList)
{
  var selectedRows = _bocList_selectedRows[bocList.id];
  for (var rowID in selectedRows.Rows)
  {
    var rowBlock = selectedRows.Rows[rowID];
    if (rowBlock != null)
    {
      BocList_UnselectRow (bocList, rowBlock);
    }
  }
  
  //  Start over with a new array
  selectedRows.Clear();
}

//  Unselects a row.
//  Removes the row frin the _bocList_selectedRows array and decrements _bocList_selectedRowsLength.
//  bocList: The BocList to which the row belongs.
//  rowBlock: The row to be unselected.
function BocList_UnselectRow (bocList, rowBlock)
{
  //  Remove currentRow from list
  var selectedRows = _bocList_selectedRows[bocList.id];
  selectedRows.Rows[rowBlock.SelectorControl.id] = null;
  selectedRows.Length--;
    
  // Unselect currentRow
  rowBlock.Row.className = _bocList_TrClassName;
  rowBlock.SelectorControl.checked = false;
}

//  Event handler for the selection selectorControl in the title row.
//  Applies the checked state of the title's selectorControl to all data rows' selectu=ion selectorControles.
//  bocList: The BocList to which the selectorControl belongs.
//  selectorControlPrefix: The common part of the selectorControles' ID (everything before the index).
//  count: The number of data rows in the BocList.
function BocList_OnSelectAllSelectorControlClick (bocList, selectAllSelectorControl, selectorControlPrefix, count, listMenu)
{
  var selectedRows = _bocList_selectedRows[bocList.id];

  if (selectedRows.Selection != _bocList_rowSelectionMultiple)
    return;
  //  BocList_SelectRow will increment the length, therefor initialize it to zero.
  if (selectAllSelectorControl.checked)      
    selectedRows.Length = 0;

  for (var i = 0; i < count; i++)
  {
    var selectorControlID = selectorControlPrefix + i;
    var selectorControl = document.getElementById (selectorControlID);
    if (selectorControl == null)
      continue;
    var row =  selectorControl.parentNode.parentNode;
    var rowBlock = new BocList_RowBlock (row, selectorControl);
    if (selectAllSelectorControl.checked)      
      BocList_SelectRow (bocList, rowBlock)
    else
      BocList_UnselectRow (bocList, rowBlock)
  }
  
  if (! selectAllSelectorControl.checked)      
    selectedRows.Length = 0;

  ListMenu_Update(listMenu, function() { return BocList_GetSelectionCount(bocList.id); });
}

//  Event handler for the selection selectorControl in a data row.
//  Sets the _bocList_isSelectorControlClick flag.
function BocList_OnSelectionSelectorControlClick()
{
  _bocList_isSelectorControlClick = true;
}

//  Event handler for the label tags associated with the row index in a data row.
//  Sets the _bocList_isSelectorControlLabelClick flag.
function BocList_OnSelectorControlLabelClick()
{
  _bocList_isSelectorControlLabelClick = true;
}

//  Event handler for the anchor tags (commands) in a data row.
//  Sets the _bocList_isCommandClick flag.
function BocList_OnCommandClick()
{
  _bocList_isCommandClick = true;
}

//  Returns the number of rows selected for the specified BocList
function BocList_GetSelectionCount (bocListID)
{
  var selectedRows = _bocList_selectedRows[bocListID];
  if (selectedRows == null)
    return 0;
  return selectedRows.Length;
}

function BocList_activateTableHeader(container)
{
    var cointainerChildrens = container.children();
    var realThead = cointainerChildrens.eq(0).find('thead');
    var realTheadRow = realThead.children().eq(0);
    var realTheadRowChildrens = realTheadRow.children();

    var fakeTableContainer = cointainerChildrens.eq(1);
    var fakeThead = fakeTableContainer.find('thead');
    var fakeTheadRow = fakeThead.children().eq(0);
    var fakeTheadRowChildrens = fakeTheadRow.children();

    // hide bocListFakeTableHead if bocList is not scrollable
    if (!checkScrollBarPresence(container))
    {
        fakeTableContainer.hide();
        return;
    }
    
    BocList_syncCheckboxes(container);

    // store cell widths in array
    var realTheadCellWidths = new Array();

    realTheadRowChildrens.each(function(index)
    {
        realTheadCellWidths[index] = $(this).width();
    });

    // apply widths to fake header
    $.each(realTheadCellWidths, function(index, item)
    {
        fakeTheadRowChildrens.eq(index).width(item);
    });

    fakeTableContainer.width(realTheadRow.width());
}

function BocList_fixHeaderPosition(containerDiv)
{
    var bocListFakeTableHead = containerDiv.find('div.bocListFakeTableHead');
    var scrollPosition = containerDiv.scrollTop();
    if (scrollPosition == 0)
    {
        bocListFakeTableHead.hide();
    } else
    {
        bocListFakeTableHead.show();
    }
    bocListFakeTableHead.css({ 'top': scrollPosition, 'left':'0' });
}

function BocList_syncCheckboxes(container)
{
    // sync checkboxes
    var checkboxes = $(container).find("th input:checkbox");
    checkboxes.click(function()
    {
        var checkName = $(this).attr('name');
        var realCheckName = checkName.replace('_fake', '');
        var checkStatus = $(this).attr('checked');
        $('input[name*=' + realCheckName + ']').attr('checked', checkStatus);
    });
}

function checkScrollBarPresence(element)
{
        return ((element.attr('scrollHeight') > element.innerHeight()) || (element.attr('scrollWidth') > element.innerWidth()));
}
