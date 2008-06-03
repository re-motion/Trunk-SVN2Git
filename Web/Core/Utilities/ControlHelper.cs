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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Utilities
{

  public static class ControlHelper
  {
    public static string PostEventSourceID
    { get { return "__EVENTTARGET"; } }

    public static string PostEventArgumentID
    { get { return "__EVENTARGUMENT"; } }

    public static string ViewStateID
    { get { return "__VIEWSTATE"; } }

    public static Control[] GetControlsRecursive (Control parentControl, Type type)
    {
      ArrayList controlList = new ArrayList ();
      GetControlsRecursiveInternal (parentControl, type, controlList);
      if (type.IsInterface)
        type = typeof (Control);
      return (Control[]) controlList.ToArray (type);
    }

    public static Control[] GetControlsRecursive (Control parentControl, Type type, Control[] stopList)
    {
      ArrayList controlList = new ArrayList ();
      GetControlsRecursiveInternal (parentControl, type, new ArrayList (stopList), controlList);
      if (type.IsInterface)
        type = typeof (Control);
      return (Control[]) controlList.ToArray (type);
    }

    private static void GetControlsRecursiveInternal
        (Control parentControl, Type type, ArrayList stopList, ArrayList controlList)
    {
      ControlCollection controls = parentControl.Controls;
      for (int i = 0; i < controls.Count; ++i)
      {
        Control control = controls[i];
        if (!stopList.Contains (control))
        {
          if (type.IsInstanceOfType (control))
            controlList.Add (control);

          GetControlsRecursiveInternal (control, type, stopList, controlList);
        }
      }
    }

    private static void GetControlsRecursiveInternal
        (Control parentControl, Type type, ArrayList controlList)
    {
      ControlCollection controls = parentControl.Controls;
      for (int i = 0; i < controls.Count; ++i)
      {
        Control control = controls[i];
        if (type.IsInstanceOfType (control))
          controlList.Add (control);

        GetControlsRecursiveInternal (control, type, controlList);
      }
    }

    public static bool ValidateOrder (BaseValidator smallerValidator, BaseValidator largerValidator, Type type)
    {
      TextBox smallerField = smallerValidator.NamingContainer.FindControl (smallerValidator.ControlToValidate) as TextBox;
      if (smallerField == null)
        throw new ArgumentException ("ControlToValidate must be TextBox", "smallerValidator");
      TextBox largerField = largerValidator.NamingContainer.FindControl (largerValidator.ControlToValidate) as TextBox;
      if (largerField == null)
        throw new ArgumentException ("ControlToValidate must be TextBox", "largerValidator");

      if (smallerField.Text.Trim () == string.Empty || largerField.Text.Trim () == string.Empty)
        return true;

      smallerValidator.Validate ();
      largerValidator.Validate ();
      if (!(smallerValidator.IsValid && largerValidator.IsValid))
        return true;

      IComparable smallerValue = (IComparable) Convert.ChangeType (smallerField.Text, type);
      IComparable largerValue = (IComparable) Convert.ChangeType (largerField.Text, type);

      if (smallerValue.CompareTo (largerValue) > 0)
        return false;
      else
        return true;
    }

    /// <summary>
    ///   This method returns the nearest containing Template Control (i.e., Page or User Control).
    /// </summary>
    public static TemplateControl GetParentTemplateControl (Control control)
    {
      for (Control parent = control;
           parent != null;
           parent = parent.Parent)
      {
        if (parent is TemplateControl)
          return (TemplateControl) parent;
      }
      return null;
    }

    /// <summary>
    ///   This method returns <see langword="true"/> if the <paramref name="control"/> is in 
    ///   design mode.
    /// </summary>
    /// <remarks>
    ///   Does not verify the control's context.
    /// </remarks>
    /// <param name="control"> 
    ///   The <see cref="Control"/> to be tested for being in design mode. 
    /// </param>
    /// <returns> 
    ///   Returns <see langword="true"/> if the <paramref name="control"/> is in design mode.
    /// </returns>
    public static bool IsDesignMode (Control control)
    {
      if (control.Site != null && control.Site.DesignMode)
        return true;
      if (control.Page != null && control.Page.Site != null && control.Page.Site.DesignMode)
        return true;
      return false;
    }

    /// <summary>
    ///   This method returns <see langword="true"/> if the <paramref name="control"/> is in 
    ///   design mode.
    /// </summary>
    /// <param name="control"> 
    ///   The <see cref="Control"/> to be tested for being in design mode. 
    /// </param>
    /// <param name="context"> 
    ///   The <see cref="HttpContext"/> of the <paramref name="control"/>. 
    /// </param>
    /// <returns> 
    ///   Returns <see langword="true"/> if the <paramref name="control"/> is in design mode.
    /// </returns>
    public static bool IsDesignMode (IControl control, HttpContext context)
    {
      return context == null || ControlHelper.IsDesignMode (control);
    }

    /// <summary>
    ///   This method returns <see langword="true"/> if the <paramref name="control"/> is in 
    ///   design mode.
    /// </summary>
    /// <remarks>
    ///   Does not verify the control's context.
    /// </remarks>
    /// <param name="control"> 
    ///   The <see cref="IControl"/> to be tested for being in design mode. 
    /// </param>
    /// <returns> 
    ///   Returns <see langword="true"/> if the <paramref name="control"/> is in design mode.
    /// </returns>
    public static bool IsDesignMode (IControl control)
    {
      if (control.Site != null && control.Site.DesignMode)
        return true;
      if (control.Page != null && control.Page.Site != null && control.Page.Site.DesignMode)
        return true;
      return false;
    }

    public static Control FindControl (Control namingContainer, string controlID)
    {
      ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);
      if (StringUtility.IsNullOrEmpty (controlID))
        return null;

      try
      {
        //  WORKAROUND: In Designmode the very first call to FindControl results in a duplicate entry.
        //  Once that initial confusion has passed, everything seems to work just fine.
        //  Reason unknown (bug in Remotion-code or bug in Framework-code)
        return namingContainer.FindControl (controlID);
      }
      catch (HttpException)
      {
        if (ControlHelper.IsDesignMode (namingContainer))
          return namingContainer.FindControl (controlID);
        else
          throw;
      }
    }

    /// <summary> Encapsulates the invokation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be restored. </param>
    /// <param name="viewState"> The view state object used for restoring. </param>
    public static void LoadViewStateRecursive (Control target, object viewState)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      const BindingFlags bindingFlags = BindingFlags.DeclaredOnly
                                      | BindingFlags.Instance
                                      | BindingFlags.NonPublic
                                      | BindingFlags.InvokeMethod;

      //  HACK: Reflection on internal void Control.LoadViewStateRecursive (object)
      //  internal void System.Web.UI.Control.LoadViewStateRecursive (object)
      typeof (Control).InvokeMember ("LoadViewStateRecursive", bindingFlags, null, target, new object[] { viewState });
    }

    /// <summary> Encapsulates the invokation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be saved. </param>
    /// <returns> The view state object for <paramref name="target"/>. </returns>
    public static object SaveViewStateRecursive (Control target)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      const BindingFlags bindingFlags = BindingFlags.DeclaredOnly
                                      | BindingFlags.Instance
                                      | BindingFlags.NonPublic
                                      | BindingFlags.InvokeMethod;

      //  HACK: Reflection on internal object Control.SaveViewStateRecursive()
      //  internal object System.Web.UI.Control.LoadViewStateRecursive()
      object viewState = typeof (Control).InvokeMember ("SaveViewStateRecursive", bindingFlags, null, target, new object[0]);

      return viewState;
    }

    public static PageStatePersister GetPageStatePersister (Page target)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      const BindingFlags bindingFlags = BindingFlags.DeclaredOnly
                                      | BindingFlags.Instance
                                      | BindingFlags.NonPublic
                                      | BindingFlags.GetProperty;

      //  HACK: Reflection on protected PageStatePersister Page.PageStatePersister
      //  protected PageStatePersister System.Web.UI.Page.PageStatePersister
      return (PageStatePersister) typeof (Page).InvokeMember ("PageStatePersister", bindingFlags, null, target, new object[0]);
    }

    public static bool IsResponseTextXml (HttpContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      return context.Response.ContentType.Equals ("TEXT/XML", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsResponseTextXHtml (HttpContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      return context.Response.ContentType.Equals ("TEXT/XHTML", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsXmlConformResponseTextRequired (HttpContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      XhtmlConformanceSection xhtmlConformanceSection = (XhtmlConformanceSection) WebConfigurationManager.GetSection ("system.web/xhtmlConformance");
      Assertion.IsNotNull (xhtmlConformanceSection, "Config section 'system.web/xhtmlConformance' was not found.");

      if (xhtmlConformanceSection.Mode != XhtmlConformanceMode.Legacy)
        return true;

      return IsResponseTextXml (context) || IsResponseTextXHtml (context);
    }
  }

}
