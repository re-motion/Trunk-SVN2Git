/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

