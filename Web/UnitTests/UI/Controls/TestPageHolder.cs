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
using System.Collections.Specialized;
using System.Web.UI;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls
{
  public class TestPageHolder
  {
    private readonly PageMock _page;
    private readonly ControlInvoker _pageInvoker;
    private readonly ReplaceableControlMock _namingContainer;
    private readonly ControlMock _parent;
    private readonly ControlMock _child;
    private readonly Control _child2;
    private readonly ControlMock _otherControl;
    private readonly NamingContainerMock _otherNamingContainer;

    public TestPageHolder (bool initializeState, RequestMode requestMode)
    {
      _page = new PageMock ();
      if (requestMode == RequestMode.PostBack)
        _page.SetRequestValueCollection (new NameValueCollection ());

      _otherNamingContainer = new NamingContainerMock ();
      _otherNamingContainer.ID = "OtherNamingContainer";
      _page.Controls.Add (_otherNamingContainer);

      _otherControl = new ControlMock ();
      _otherControl.ID = "OtherControl";
      _otherNamingContainer.Controls.Add (_otherControl);

      _namingContainer = new ReplaceableControlMock();
      _namingContainer.ID = "NamingContainer";
      _page.Controls.Add (_namingContainer);

      _parent = new ControlMock ();
      _parent.ID = "Parent";
      _namingContainer.Controls.Add (_parent);

      _child = new ControlMock ();
      _child.ID = "Child";
      _parent.Controls.Add (_child);

      _child2 = new Control ();
      _child2.ID = "Child2";
      _parent.Controls.Add (_child2);

      _pageInvoker = new ControlInvoker (_page);

      if (initializeState)
      {
        _parent.ValueInViewState = "ParentValue";
        _parent.ValueInControlState = "ParentValue";

        _namingContainer.ValueInViewState = "NamingContainerValue";
        _namingContainer.ValueInControlState = "NamingContainerValue";

        _otherControl.ValueInViewState = "OtherValue";
        _otherControl.ValueInControlState = "OtherValue";
      }

      Page.RegisterViewStateHandler ();
    }

    public PageMock Page
    {
      get { return _page; }
    }

    public ControlInvoker PageInvoker
    {
      get { return _pageInvoker; }
    }

    public ReplaceableControlMock NamingContainer
    {
      get { return _namingContainer; }
    }

    public ControlMock Parent
    {
      get { return _parent; }
    }

    public ControlMock Child
    {
      get { return _child; }
    }

    public Control Child2
    {
      get { return _child2; }
    }

    public ControlMock OtherControl
    {
      get { return _otherControl; }
    }

    public NamingContainerMock OtherNamingContainer
    {
      get { return _otherNamingContainer; }
    }
  }
}