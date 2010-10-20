// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Diagnostics;
using System.Linq;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Parses a <see cref="DBBidirectionalRelationAttribute.SortExpression"/> into a <see cref="SortExpressionDefinition"/>.
  /// </summary>
  public class SortExpressionParser
  {
    private readonly ClassDefinition _classDefinition;
    private readonly IMappingNameResolver _mappingNameResolver;

    public SortExpressionParser (ClassDefinition classDefinition, IMappingNameResolver mappingNameResolver)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("mappingNameResolver", mappingNameResolver);

      _classDefinition = classDefinition;
      _mappingNameResolver = mappingNameResolver;
    }

    public SortExpressionDefinition Parse (string sortExpression)
    {
      ArgumentUtility.CheckNotNull ("sortExpression", sortExpression);

      try
      {
        var sortedProperties = from s in sortExpression.Split (',')
                               let specs = s.Trim()
                               where !string.IsNullOrEmpty (specs)
                               select ParseSortedPropertySpecification (specs);
        return new SortExpressionDefinition (sortedProperties);
      }
      catch (MappingException ex)
      {
        var message = string.Format ("SortExpression '{0}' cannot be parsed: {1}", sortExpression, ex.Message);
        throw new MappingException (message);
      }

    }

    private SortExpressionDefinition.SortedProperty ParseSortedPropertySpecification (string sortedPropertySpecification)
    {
      var splitSpecification = SplitSortedPropertySpecification (sortedPropertySpecification);

      var propertyDefinition = ParsePropertyName (splitSpecification.Item1);
      var sortOrder = ParseOrderSpecification (splitSpecification.Item2);

      return new SortExpressionDefinition.SortedProperty (propertyDefinition, sortOrder);
    }

    private Tuple<string, string> SplitSortedPropertySpecification (string sortedPropertySpecification)
    {
      var parts = sortedPropertySpecification.Split (' ').Where (s => !string.IsNullOrEmpty (s)).ToArray();

      if (parts.Length > 2)
      {
        var message = string.Format ("Expected one or two parts (a property name and an optional identifier), found {0} parts instead.", parts.Length);
        throw new MappingException (message);
      }

      return Tuple.Create (parts[0], parts.Length > 1 ? parts[1] : null);
    }

    private SortExpressionDefinition.SortOrder ParseOrderSpecification (string orderSpecification)
    {
      if (orderSpecification == null)
        return SortExpressionDefinition.SortOrder.Ascending;

      switch (orderSpecification.ToLower ())
      {
        case "asc":
          return SortExpressionDefinition.SortOrder.Ascending;
        case "desc":
          return SortExpressionDefinition.SortOrder.Descending;
        default:
          var message = string.Format ("'{0}' is not a valid sort order. Expected 'asc' or 'desc'.", orderSpecification);
          throw new MappingException (message);
      }
    }

    private PropertyDefinition ParsePropertyName (string propertyName)
    {
      var propertyDefinition = _classDefinition.GetPropertyDefinition (propertyName);
      if (propertyDefinition == null)
      {
        propertyDefinition = Maybe
            .ForValue (_classDefinition.ClassType.GetProperty (propertyName))
            .Select (pi => new PropertyInfoAdapter (pi))
            .Select (pi => _mappingNameResolver.GetPropertyName (pi))
            .Select (name => _classDefinition.GetPropertyDefinition (name))
            .ValueOrDefault();
      }

      if (propertyDefinition == null)
      {
        var message = string.Format (
            "'{0}' is not a valid mapped property name. Expected a full property identifier or a property name that can be resolved by performing "
            + "Reflection on the '{1}' class.",
            propertyName,
            _classDefinition.ClassType.Name);
        throw new MappingException (message);
      }
      return propertyDefinition;
    }
  }
}