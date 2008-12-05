// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

public class EditableRowControlFactory
{
  public EditableRowControlFactory ()
  {
  }

  public virtual IBusinessObjectBoundEditableWebControl Create (BocSimpleColumnDefinition column, int columnIndex)
  {
    ArgumentUtility.CheckNotNull ("column", column);
    if (columnIndex < 0) throw new ArgumentOutOfRangeException ("columnIndex");

    IBusinessObjectBoundEditableWebControl control = column.CreateEditModeControl();

    if (control == null)
    {
      IBusinessObjectPropertyPath propertyPath = column.GetPropertyPath ();
      control = (IBusinessObjectBoundEditableWebControl) ControlFactory.CreateControl (propertyPath.LastProperty, ControlFactory.EditMode.InlineEdit);
    }

    return control;
  }

  public virtual void RegisterHtmlHeadContents (HttpContext context)
  {
    var bocBooleanValue = new Controls.BocBooleanValue ();
    bocBooleanValue.RegisterHtmlHeadContents (context);

    var bocDateTimeValue = new BocDateTimeValue();
    bocDateTimeValue.RegisterHtmlHeadContents (context);

    var bocEnumValue = new BocEnumValue();
    bocEnumValue.RegisterHtmlHeadContents (context);

    var bocMultilineTextValue = new BocMultilineTextValue ();
    bocMultilineTextValue.RegisterHtmlHeadContents (context);

    var bocReferenceValue = new BocReferenceValue();
    bocReferenceValue.RegisterHtmlHeadContents (context);

    var bocTextValue = new BocTextValue();
    bocTextValue.RegisterHtmlHeadContents (context);
  }
}

}
