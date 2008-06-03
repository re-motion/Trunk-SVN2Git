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
using System.Collections;
using System.ComponentModel;

namespace Remotion.Web.UI.Design
{

/// <summary>
///   Derive from this class to create a VS.NET designer pick list for a property that 
///   references another control.
/// </summary>
/// <remarks>
///   Call the base class constructor with the type of acceptable target controls. For additional
///   filtering, overload <see cref="IsValidTargetControl"/>. Use the <see cref="TypeConverter"/> attribute 
///   to assign a derived converter to a property.
/// </remarks>
public abstract class ControlToStringConverter : StringConverter
{
  Type _targetControlType;

  public ControlToStringConverter (Type targetControlType)
  {
    _targetControlType = targetControlType;
  }

  private object[] GetControls (IContainer container)
  {
    ArrayList idList = new ArrayList();
    foreach (IComponent component in container.Components)
    {
      // TODO: remove
//      Control control = component as Control;
//      if (control != null)
//      {
      if (_targetControlType.IsAssignableFrom (component.GetType()) && IsValidTargetControl (component))
        idList.Add (component.Site.Name);
//      }
    }
    idList.Sort(Comparer.Default);
    return idList.ToArray();
  }

  public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
  {
    if ((context == null) || (context.Container == null))
      return null; 

    object[] controls = GetControls (context.Container);
    if (controls != null)
      return new StandardValuesCollection (controls); 

    return null; 
  }

  public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
  {
    return false;
  }

  public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
  {
    return true;
  }

  /// <summary>
  /// Overload this method to defines whether a control should appear in the designer pick list. 
  /// </summary>
  public virtual bool IsValidTargetControl (IComponent control)
  {
    return true;
  }
}

}
