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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
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

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be restored. </param>
    /// <param name="viewState"> The view state object used for restoring. </param>
    public static void LoadViewStateRecursive (Control target, object viewState)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      //  HACK: Reflection on internal void Control.LoadViewStateRecursive (object)
      //  internal void System.Web.UI.Control.LoadViewStateRecursive (object)
      MethodCaller.CallAction ("LoadViewStateRecursive", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic)
        .With (target, viewState);
    }

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be saved. </param>
    /// <returns> The view state object for <paramref name="target"/>. </returns>
    public static object SaveViewStateRecursive (Control target)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      //  HACK: Reflection on internal object Control.SaveViewStateRecursive()
      //  internal object System.Web.UI.Control.LoadViewStateRecursive()
      return MethodCaller.CallFunc<object> ("SaveViewStateRecursive", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic)
        .With (target);
    }
    
    /// <summary>Encapsulates the invocation of <see cref="Page"/>'s SaveAllState method.</summary>
    /// <param name="page">The <see cref="Page"/> for which SaveAllState will be invoked. Must not be <see langword="null" />.</param>
    public static void SaveAllState (Page page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      //  HACK: Reflection on protected void Page.SaveAllState()
      //  private void System.Web.UI.Page.SaveAllState()
      MethodCaller.CallAction ("SaveAllState", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).With (page);
    }

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveChildControlState method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveChildControlState will be invoked. Must not be <see langword="null" />.</param>
    public static Dictionary<string, object> SaveChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer : Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //  HACK: Reflection on private ControlSet Page._registeredControlsRequiringControlState
      //  private ControlSet System.Web.UI.Page._registeredControlsRequiringControlState
      var registeredControlsRequiringControlStateFieldInfo = typeof (Page).GetField ("_registeredControlsRequiringControlState", BindingFlags.Instance | BindingFlags.NonPublic);
      var registeredControlsRequiringControlState = (ICollection) registeredControlsRequiringControlStateFieldInfo.GetValue (control.Page);
      
      Dictionary<string, object> dictionary = new Dictionary<string, object> ();
      if (registeredControlsRequiringControlState != null)
      {
        foreach (Control registeredControl in registeredControlsRequiringControlState)
        {
          if (registeredControl.UniqueID.StartsWith (control.UniqueID) && registeredControl != control)
          {
            object controlState = SaveControlStateInternal (registeredControl);
            if (controlState != null)
              dictionary.Add (registeredControl.UniqueID, controlState);
          }
        }
      }

      if (dictionary.Count == 0)
        return null;
      return dictionary;
    }

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveControlStateInternal method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveControlStateInternal will be invoked. Must not be <see langword="null" />.</param>
    public static object SaveControlStateInternal (Control control)
    {
      //  HACK: Reflection on protected object Page.SaveControlStateInternal
      //  protected object System.Web.UI.Page.SaveControlStateInternal
      return MethodCaller.CallFunc<object> ("SaveControlStateInternal", BindingFlags.Instance | BindingFlags.NonPublic).With (control);
    }

    /// <summary>Returns the control states for all controls that are child-controls of the passed <see cref="Control"/>.</summary>
    public static Dictionary<string, object> GetChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer : Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      var childControlState = new Dictionary<string, object> ();

      var pageStatePersister = ControlHelper.GetPageStatePersister (control.Page);
      var controlStates = (IDictionary) pageStatePersister.ControlState;

      foreach (string key in controlStates.Keys)
      {
        if (key.StartsWith (control.UniqueID) && key != control.UniqueID)
          childControlState.Add (key, controlStates[key]);
      }

      if (childControlState.Count == 0)
        return null;
      return childControlState;
    }

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    public static void SetChildControlState<TNamingContainer> (TNamingContainer control, IDictionary newControlState)
        where TNamingContainer : Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (newControlState == null)
        return;

      var pageStatePersister = ControlHelper.GetPageStatePersister (control.Page);
      var controlState = (IDictionary) pageStatePersister.ControlState;

      foreach (string key in newControlState.Keys)
        controlState[key] = newControlState[key];
    }

    /// <summary>Encapsulates the get-access the the <see cref="Page"/>'s PageStatePersister property.</summary>
    public static PageStatePersister GetPageStatePersister (Page page)
    {
      ArgumentUtility.CheckNotNull ("target", page);

      const BindingFlags bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty;

      //  HACK: Reflection on protected PageStatePersister Page.PageStatePersister
      //  protected PageStatePersister System.Web.UI.Page.PageStatePersister
      return (PageStatePersister) typeof (Page).InvokeMember ("PageStatePersister", bindingFlags, null, page, new object[0]);
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
