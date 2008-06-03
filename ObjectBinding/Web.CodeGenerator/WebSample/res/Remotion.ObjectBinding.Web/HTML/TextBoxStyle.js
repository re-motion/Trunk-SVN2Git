/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

// Cannot detect paste operations.
function TextBoxStyle_OnKeyDown (textBox, length)
{
  if (textBox.disabled)
    return true;
    
  var isInsertDelete = event.keyCode == 45 || event.keyCode == 46;
  var isCursor = event.keyCode >= 37 && event.keyCode <= 40; // Left, Top, Right, Buttom
  var isPagePosition = event.keyCode >= 33 && event.keyCode <= 36; // Home, End, PageUp, PageDown
  var isControlCharacter = 
         event.keyCode < 32 
      || isInsertDelete 
      || isCursor 
      || isPagePosition
      || event.ctrlKey 
      || event.altKey;
  var isLineFeed = event.keyCode == 10;
  var isCarriageReturn = event.keyCode == 13;
  
  
  if (isControlCharacter && ! (isLineFeed || isCarriageReturn))
    return true;
  
  if (textBox.value.length >= length) 
    return false;
    
  return true;
}
