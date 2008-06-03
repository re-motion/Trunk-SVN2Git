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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{
  /// <summary>
  ///   A desinger that requries the complete loading of the control.
  /// </summary>
  public class WebControlDesigner : ControlDesigner
  {
    public override void Initialize (IComponent component)
    {
      ArgumentUtility.CheckNotNull ("component", component);

      base.Initialize (component);
      SetViewFlags (ViewFlags.DesignTimeHtmlRequiresLoadComplete, true);

      Assertion.IsNotNull (component.Site, "The component does not have a Site.");
      IDesignerHost designerHost = (IDesignerHost) component.Site.GetService (typeof (IDesignerHost));
      Assertion.IsNotNull (designerHost, "No IDesignerHost found.");

      if (!DesignerUtility.IsDesignMode || !object.ReferenceEquals (DesignerUtility.DesignerHost, designerHost))
        DesignerUtility.SetDesignMode (new WebDesginModeHelper (designerHost));
    }

    public override string GetDesignTimeHtml ()
    {
      try
      {
        IControlWithDesignTimeSupport control = Component as IControlWithDesignTimeSupport;
        if (control != null)
          control.PreRenderForDesignMode();
      }
      catch (Exception e)
      {
        System.Diagnostics.Debug.WriteLine (e.Message);
      }

      return base.GetDesignTimeHtml();
    }
  }
}
