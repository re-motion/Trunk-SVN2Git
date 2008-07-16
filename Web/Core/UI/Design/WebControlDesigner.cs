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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
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
    private bool _hasCheckedForDuplicateAssemblies;

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

      EnsureCheckForDuplicateAssemblies();
    }

    public override string GetDesignTimeHtml ()
    {
      EnsureCheckForDuplicateAssemblies();

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

    protected void EnsureCheckForDuplicateAssemblies ()
    {
      if (_hasCheckedForDuplicateAssemblies)
        return;

      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      Assembly[] remotionAssemblies = Array.FindAll (
          assemblies,
          delegate (Assembly assembly) { return assembly.FullName.StartsWith ("Remotion"); });

      Dictionary<string, Assembly> assemblyDictionary = new Dictionary<string, Assembly>();
      foreach (Assembly assembly in remotionAssemblies)
      {
        if (assemblyDictionary.ContainsKey (assembly.FullName))
        {
          throw new NotSupportedException (
              "Duplicate re:motion framework assemblies have been detected. In order to provide a consistent design time experience it is necessary"
              + " to install the re:motion framework in the global assembly cache (GAC). In addition, please ensure that the 'Copy Local' flag"
              + " is set to 'true' for all re:motion framework assemblies referenced by the web project.");
        }
        assemblyDictionary.Add (assembly.FullName, assembly);
      }

      _hasCheckedForDuplicateAssemblies = true;
    }
  }
}