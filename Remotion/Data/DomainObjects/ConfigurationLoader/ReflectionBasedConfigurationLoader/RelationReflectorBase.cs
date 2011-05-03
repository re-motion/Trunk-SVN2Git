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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Base class for reflecting on the relations of a class.</summary>
  public abstract class RelationReflectorBase<T> : MemberReflectorBase where T: BidirectionalRelationAttribute
  {
    protected RelationReflectorBase (ClassDefinition classDefinition, PropertyInfo propertyInfo, IMappingNameResolver nameResolver)
        : base (propertyInfo, nameResolver)
    {
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

      ClassDefinition = classDefinition;
      BidirectionalRelationAttribute = AttributeUtility.GetCustomAttribute<T> (PropertyInfo, true);
    }

    public ClassDefinition ClassDefinition { get; private set; }
    public T BidirectionalRelationAttribute { get; private set; }

    protected bool IsBidirectionalRelation
    {
      get { return BidirectionalRelationAttribute != null; }
    }

    protected PropertyInfo GetOppositePropertyInfo ()
    {
      var type = ReflectionUtility.GetRelatedObjectTypeFromRelationProperty (PropertyInfo);
      var propertyFinder = new NameBasedPropertyFinder (
          BidirectionalRelationAttribute.OppositeProperty, 
          type, 
          true, 
          true, 
          NameResolver, 
          new PersistentMixinFinder (type, true));
      
      return propertyFinder.FindPropertyInfos().LastOrDefault();
    }
  }
}