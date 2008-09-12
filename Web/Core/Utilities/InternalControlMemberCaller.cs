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
using System.Reflection;
using System.Web.UI;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  public class InternalControlMemberCaller : IInternalControlMemberCaller
  {
    private const BindingFlags c_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    public static readonly Type InternalControlStateType = typeof (Control).Assembly.GetType ("System.Web.UI.ControlState", true, false);

    public void SetControlState (Control control, ControlState value)
    {
      object internalValue = Enum.ToObject (InternalControlStateType, value);
      MethodCaller.CallAction ("set_ControlState", c_bindingFlags)
        .Invoke (new[] { typeof (Control), InternalControlStateType }, new[] { control, internalValue });
    }

    public ControlState GetControlState (Control control)
    {
      int internalValue = MethodCaller.CallFunc<int> ("get_ControlState", c_bindingFlags).With (control);
      return (ControlState) internalValue;
    }

    public void InitRecursive (Control control, Control namingContainer)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);
      MethodCaller.CallAction ("InitRecursive", c_bindingFlags).With (control, namingContainer);
    }

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be restored. </param>
    /// <param name="viewState"> The view state object used for restoring. </param>
    public void LoadViewStateRecursive (Control target, object viewState)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      //  internal void System.Web.UI.Control.LoadViewStateRecursive (object)
      MethodCaller.CallAction ("LoadViewStateRecursive", c_bindingFlags).With (target, viewState);
    }

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be saved. </param>
    /// <returns> The view state object for <paramref name="target"/>. </returns>
    public object SaveViewStateRecursive (Control target)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      //  internal object System.Web.UI.Control.LoadViewStateRecursive()
      return MethodCaller.CallFunc<object> ("SaveViewStateRecursive", c_bindingFlags).With (target);
    }

    /// <summary>Encapsulates the invocation of <see cref="Page"/>'s SaveAllState method.</summary>
    /// <param name="page">The <see cref="Page"/> for which SaveAllState will be invoked. Must not be <see langword="null" />.</param>
    public void SaveAllState (Page page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      //  private void System.Web.UI.Page.SaveAllState()
      MethodCaller.CallAction ("SaveAllState", c_bindingFlags).With (page);
    }

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveChildControlState method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveChildControlState will be invoked. Must not be <see langword="null" />.</param>
    public Dictionary<string, object> SaveChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //  private ControlSet System.Web.UI.Page._registeredControlsRequiringControlState
      var registeredControlsRequiringControlStateFieldInfo = typeof (Page).GetField ("_registeredControlsRequiringControlState", c_bindingFlags);
      var registeredControlsRequiringControlState = (ICollection) registeredControlsRequiringControlStateFieldInfo.GetValue (control.Page);

      Dictionary<string, object> dictionary = new Dictionary<string, object>();
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
    public object SaveControlStateInternal (Control control)
    {
      //  protected object System.Web.UI.Page.SaveControlStateInternal
      return MethodCaller.CallFunc<object> ("SaveControlStateInternal", c_bindingFlags).With (control);
    }

    /// <summary>Returns the control states for all controls that are child-controls of the passed <see cref="Control"/>.</summary>
    public Dictionary<string, object> GetChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      var childControlState = new Dictionary<string, object>();

      var pageStatePersister = GetPageStatePersister (control.Page);
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
    public void SetChildControlState<TNamingContainer> (TNamingContainer control, IDictionary newControlState)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (newControlState == null)
        return;

      var pageStatePersister = GetPageStatePersister (control.Page);
      var controlState = (IDictionary) pageStatePersister.ControlState;

      foreach (string key in newControlState.Keys)
        controlState[key] = newControlState[key];
    }

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    public void ClearChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //  protected void System.Web.UI.Control.ClearChildControlState
      MethodCaller.CallFunc<string> ("ClearChildControlState", c_bindingFlags).With (control);
    }

    /// <summary>Encapsulates the get-access the the <see cref="Page"/>'s PageStatePersister property.</summary>
    public PageStatePersister GetPageStatePersister (Page page)
    {
      ArgumentUtility.CheckNotNull ("target", page);

      //  protected PageStatePersister System.Web.UI.Page.PageStatePersister
      return MethodCaller.CallFunc<PageStatePersister> ("get_PageStatePersister", c_bindingFlags).With (page);
    }

    public string SetCollectionReadOnly (ControlCollection collection, string exceptionMessage)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      //  internal void System.Web.UI.ControlCollection.SetCollectionReadOnly
      return MethodCaller.CallFunc<string> ("SetCollectionReadOnly", c_bindingFlags).With (collection, exceptionMessage);
    }
  }
}