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
using Remotion.Utilities;

namespace Remotion.Development.Web.UnitTesting.AspNetFramework
{
  public class ControlMock : Control
  {
    // types

    // static members and constants

    // member fields

    private string _valueInViewState;
    private string _valueInControlState;

    // construction and disposing

    public ControlMock ()
    {
    }

    // methods and properties

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      Assertion.IsNotNull (Page, "Page is null for control '{0}'", ID);
      Page.RegisterRequiresControlState (this);
    }

    public string ValueInViewState
    {
      get { return _valueInViewState; }
      set { _valueInViewState = value; }
    }

    public string ValueInControlState
    {
      get { return _valueInControlState; }
      set { _valueInControlState = value; }
    }
    
    protected override void LoadViewState (object savedState)
    {
      _valueInViewState = (string) savedState;
    }

    protected override object SaveViewState()
    {
      return _valueInViewState;
    }

    protected override void LoadControlState (object savedState)
    {
      _valueInControlState = (string) savedState;
    }
  
    protected override object SaveControlState ()
    {
      return _valueInControlState;
    }

    protected override void Render (HtmlTextWriter writer)
    {
      writer.RenderBeginTag( HtmlTextWriterTag.Div);
      writer.Write ("ValueInViewState: {0}", _valueInViewState);
      writer.WriteBreak();
      writer.Write ("ValueInControlState: {0}", _valueInControlState);
      writer.RenderEndTag ();
    }
  }
}