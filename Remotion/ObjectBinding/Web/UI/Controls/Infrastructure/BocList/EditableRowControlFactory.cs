// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
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
      if (columnIndex < 0)
        throw new ArgumentOutOfRangeException ("columnIndex");

      IBusinessObjectBoundEditableWebControl control = column.CreateEditModeControl();

      if (control == null)
        control = CreateFromPropertyPath (column.GetPropertyPath());

      return control;
    }

    protected virtual IBusinessObjectBoundEditableWebControl CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      return (IBusinessObjectBoundEditableWebControl) ControlFactory.CreateControl (propertyPath.LastProperty, ControlFactory.EditMode.InlineEdit);
    }

    public virtual void RegisterHtmlHeadContents (HttpContext context)
    {
      var bocBooleanValue = new Controls.BocBooleanValue();
      bocBooleanValue.RegisterHtmlHeadContents (context);

      var bocDateTimeValue = new BocDateTimeValue();
      bocDateTimeValue.RegisterHtmlHeadContents (context);

      var bocEnumValue = new BocEnumValue();
      bocEnumValue.RegisterHtmlHeadContents (context);

      var bocMultilineTextValue = new BocMultilineTextValue();
      bocMultilineTextValue.RegisterHtmlHeadContents (context);

      var bocReferenceValue = new BocReferenceValue();
      bocReferenceValue.RegisterHtmlHeadContents (context);

      var bocTextValue = new BocTextValue();
      bocTextValue.RegisterHtmlHeadContents (context);
    }
  }
}