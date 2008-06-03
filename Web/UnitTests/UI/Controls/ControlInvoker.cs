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
using System.Web.UI;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Web.UnitTests.UI.Controls
{

public class ControlInvoker
{
  // types

  // static members and constants

  // member fields
  
  private Control _control;

  // construction and disposing

  public ControlInvoker (Control control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    _control = control;
  }

  // methods and properties

  public Control Control
  {
    get { return _control; }
  }

  public void InitRecursive ()
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "InitRecursive", new object[] { null });
  }

  public void LoadRecursive ()
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "LoadRecursive", new object[0]);
  }

  public void PreRenderRecursive ()
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "PreRenderRecursiveInternal", new object[0]);
  }

  public void LoadViewStateRecursive (object viewState)
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "LoadViewStateRecursive", new object[] { viewState });
  }

  public object SaveViewStateRecursive ()
  {
    return PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "SaveViewStateRecursive", new object[0]);
  }

  public void LoadViewState (object viewState)
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "LoadViewState", new object[] { viewState });
  }

  public object SaveViewState ()
  {
    return PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "SaveViewState", new object[0]);
  }

  public void LoadControlState (object controlState)
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "LoadControlState", new object[] { controlState });
  }

  public void LoadControlStateInternal (object controlState)
  {
    PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "LoadControlStateInternal", new object[] { controlState });
  }

  public object SaveControlState ()
  {
    return PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "SaveControlState", new object[0]);
  }

  public object SaveControlStateInternal ()
  {
    return PrivateInvoke.InvokeNonPublicMethod (_control, typeof (Control), "SaveControlStateInternal", new object[0]);
  }
}

}
