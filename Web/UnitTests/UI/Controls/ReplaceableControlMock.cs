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
using Remotion.Collections;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;

namespace Remotion.Web.UnitTests.UI.Controls
{
  public class ReplaceableControlMock : ControlMock, INamingContainer, IReplaceableControl
  {
    private readonly LazyInitializationContainer _lazyInitializationContainer = new LazyInitializationContainer();

    protected override void OnInit (EventArgs e)
    {
      Init += delegate { Assertion.IsNotNull (Page, "Page was null."); };

      if (Replacer == null)
      {
        OnInitParameters.A.ReplaceAndWrap (this, this, OnInitParameters.B);

        if (IsInitialized)
        {
          EnsureLazyInitializationContainer();
          base.OnInit (e);
        }
      }
      else
      {
        EnsureLazyInitializationContainer ();
        base.OnInit (e);
      }
    }

    public void EnsureLazyInitializationContainer ()
    {
      _lazyInitializationContainer.Ensure (base.Controls);
    }

    public override ControlCollection Controls
    {
      get { return _lazyInitializationContainer.GetControls (base.Controls); }
    }

    public bool IsInitialized
    {
      get { return _lazyInitializationContainer.IsInitialized; }
    }

    public ControlReplacer Replacer { get; set; }

    public Tuple<ControlReplacer, IModificationStateSelectionStrategy> OnInitParameters { get; set; }
  }
}