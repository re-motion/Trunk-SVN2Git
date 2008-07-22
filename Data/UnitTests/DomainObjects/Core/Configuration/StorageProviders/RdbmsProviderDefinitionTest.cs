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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration.StorageProviders;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.StorageProviders
{
  [TestFixture]
  public class RdbmsProviderDefinitionTest
  {
    [Test]
    public void Initialize()
    {
      RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");

      Assert.That (providerDefinition.Name, Is.EqualTo ("Provider"));
      Assert.That (providerDefinition.StorageProviderType, Is.SameAs (typeof (SqlProvider)));
      Assert.That (providerDefinition.ConnectionString, Is.EqualTo ("ConnectionString"));
    }

    [Test]
    public void GetTypeConversionProvider ()
    {
      RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeConversionProvider, Is.InstanceOfType (typeof (TypeConversionProvider)));
    }

    [Test]
    public void GetTypeProvider ()
    {
      RdbmsProviderDefinition providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeProvider, Is.InstanceOfType (typeof (TypeProvider)));
    }
  }
}
