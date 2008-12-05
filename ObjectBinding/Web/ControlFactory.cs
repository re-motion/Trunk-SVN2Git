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
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web
{

/// <summary>
///   Creates default controls for <see cref="IBusinessObjectProperty"/> properties.
/// </summary>
public sealed class ControlFactory
{
  public enum EditMode
  {
    Read,
    Edit,
    InlineEdit
  }

  public static IBusinessObjectBoundWebControl CreateControl (IBusinessObjectProperty property)
  {
    return CreateControl (property, EditMode.Edit);
  }

  /// <summary>
  ///   Creates a control for the specified property.
  /// </summary>
  /// <param name="property"> The business object property. </param>
  /// <param name="editMode"> Specifies how the control will be used. Some controls types 
  ///   do not support all edit modes. Default is <see cref="EditMode.Edit"/>.</param>
  /// <returns> 
  ///   A new, uninitialized business object bound control that can handle the specified property. 
  /// </returns>
  public static IBusinessObjectBoundWebControl CreateControl (IBusinessObjectProperty property, EditMode editMode)
  {
    if (! property.IsList)
    {
      if (property is IBusinessObjectStringProperty || property is IBusinessObjectNumericProperty)
        return new BocTextValue();
      else if (property is IBusinessObjectDateTimeProperty)
        return new BocDateTimeValue();
      else if (property is IBusinessObjectBooleanProperty)
        return new BocBooleanValue();
      else if (property is IBusinessObjectEnumerationProperty)
        return new BocEnumValue();
      else if (property is IBusinessObjectReferenceProperty && ((IBusinessObjectReferenceProperty)property).ReferenceClass is IBusinessObjectClassWithIdentity)
        return new BocReferenceValue();
    }
    else
    {
      if (property is IBusinessObjectStringProperty)
        return new BocMultilineTextValue();
      else if (property is IBusinessObjectReferenceDataSource && editMode != EditMode.InlineEdit)
        return new BocList();
    }

    return null;
  }

	private ControlFactory()
	{
	}
}

}
