/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{

public class ReflectionBusinessObjectWebUIService: IBusinessObjectWebUIService
{ 
  public IconInfo GetIcon (IBusinessObject obj)
  {
    if (obj == null)
    {
      string url = "~/Images/NullIcon.gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }
    else
    {
      string url = "~/Images/" + ((BindableObjectClass) obj.BusinessObjectClass).TargetType.FullName + ".gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }
  }

  public string GetToolTip (IBusinessObject obj)
  {
    if (obj == null)
      return "No ToolTip";
    else
      return "ToolTip: " + ((BindableObjectClass)obj.BusinessObjectClass).TargetType.FullName;
  }

}

}
