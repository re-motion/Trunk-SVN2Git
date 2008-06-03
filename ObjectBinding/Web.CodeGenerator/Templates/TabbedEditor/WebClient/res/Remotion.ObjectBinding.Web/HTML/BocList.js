/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

var _bocList_listMenuInfos = new Object();

var _contentMenu_itemClassName = 'contentMenuItem';
var _contentMenu_itemFocusClassName = 'contentMenuItemFocus';
var _contentMenu_itemDisabledClassName = 'contentMenuItemDisabled';
var _contentMenu_requiredSelectionAny = 0;
var _contentMenu_requiredSelectionExactlyOne = 1;
var _contentMenu_requiredSelectionOneOrMore = 2;

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
function BocList_InitializeList (bocList, selectorControlPrefix, count, selection)
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
      var row =  selectorControl.parentNode.parentNode;
      if (selectorControl.checked)      
      {
        var rowBlock = new BocList_RowBlock (row, selectorControl);
        selectedRows.Rows[selectorControl.id] = rowBlock;
        selectedRows.Length++;
        
        if (   selectedRows.Selection == _bocList_rowSelectionSingleCheckBox
            || selectedRows.Selection == _bocList_rowSelectionSingleRadioButton)
        {
          break;
        }
      }
    }
  }
  _bocList_selectedRows[bocList.id] = selectedRows;
}

//  Event handler for a table row in the BocList. 
//  Selects/unselects a row/all rows depending on its selection state,
//      whether CTRL has been pressed and if _bocList_isSelectorControlClick is true.
//  Aborts the execution if _bocList_isCommandClick or _bocList_isSelectorControlClick is true.
//  bocList: The BocList to which the row belongs.
//  currentRow: The row that fired the click event.
//  selectorControl: The selection selectorControl in this row.
function BocList_OnRowClick (bocList, currentRow, selectorControl)
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
  var isCtrlKeyPress = window.event.ctrlKey;
    
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

  BocList_UpdateListMenu (bocList);
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
function BocList_OnSelectAllSelectorControlClick (bocList, selectAllSelectorControl, selectorControlPrefix, count)
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
    
  BocList_UpdateListMenu (bocList);
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

function ContentMenu_MenuInfo (id, itemInfos)
{
  this.ID = id;
  this.ItemInfos = itemInfos;
}

function BocList_AddMenuInfo (bocList, menuInfo)
{
  _bocList_listMenuInfos[bocList.id] = menuInfo;
}

function ContentMenu_MenuItemInfo (id, category, text, icon, iconDisabled, requiredSelection, isDisabled, href, target)
{
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

function BocList_UpdateListMenu (bocList)
{
  var menuInfo = _bocList_listMenuInfos[bocList.id];
  if (menuInfo == null)
    return;
    
  var itemInfos = menuInfo.ItemInfos;
  var selectionCount = BocList_GetSelectionCount (bocList.id);
  
  for (var i = 0; i < itemInfos.length; i++)
  {
    var itemInfo = itemInfos[i];
    var isEnabled = true;
    if (itemInfo.IsDisabled)
    {
      isEnabled = false;
    }
    else
    {
      if (   itemInfo.RequiredSelection == _contentMenu_requiredSelectionExactlyOne
          && selectionCount != 1)
      {
        isEnabled = false;
      }
      if (   itemInfo.RequiredSelection == _contentMenu_requiredSelectionOneOrMore
          && selectionCount < 1)
      {
        isEnabled = false;
      }
    }
    var item = document.getElementById (itemInfo.ID);
    var anchor = item.children[0];
    var icon = anchor.children[0];
    if (isEnabled)
    {
      if (icon != null)
        icon.src = itemInfo.Icon;
  	  item.className = _contentMenu_itemClassName;
  	  if (itemInfo.Href != null)
      {
        if (itemInfo.Href.toLowerCase().indexOf ('javascript:') >= 0)
        {
          anchor.href = '#';
          anchor.removeAttribute ('target');
          anchor.setAttribute ('javascript', itemInfo.Href);
          anchor.onclick = function () { eval (this.getAttribute ('javascript')); };
        }
        else
        {
          anchor.href = itemInfo.Href;
          if (itemInfo.Target != null)
      	    anchor.target = itemInfo.Target;
          anchor.removeAttribute ('onclick');
          anchor.removeAttribute ('javascript');
        }
      }
    }
    else
    {
      if (icon != null)
      {
        if (itemInfo.IconDisabled != null)
          icon.src = itemInfo.IconDisabled;
        else
          icon.src = itemInfo.Icon;
      }
      item.className = _contentMenu_itemDisabledClassName;
      anchor.removeAttribute ('href');
      anchor.removeAttribute ('target');
      anchor.removeAttribute ('onclick');
      anchor.removeAttribute ('javascript');
    }
  }
}

function ContentMenu_GoTo (menuItem)
{
  window.location = menuItem.href;
}

function ContentMenu_SelectItem (menuItem)
{
	if (menuItem == null)
	  return;
	menuItem.className = _contentMenu_itemFocusClassName;
}

function ContentMenu_UnselectItem (menuItem)
{
	if (menuItem == null)
	  return;
	menuItem.className = _contentMenu_itemClassName;
}
