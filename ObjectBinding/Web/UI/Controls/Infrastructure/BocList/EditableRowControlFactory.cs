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
