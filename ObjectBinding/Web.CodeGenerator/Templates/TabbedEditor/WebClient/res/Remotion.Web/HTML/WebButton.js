function WebButton_MouseDown (element, cssClass)
{
  element.className += " " + cssClass;
  return false;
}

function WebButton_MouseUp (element, cssClass)
{
  element.className = element.className.replace (cssClass, '');
  return false;
}

function WebButton_MouseOut (element, cssClass)
{
  element.className = element.className.replace (cssClass, '');
  return false;
}

