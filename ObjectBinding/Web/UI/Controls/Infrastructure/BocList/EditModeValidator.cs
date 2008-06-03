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
using System.ComponentModel;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

[ToolboxItem (false)]
public class EditModeValidator : CustomValidator
{
  // types

  // static members and constants

  // member fields
  private Controls.BocList _owner;

  // construction and disposing

  public EditModeValidator (Controls.BocList owner)
  {
    _owner = owner;
  }

  // methods and properties

  protected override bool EvaluateIsValid()
  {
    return _owner.ValidateEditableRows();
  }

  protected override bool ControlPropertiesValid()
  {
    string controlToValidate = ControlToValidate;
    if (StringUtility.IsNullOrEmpty (controlToValidate))
      return base.ControlPropertiesValid();
    else
      return NamingContainer.FindControl (controlToValidate) == _owner;
  }
}

}
