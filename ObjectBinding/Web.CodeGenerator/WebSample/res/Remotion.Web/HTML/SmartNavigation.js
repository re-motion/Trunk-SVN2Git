function SmartScrolling_Element (id, top, left)
{
  this.ID = id;
  this.Top = top;
  this.Left = left;
  
  this.ToString = function ()
  {
    if (StringUtility.IsNullOrEmpty (this.ID))
      return '';
    else
      return this.ID + ' ' + this.Top + ' ' + this.Left;
  };
}

SmartScrolling_Element.Parse = function (value)
{
  ArgumentUtility.CheckTypeIsString ('value', value);
  if (StringUtility.IsNullOrEmpty (value))
    return null;

  var fields = value.split (' ');
  return new SmartScrolling_Element (fields[0], fields[1], fields[2]);
};

function SmartScrolling_Restore (data)
{
  ArgumentUtility.CheckTypeIsString ('data', data);
  if (StringUtility.IsNullOrEmpty (data))
    return;
        
  var dataFields = data.split ('*');
  if (dataFields.length == 0)
    return;
  
  var dataField = dataFields[0];
  dataFields = dataFields.slice (1);
  var sseBody = SmartScrolling_Element.Parse (dataField);
  window.document.body.scrollTop = sseBody.Top;
  window.document.body.scrollLeft = sseBody.Left;
  
  for (var i = 0; i < dataFields.length; i++)
  {
    var scrollElement = SmartScrolling_Element.Parse (dataFields[i]);
    SmartScrolling_SetScrollPosition (scrollElement);
  } 
}

function SmartScrolling_Backup (activeElement)
{
  var data = '';
  var scrollElements = new Array();
  
  if (TypeUtility.IsUndefined (window.document.body.id) || StringUtility.IsNullOrEmpty (window.document.body.id))
  {
    var sseBody = 
        new SmartScrolling_Element ('body', window.document.body.scrollTop, window.document.body.scrollLeft);
    scrollElements[scrollElements.length] = sseBody;
  }
  scrollElements = scrollElements.concat (SmartScrolling_GetScrollPositions (window.document.body));
  
  for (var i = 0; i < scrollElements.length; i++)
  {
    if (i > 0)
      data += '*'; 
    var scrollElement = scrollElements[i];
    data += scrollElement.ToString();
  }

  return data;
}

function SmartScrolling_GetScrollPositions (currentElement)
{
  var scrollElements = new Array();
  if (currentElement != null)
  {
    if (   ! TypeUtility.IsUndefined (currentElement.id) && ! StringUtility.IsNullOrEmpty (currentElement.id)
        && (currentElement.scrollTop != 0 || currentElement.scrollLeft != 0))
    {
      var sseCurrentElement = SmartScrolling_GetScrollPosition (currentElement);
      scrollElements[scrollElements.length] = sseCurrentElement;
    }
    
    for (var i = 0; i < currentElement.childNodes.length; i++)
    {
      var element = currentElement.childNodes[i];
      var scrollChilden = SmartScrolling_GetScrollPositions (element);
      scrollElements = scrollElements.concat (scrollChilden);
    }
  }
  return scrollElements;  
}

function SmartScrolling_GetScrollPosition (htmlElement)
{
  if (htmlElement != null)
    return new SmartScrolling_Element (htmlElement.id, htmlElement.scrollTop, htmlElement.scrollLeft);
  else
    return null;
}

function SmartScrolling_SetScrollPosition (scrollElement)
{
  if (scrollElement == null)
    return;
  var htmlElement = window.document.getElementById (scrollElement.ID);
  if (htmlElement == null)
    return;
  htmlElement.scrollTop = scrollElement.Top;
  htmlElement.scrollLeft = scrollElement.Left;
}

function SmartFocus_Backup (activeElement)
{
  var data = '';  
  if (activeElement != null)
  {
    data += activeElement.id;
  }
  
  return data;
}

function SmartFocus_Restore (data)
{
  var activeElementID = data;
  if (! StringUtility.IsNullOrEmpty (activeElementID))
  {
    var activeElement = window.document.getElementById (activeElementID);
    if (activeElement != null)
    {
      activeElement.focus();
    }
  }
}

