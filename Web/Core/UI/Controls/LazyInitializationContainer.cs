/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UI.Controls
{
  public class LazyInitializationContainer
  {
    private bool _isEnsured;
    private readonly PlaceHolder _placeHolder;

    public LazyInitializationContainer ()
    {
      _placeHolder = new PlaceHolder ();
    }

    public bool IsInitialized
    {
      get { return _isEnsured; }
    }

    public ControlCollection GetControls (ControlCollection baseControls)
    {
      ArgumentUtility.CheckNotNull ("baseControls", baseControls);
        if (_isEnsured)
          return baseControls;
        else
          return _placeHolder.Controls;
    }

    public void Ensure (ControlCollection baseControls)
    {
      ArgumentUtility.CheckNotNull ("baseControls", baseControls);

      if (_isEnsured)
        return;

      _isEnsured = true;

      List<Control> controls = new List<Control> (_placeHolder.Controls.Cast<Control> ());
      foreach (Control control in controls)
        baseControls.Add (control);
    }
  }
}