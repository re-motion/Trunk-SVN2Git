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
