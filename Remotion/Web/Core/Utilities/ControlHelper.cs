// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections;
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
    private static readonly IInternalControlMemberCaller s_memberCaller = new InternalControlMemberCaller();

    public static string PostEventSourceID
    {
      get { return Page.postEventSourceID; }
    }

    public static string PostEventArgumentID
    {
      get { return Page.postEventArgumentID; }
    }

    public static string ViewStateFieldPrefixID
    {
      get { return "__VIEWSTATE"; }
    }
    
    public static string AsyncPostBackErrorKey
    {
      get { return "System.Web.UI.PageRequestManager:AsyncPostBackError"; }
    }

    public static string AsyncPostBackErrorMessageKey
    {
      get { return "System.Web.UI.PageRequestManager:AsyncPostBackErrorMessage"; }
    }

    public static string AsyncPostBackErrorHttpCodeKey
    {
      get { return "System.Web.UI.PageRequestManager:AsyncPostBackErrorHttpCode"; }
    }

    public static bool IsNestedInUpdatePanel (Control child)
    {
      ArgumentUtility.CheckNotNull ("child", child);

      var scriptManager = ScriptManager.GetCurrent (child.Page);
      if (scriptManager == null)
        return false;

      for (Control current = child; current != null && !(current is Page); current = current.Parent)
      {
        if (current is UpdatePanel)
          return true;
      }
      return false;
    }

    public static Control[] GetControlsRecursive (Control parentControl, Type type)
    {
      ArrayList controlList = new ArrayList();
      GetControlsRecursiveInternal (parentControl, type, controlList);
      if (type.IsInterface)
        type = typeof (Control);
      return (Control[]) controlList.ToArray (type);
    }

    public static Control[] GetControlsRecursive (Control parentControl, Type type, Control[] stopList)
    {
      ArrayList controlList = new ArrayList();
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

      if (smallerField.Text.Trim() == string.Empty || largerField.Text.Trim() == string.Empty)
        return true;

      smallerValidator.Validate();
      largerValidator.Validate();
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
    public static bool IsDesignModeForControl (Control control)
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
      ArgumentUtility.CheckNotNull ("control", control);

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
        if (IsDesignModeForControl (namingContainer))
          return namingContainer.FindControl (controlID);
        else
          throw;
      }
    }

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be restored. </param>
    /// <param name="viewState"> The view state object used for restoring. </param>
    public static void LoadViewStateRecursive (Control target, object viewState)
    {
      s_memberCaller.LoadViewStateRecursive (target, viewState);
    }

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be saved. </param>
    /// <returns> The view state object for <paramref name="target"/>. </returns>
    public static object SaveViewStateRecursive (Control target)
    {
      return s_memberCaller.SaveViewStateRecursive (target);
    }

    /// <summary>Encapsulates the invocation of <see cref="Page"/>'s SaveAllState method.</summary>
    /// <param name="page">The <see cref="Page"/> for which SaveAllState will be invoked. Must not be <see langword="null" />.</param>
    public static void SaveAllState (Page page)
    {
      s_memberCaller.SaveAllState (page);
    }

    /// <summary>Returns the control states for all controls that are child-controls of the passed <see cref="Control"/>.</summary>
    public static IDictionary GetChildControlState<TNamingContainer> (TNamingContainer control) where TNamingContainer : Control, INamingContainer
    {
      return s_memberCaller.GetChildControlState (control);
    }

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    public static void SetChildControlState<TNamingContainer> (TNamingContainer control, IDictionary newControlState)
        where TNamingContainer : Control, INamingContainer
    {
      s_memberCaller.SetChildControlState (control, newControlState);
    }

    public static bool IsResponseTextXml (HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      return context.Response.ContentType.Equals ("TEXT/XML", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsResponseTextXHtml (HttpContextBase context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      return context.Response.ContentType.Equals ("TEXT/XHTML", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsXmlConformResponseTextRequired (HttpContextBase context)
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