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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Configuration
{
  [TestFixture]
  public class StorageProviderDefinitionTest
  {
    [Test]
    public void Initialize()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");

      Assert.That (providerDefinition.Name, Is.EqualTo ("Provider"));
      Assert.That (providerDefinition.StorageProviderType, Is.SameAs (typeof (SqlProvider)));
      Assert.That (providerDefinition.ConnectionString, Is.EqualTo ("ConnectionString"));
    }

    [Test]
    public void GetTypeConversionProvider ()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeConversionProvider, Is.InstanceOfType (typeof (TypeConversionProvider)));
    }

    [Test]
    public void GetTypeProvider ()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeProvider, Is.InstanceOfType (typeof (TypeProvider)));
    }

  }
}
