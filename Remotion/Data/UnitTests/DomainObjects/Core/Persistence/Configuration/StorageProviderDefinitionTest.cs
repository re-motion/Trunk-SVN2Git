// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.Linq.Backend.DetailParsing.WhereConditionParsing;
using Remotion.Data.Linq.Backend.SqlGeneration;
using Remotion.Data.Linq.Backend.SqlGeneration.SqlServer;
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

    [Test]
    public void LinqSqlGenerator()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.LinqSqlGenerator, Is.InstanceOfType (typeof (SqlServerGenerator)));
    }

    [Test]
    public void LinqSqlGenerator_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<SqlServerGenerator> ().AddMixin<object> ().EnterScope ())
      {
        var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
        Assert.That (providerDefinition.LinqSqlGenerator, Is.InstanceOfType (typeof (SqlServerGenerator)));
        Assert.That (Mixin.Get<object> (providerDefinition.LinqSqlGenerator), Is.Not.Null);
      }
    }

    [Test]
    public void ResetLinqSqlGenerator ()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      ISqlGenerator generator = providerDefinition.LinqSqlGenerator;
      providerDefinition.ResetLinqSqlGenerator ();

      ISqlGenerator generator2 = providerDefinition.LinqSqlGenerator;

      Assert.That (generator2, Is.Not.Null);
      Assert.That (generator2, Is.Not.SameAs (generator));
    }
  }
}