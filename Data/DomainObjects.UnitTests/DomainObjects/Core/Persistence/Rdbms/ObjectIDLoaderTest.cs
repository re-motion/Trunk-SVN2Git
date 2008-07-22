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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class ObjectIDLoaderTest : SqlProviderBaseTest
  {
    private ObjectIDLoader _loader;

    public override void SetUp ()
    {
      base.SetUp ();
      Provider.Connect ();

      _loader = new ObjectIDLoader (Provider);
    }

    public override void TearDown ()
    {
      base.TearDown ();
      Provider.Disconnect ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (Provider, _loader.Provider);
    }

    [Test]
    public void LoadObjectIDsFromCommandBuilder ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      PropertyDefinition propertyDefinition = classDefinition.GetMandatoryPropertyDefinition (MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (OrderItem), "Order"));
      UnionSelectCommandBuilder builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, classDefinition, propertyDefinition, DomainObjectIDs.Order1);
      List<ObjectID> objectIDs = _loader.LoadObjectIDsFromCommandBuilder (builder);
      Assert.That (objectIDs, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));
    }
  }
}
