// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="ClassReflector"/> is used to build a <see cref="ClassDefinition"/>.
  /// </summary>
  /// <remarks>Derived classes must have a cosntructor with a matching the <see cref="ClassReflector"/>'s constructor signature. </remarks>
  public class ClassReflector
  {
    private readonly Type _type;
    private readonly IMappingObjectFactory _mappingObjectFactory;
    private readonly IMappingNameResolver _nameResolver;
    private readonly IClassIDProvider _classIDProvider;
    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;

    public ClassReflector (
        Type type,
        IMappingObjectFactory mappingObjectFactory,
        IMappingNameResolver nameResolver,
        IClassIDProvider classIDProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (DomainObject));
      ArgumentUtility.CheckNotNull ("mappingObjectFactory", mappingObjectFactory);
      ArgumentUtility.CheckNotNull ("nameResolver", nameResolver);
      ArgumentUtility.CheckNotNull ("classIDProvider", classIDProvider);
      ArgumentUtility.CheckNotNull ("domainModelConstraintProvider", domainModelConstraintProvider);

      _type = type;
      _mappingObjectFactory = mappingObjectFactory;
      _nameResolver = nameResolver;
      _classIDProvider = classIDProvider;
      _domainModelConstraintProvider = domainModelConstraintProvider;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IMappingObjectFactory MappingObjectFactory
    {
      get { return _mappingObjectFactory; }
    }

    public IMappingNameResolver NameResolver
    {
      get { return _nameResolver; }
    }

    public ClassDefinition GetMetadata (ClassDefinition baseClassDefinition)
    {
      var persistentMixinFinder = new PersistentMixinFinder (Type, baseClassDefinition == null);
      var classDefinition = new ClassDefinition (
          _classIDProvider.GetClassID(Type), Type, IsAbstract(), baseClassDefinition, GetStorageGroupType(), persistentMixinFinder);

      var properties = MappingObjectFactory.CreatePropertyDefinitionCollection (classDefinition, GetPropertyInfos (classDefinition));
      classDefinition.SetPropertyDefinitions (properties);
      var endPoints = MappingObjectFactory.CreateRelationEndPointDefinitionCollection (classDefinition);
      classDefinition.SetRelationEndPointDefinitions (endPoints);

      return classDefinition;
    }

    private Type GetStorageGroupType ()
    {
      var storageGroupAttribute = AttributeUtility.GetCustomAttributes<StorageGroupAttribute> (Type, true).FirstOrDefault();
      if (storageGroupAttribute != null)
        return storageGroupAttribute.GetType();
      return null;
    }

    private bool IsAbstract ()
    {
      if (Type.IsAbstract)
        return !Attribute.IsDefined (Type, typeof (InstantiableAttribute), false);

      return false;
    }

    private IEnumerable<IPropertyInformation> GetPropertyInfos (ClassDefinition classDefinition)
    {
      PropertyFinder propertyFinder = new PropertyFinder (
          classDefinition.ClassType,
          classDefinition,
          classDefinition.BaseClass == null,
          true,
          NameResolver,
          classDefinition.PersistentMixinFinder, 
          _domainModelConstraintProvider
          );
      return propertyFinder.FindPropertyInfos();
    }
  }
}