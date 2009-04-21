// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
