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
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.Oracle.CodeGenerator.UnitTests
{
  public class MappingBaseTest 
  {
    // types

    // static members and constants

    // member fields

    private StorageProviderConfiguration _storageProviderConfiguration;
    private MappingConfiguration _mappingConfiguration;

    private ClassDefinition _orderItemClass;
    private ClassDefinition _orderClass;
    private ClassDefinition _companyClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _partnerClass;
    private ClassDefinition _abstractWithoutConcreteClass;
    private ClassDefinition _concreteClass;
    private ClassDefinition _derivedClass;
    private ClassDefinition _secondDerivedClass;
    private ClassDefinition _derivedOfDerivedClass;
    private ClassDefinition _ceoClass;
    private ClassDefinition _classWithRelations;

    // construction and disposing

    public MappingBaseTest ()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public virtual void TextFixtureSetUp ()
    {
    }

    [SetUp]
    public virtual void SetUp ()
    {
      _storageProviderConfiguration = new StorageProviderConfiguration ("StorageProviders.xml");
      _mappingConfiguration = new MappingConfiguration ("Mapping.xml", false);

      _orderItemClass = MappingConfiguration.ClassDefinitions.GetMandatory ("OrderItem");
      _orderClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Order");
      _companyClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Company");
      _customerClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Customer");
      _partnerClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Partner");
      _abstractWithoutConcreteClass = MappingConfiguration.ClassDefinitions.GetMandatory ("AbstractWithoutConcreteClass");
      _concreteClass = MappingConfiguration.ClassDefinitions.GetMandatory ("ConcreteClass");
      _derivedClass = MappingConfiguration.ClassDefinitions.GetMandatory ("DerivedClass");
      _secondDerivedClass = MappingConfiguration.ClassDefinitions.GetMandatory ("SecondDerivedClass");
      _derivedOfDerivedClass = MappingConfiguration.ClassDefinitions.GetMandatory ("DerivedOfDerivedClass");
      _ceoClass = MappingConfiguration.ClassDefinitions.GetMandatory ("Ceo");
      _classWithRelations = MappingConfiguration.ClassDefinitions.GetMandatory ("ClassWithRelations");
    }

    [TearDown]
    public virtual void TearDown ()
    {
    }

    protected StorageProviderConfiguration StorageProviderConfiguration
    {
      get { return _storageProviderConfiguration; }
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }

    protected ClassDefinition OrderItemClass
    {
      get { return _orderItemClass; }
    }

    protected ClassDefinition OrderClass
    {
      get { return _orderClass; }
    }

    protected ClassDefinition CompanyClass
    {
      get { return _companyClass; }
    }

    protected ClassDefinition CustomerClass
    {
      get { return _customerClass; }
    }

    protected ClassDefinition PartnerClass
    {
      get { return _partnerClass; }
    }

    public ClassDefinition AbstractWithoutConcreteClass
    {
      get { return _abstractWithoutConcreteClass; }
    }

    protected ClassDefinition ConcreteClass
    {
      get { return _concreteClass; }
    }

    public ClassDefinition DerivedClass
    {
      get { return _derivedClass; }
    }

    protected ClassDefinition SecondDerivedClass
    {
      get { return _secondDerivedClass; }
    }

    protected ClassDefinition DerivedOfDerivedClass
    {
      get { return _derivedOfDerivedClass; }
    }

    protected ClassDefinition CeoClass
    {
      get { return _ceoClass; }
    }

    protected ClassDefinition ClassWithRelations
    {
      get { return _classWithRelations; }
    }
  }
}
