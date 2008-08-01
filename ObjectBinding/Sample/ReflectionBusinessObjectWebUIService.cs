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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class ReflectionBusinessObjectWebUIService : IBusinessObjectWebUIService
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
        return "ToolTip: " + ((BindableObjectClass) obj.BusinessObjectClass).TargetType.FullName;
    }

    public HelpInfo GetHelpInfo (
        IBusinessObjectBoundWebControl control,
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty businessObjectProperty,
        IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("businessObjectClass", businessObjectClass);

      return new HelpInfo (
          "#",
          null,
          string.Format (
              "{0}\r\n{1}\r\n{2}\r\n{3}",
              control.ID,
              businessObjectClass.Identifier,
              (businessObjectProperty != null ? businessObjectProperty.Identifier : "prop"),
              (businessObject != null ? businessObject.DisplayName : "obj")),
          "return false;");
    }
  }
}