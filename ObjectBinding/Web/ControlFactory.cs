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
