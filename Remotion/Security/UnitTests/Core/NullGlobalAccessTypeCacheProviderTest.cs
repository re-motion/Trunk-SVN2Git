// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullGlobalAccessTypeCacheProviderTest
  {
    private IGlobalAccessTypeCacheProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new NullGlobalAccessTypeCacheProvider();
    }

    [Test]
    public void Initialize()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new NullGlobalAccessTypeCacheProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
    
    [Test]
    public void GetAccessTypeCache ()
    {
      Assert.IsInstanceOf (typeof (NullCache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>), _provider.GetCache ());
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsTrue (_provider.IsNull);
    }

    [Test]
    public void SerializeInstanceNotInConfiguration ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      NullGlobalAccessTypeCacheProvider provider = new NullGlobalAccessTypeCacheProvider ("MyProvider", config);
      NullGlobalAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);

      Assert.AreEqual ("MyProvider", deserializedProvider.Name);
      Assert.AreEqual ("The Description", deserializedProvider.Description);
      Assert.IsInstanceOf (typeof (NullCache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>), deserializedProvider.GetCache ());
      Assert.IsTrue (((IGlobalAccessTypeCacheProvider) deserializedProvider).IsNull);
    }

    [Test]
    public void SerializeInstanceFromConfiguration ()
    {
      NullGlobalAccessTypeCacheProvider provider =
          (NullGlobalAccessTypeCacheProvider) SecurityConfiguration.Current.GlobalAccessTypeCacheProviders["None"];

      NullGlobalAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);
      Assert.AreSame (provider, deserializedProvider);
    }
  }
}
