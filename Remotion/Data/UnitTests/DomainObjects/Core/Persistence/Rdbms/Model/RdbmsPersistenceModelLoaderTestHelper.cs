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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.RdbmsPersistenceModelLoaderTestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  // Test Domain:
  //
  //                 BaseBase
  //                     |
  //                   Base
  //                 /      \
  //            Table1       Table2
  //                         /    \
  //                   Derived1  Derived2
  //                                |
  //                          DerivedDerived
  //                                |
  //                       DerivedDerivedDerived
  //
  // All Base classes are persisted as UnionViewDefinitions, all Tables as TableDefinitions, all Derived as FilterViewDefinitions.
  
  public class RdbmsPersistenceModelLoaderTestHelper
  {
    private readonly ClassDefinition _baseBaseClassDefinition;
    private readonly ClassDefinition _baseClassDefinition;
    private readonly ClassDefinition _tableClassDefinition1;
    private readonly ClassDefinition _tableClassDefinition2;
    private readonly ClassDefinition _derivedClassDefinition1;
    private readonly ClassDefinition _derivedClassDefinition2;
    private readonly ClassDefinition _derivedDerivedClassDefinition;
    private readonly ClassDefinition _derivedDerivedDerivedClassDefinition;
    private readonly ReflectionBasedPropertyDefinition _baseBasePropertyDefinition;
    private readonly ReflectionBasedPropertyDefinition _basePropertyDefinition;
    private readonly ReflectionBasedPropertyDefinition _tablePropertyDefinition1;
    private readonly ReflectionBasedPropertyDefinition _tablePropertyDefinition2;
    private readonly ReflectionBasedPropertyDefinition _derivedPropertyDefinition1;
    private readonly ReflectionBasedPropertyDefinition _derivedPropertyDefinition2;
    private readonly ReflectionBasedPropertyDefinition _derivedDerivedPropertyDefinition;

    public RdbmsPersistenceModelLoaderTestHelper ()
    {
      _baseBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (typeof (BaseBaseClass), null);
      _baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (BaseClass), _baseBaseClassDefinition);
      _tableClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Table1Class), _baseClassDefinition);
      _tableClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Table2Class), _baseClassDefinition);
      _derivedClassDefinition1 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Derived1Class), _tableClassDefinition2);
      _derivedClassDefinition2 = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (Derived2Class), _tableClassDefinition2);
      _derivedDerivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (DerivedDerivedClass), _derivedClassDefinition2);
      _derivedDerivedDerivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinitionWithoutStorageEntity (
          typeof (DerivedDerivedDerivedClass), _derivedDerivedClassDefinition);

      _baseBaseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _baseClassDefinition }, true, true));
      _baseClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _tableClassDefinition1, _tableClassDefinition2 }, true, true));
      _tableClassDefinition2.SetDerivedClasses (
          new ClassDefinitionCollection (new[] { _derivedClassDefinition1, _derivedClassDefinition2 }, true, true));
      _derivedClassDefinition2.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedDerivedClassDefinition }, true, true));
      _tableClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedClassDefinition1.SetDerivedClasses (new ClassDefinitionCollection());
      _derivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection (new[] { _derivedDerivedDerivedClassDefinition }, true, true));
      _derivedDerivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinitionCollection());

      _baseBaseClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _baseClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _tableClassDefinition1.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _tableClassDefinition2.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedClassDefinition1.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedClassDefinition2.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedDerivedClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedDerivedDerivedClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      
      _baseBasePropertyDefinition = CreateAndAddPropertyDefinition (
          _baseBaseClassDefinition, "BaseBaseProperty", typeof (BaseBaseClass).GetProperty ("BaseBaseProperty"));
      _basePropertyDefinition = CreateAndAddPropertyDefinition (_baseClassDefinition, "BaseProperty", typeof (BaseClass).GetProperty ("BaseProperty"));
      _tablePropertyDefinition1 = CreateAndAddPropertyDefinition (
          _tableClassDefinition1, "TableProperty1", typeof (Table1Class).GetProperty ("TableProperty1"));
      _tablePropertyDefinition2 = CreateAndAddPropertyDefinition (
          _tableClassDefinition2, "TableProperty2", typeof (Table2Class).GetProperty ("TableProperty2"));
      _derivedPropertyDefinition1 = CreateAndAddPropertyDefinition (
          _derivedClassDefinition1, "DerivedProperty1", typeof (Derived1Class).GetProperty ("DerivedProperty1"));
      _derivedPropertyDefinition2 = CreateAndAddPropertyDefinition (
          _derivedClassDefinition2, "DerivedProperty2", typeof (Derived2Class).GetProperty ("DerivedProperty2"));
      _derivedDerivedPropertyDefinition = CreateAndAddPropertyDefinition (
          _derivedDerivedClassDefinition, "DerivedDerivedProperty", typeof (DerivedDerivedClass).GetProperty ("DerivedDerivedProperty"));
      _derivedDerivedDerivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
    }

    public ClassDefinition BaseBaseClassDefinition
    {
      get { return _baseBaseClassDefinition; }
    }

    public ClassDefinition BaseClassDefinition
    {
      get { return _baseClassDefinition; }
    }

    public ClassDefinition TableClassDefinition1
    {
      get { return _tableClassDefinition1; }
    }

    public ClassDefinition TableClassDefinition2
    {
      get { return _tableClassDefinition2; }
    }

    public ClassDefinition DerivedClassDefinition1
    {
      get { return _derivedClassDefinition1; }
    }

    public ClassDefinition DerivedClassDefinition2
    {
      get { return _derivedClassDefinition2; }
    }

    public ClassDefinition DerivedDerivedClassDefinition
    {
      get { return _derivedDerivedClassDefinition; }
    }

    public ClassDefinition DerivedDerivedDerivedClassDefinition
    {
      get { return _derivedDerivedDerivedClassDefinition; }
    }

    public ReflectionBasedPropertyDefinition BaseBasePropertyDefinition
    {
      get { return _baseBasePropertyDefinition; }
    }

    public ReflectionBasedPropertyDefinition BasePropertyDefinition
    {
      get { return _basePropertyDefinition; }
    }

    public ReflectionBasedPropertyDefinition TablePropertyDefinition1
    {
      get { return _tablePropertyDefinition1; }
    }

    public ReflectionBasedPropertyDefinition TablePropertyDefinition2
    {
      get { return _tablePropertyDefinition2; }
    }

    public ReflectionBasedPropertyDefinition DerivedPropertyDefinition1
    {
      get { return _derivedPropertyDefinition1; }
    }

    public ReflectionBasedPropertyDefinition DerivedPropertyDefinition2
    {
      get { return _derivedPropertyDefinition2; }
    }

    public ReflectionBasedPropertyDefinition DerivedDerivedPropertyDefinition
    {
      get { return _derivedDerivedPropertyDefinition; }
    }

    private ReflectionBasedPropertyDefinition CreateAndAddPropertyDefinition (
        ClassDefinition classDefinition, string propertyName, PropertyInfo propertyInfo)
    {
      var propertyDefinition = ReflectionBasedPropertyDefinitionFactory.Create (
          classDefinition,
          propertyName,
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      return propertyDefinition;
    }
  }
}