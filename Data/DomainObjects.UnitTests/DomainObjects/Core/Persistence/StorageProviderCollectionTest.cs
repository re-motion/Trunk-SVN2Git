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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
{
  [TestFixture]
  public class StorageProviderCollectionTest : StandardMappingTest
  {
    private StorageProviderCollection _collection;
    private StorageProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _provider = new SqlProvider (new RdbmsProviderDefinition ("TestDomain", typeof (SqlProvider), "ConnectionString"));
      _collection = new StorageProviderCollection ();
    }

    [Test]
    public void ContainsProviderTrue ()
    {
      _collection.Add (_provider);
      Assert.IsTrue (_collection.Contains (_provider));
    }

    [Test]
    public void ContainsProviderFalse ()
    {
      _collection.Add (_provider);

      StorageProvider copy = new SqlProvider ((RdbmsProviderDefinition) _provider.Definition);
      Assert.IsFalse (_collection.Contains (copy));
    }

  }
}
