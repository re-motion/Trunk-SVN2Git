﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.SortExpressions
{
  /// <summary>
  /// Defines how a property is to be sorted.
  /// </summary>
  public sealed class SortedPropertySpecification
  {
    public readonly PropertyDefinition PropertyDefinition;
    public readonly SortOrder Order;

    public SortedPropertySpecification (PropertyDefinition propertyDefinition, SortOrder order)
    {
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);

      var underlyingType = propertyDefinition.IsPropertyTypeResolved
        ? Nullable.GetUnderlyingType (propertyDefinition.PropertyType) ?? propertyDefinition.PropertyType 
        : null;

      if (underlyingType != null && !typeof (IComparable).IsAssignableFrom (underlyingType))
      {
        var message = string.Format (
            "Cannot sort by property '{0}' - its property type ('{1}') does not implement IComparable.",
            propertyDefinition.PropertyName,
            propertyDefinition.PropertyType.Name);
        throw new MappingException (message);
      }

      PropertyDefinition = propertyDefinition;
      Order = order;
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;

      if (obj.GetType () != GetType ())
        return false;
      
      var other = (SortedPropertySpecification) obj;
      return PropertyDefinition == other.PropertyDefinition && Order == other.Order;
    }

    public override int GetHashCode ()
    {
      return (PropertyDefinition.GetHashCode () << 1) ^ Order.GetHashCode();
    }

    public override string ToString ()
    {
      return PropertyDefinition.PropertyName + " " + (Order == SortOrder.Ascending ? "ASC" : "DESC");
    }

    public IComparer<T> GetComparer<T> (Func<T, PropertyDefinition, object> valueGetter)
    {
      ArgumentUtility.CheckNotNull ("valueGetter", valueGetter);

      return new InternalComparer<T> (PropertyDefinition, Order, valueGetter);
    }

    [Serializable]
    private class InternalComparer<T> : IComparer<T>
    {
      private readonly PropertyDefinition _propertyDefinition;
      private readonly SortOrder _order;
      private readonly Func<T, PropertyDefinition, object> _valueGetter;

      public InternalComparer (PropertyDefinition propertyDefinition, SortOrder order, Func<T, PropertyDefinition, object> valueGetter)
      {
        ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
        ArgumentUtility.CheckNotNull ("valueGetter", valueGetter);

        _propertyDefinition = propertyDefinition;
        _order = order;
        _valueGetter = valueGetter;
      }

      public int Compare (T x, T y)
      {
        var valueX = _valueGetter (x, _propertyDefinition);
        var valueY = _valueGetter (y, _propertyDefinition);

        if (_order == SortOrder.Ascending)
          return Comparer<object>.Default.Compare (valueX, valueY);
        else
          return -Comparer<object>.Default.Compare (valueX, valueY);
      }
    }
  }
}