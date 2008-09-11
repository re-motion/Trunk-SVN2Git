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
using System.Web.UI;
using Remotion.Web.Utilities;

namespace Remotion.Web.Utilities
{
  public interface IInternalControlMemberCaller
  {
    void SetControlState (Control control, ControlState value);
    ControlState GetControlState (Control control);
    void InitRecursive (Control control, Control namingContainer);

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s LoadViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be restored. </param>
    /// <param name="viewState"> The view state object used for restoring. </param>
    void LoadViewStateRecursive (Control target, object viewState);

    /// <summary> Encapsulates the invocation of <see cref="Control"/>'s SaveViewStateRecursive method. </summary>
    /// <param name="target"> The <see cref="Control"/> to be saved. </param>
    /// <returns> The view state object for <paramref name="target"/>. </returns>
    object SaveViewStateRecursive (Control target);

    /// <summary>Encapsulates the invocation of <see cref="Page"/>'s SaveAllState method.</summary>
    /// <param name="page">The <see cref="Page"/> for which SaveAllState will be invoked. Must not be <see langword="null" />.</param>
    void SaveAllState (Page page);

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveChildControlState method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveChildControlState will be invoked. Must not be <see langword="null" />.</param>
    Dictionary<string, object> SaveChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer : Control, INamingContainer;

    /// <summary>Encapsulates the invocation of <see cref="Control"/>'s SaveControlStateInternal method.</summary>
    /// <param name="control">The <see cref="Control"/> for which SaveControlStateInternal will be invoked. Must not be <see langword="null" />.</param>
    object SaveControlStateInternal (Control control);

    /// <summary>Returns the control states for all controls that are child-controls of the passed <see cref="Control"/>.</summary>
    Dictionary<string, object> GetChildControlState<TNamingContainer> (TNamingContainer control)
        where TNamingContainer : Control, INamingContainer;

    /// <summary>Sets the control states for the child control of the passed <see cref="Control"/>.</summary>
    void SetChildControlState<TNamingContainer> (TNamingContainer control, IDictionary newControlState)
        where TNamingContainer : Control, INamingContainer;

    /// <summary>Encapsulates the get-access the the <see cref="Page"/>'s PageStatePersister property.</summary>
    PageStatePersister GetPageStatePersister (Page page);

    string SetCollectionReadOnly (ControlCollection collection, string exceptionMessage);
  }
}