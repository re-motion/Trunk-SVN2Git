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
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  /// <summary>
  /// Default implementation of the <seealso cref="IInternalControlMemberCaller"/> interface.
  /// </summary>
  public class InternalControlMemberCaller : IInternalControlMemberCaller
  {
    private const BindingFlags c_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    public static readonly Type InternalControlStateType = typeof (Control).Assembly.GetType ("System.Web.UI.ControlState", true, false);

    //  private System.Web.UI.UpdatePanel._rendered
    private static readonly FieldInfo s_updatePanelRenderedFieldInfo = typeof (UpdatePanel).GetField ("_rendered", c_bindingFlags);

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

      var inheritedViewState = target.CreateSequence (c => c.Parent)
                                     .Select (c => (ViewStateMode?) c.ViewStateMode)
                                     .FirstOrDefault (m => m != ViewStateMode.Inherit) ?? ViewStateMode.Enabled;

      //  internal object System.Web.UI.Control.LoadViewStateRecursive()
      return MethodCaller.CallFunc<object> ("SaveViewStateRecursive", c_bindingFlags).With (target, inheritedViewState);
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
    public IDictionary SaveChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //  private ControlSet System.Web.UI.Page._registeredControlsRequiringControlState
      var registeredControlsRequiringControlStateFieldInfo = typeof (Page).GetField ("_registeredControlsRequiringControlState", c_bindingFlags);
      var registeredControlsRequiringControlState = (ICollection) registeredControlsRequiringControlStateFieldInfo.GetValue (control.Page);

      //LosFormatter only supports Hashtable and HybridDictionary without using native serialization
      var dictionary = new HybridDictionary ();
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
    public IDictionary GetChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer: Control, INamingContainer
    {
      ArgumentUtility.CheckNotNull ("control", control);

      //LosFormatter only supports Hashtable and HybridDictionary without using native serialization
      var childControlState = new HybridDictionary ();

      var pageStatePersister = GetPageStatePersister (control.Page);
      var controlStates = (IDictionary) pageStatePersister.ControlState;

      var parentPrefix = control.UniqueID + control.Page.IdSeparator;
      foreach (string key in controlStates.Keys)
      {
        if (key.StartsWith (parentPrefix))
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
      MethodCaller.CallAction ("ClearChildControlState", c_bindingFlags).With (control);
    }

    /// <summary>Encapsulates the get-access the the <see cref="Page"/>'s PageStatePersister property.</summary>
    public PageStatePersister GetPageStatePersister (Page page)
    {
      ArgumentUtility.CheckNotNull ("page", page);

      //  protected PageStatePersister System.Web.UI.Page.PageStatePersister
      return MethodCaller.CallFunc<PageStatePersister> ("get_PageStatePersister", c_bindingFlags).With (page);
    }

    public string SetCollectionReadOnly (ControlCollection collection, string exceptionMessage)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      //  internal void System.Web.UI.ControlCollection.SetCollectionReadOnly
      return MethodCaller.CallFunc<string> ("SetCollectionReadOnly", c_bindingFlags).With (collection, exceptionMessage);
    }

    /// <summary>Calls the <b>RenderChildrenInternal</b> method of the <see cref="Control"/>.</summary>
    public void RenderChildrenInternal (Control control, HtmlTextWriter writer, ICollection controls)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("writer", writer);
      ArgumentUtility.CheckNotNull ("controls", controls);

      //  internal void System.Web.UI.Control.RenderChildrenInternal
      MethodCaller.CallAction ("RenderChildrenInternal", c_bindingFlags).With (control, writer, controls);
    }

    /// <summary>Sets the <b>_rendered</b> flag of the <see cref="UpdatePanel"/>.</summary>
    public void SetUpdatePanelRendered (UpdatePanel updatePanel, bool value)
    {
      ArgumentUtility.CheckNotNull ("updatePanel", updatePanel);

      s_updatePanelRenderedFieldInfo.SetValue (updatePanel, value);
    }
  }
}
