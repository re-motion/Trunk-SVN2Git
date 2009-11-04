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
