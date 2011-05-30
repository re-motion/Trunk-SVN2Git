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
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class RevisionBasedAccessTypeCacheProviderTest
  {
    private MockRepository _mocks;
    private IRevisionBasedSecurityProvider _mockSecurityProvider;
    private IGlobalAccessTypeCacheProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _provider = new RevisionBasedAccessTypeCacheProvider();

      _mocks = new MockRepository();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
      _mockSecurityProvider = _mocks.StrictMock<IRevisionBasedSecurityProvider>();
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
      SafeContext.Instance.SetData (SafeContextKeys.SecurityRevisionBasedAccessTypeCacheProviderRevision, null);
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new RevisionBasedAccessTypeCacheProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetCache ()
    {
      Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
      _mocks.ReplayAll();

      ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> actual = _provider.GetCache ();

      _mocks.VerifyAll();
      Assert.IsNotNull (actual);
    }

    [Test]
    public void GetCache_SameCacheTwice ()
    {
      Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
      _mocks.ReplayAll();

      ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> expected = _provider.GetCache ();
      ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> actual = _provider.GetCache ();

      _mocks.VerifyAll();
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void GetCache_InvalidateFromOtherThread ()
    {
      using (_mocks.Ordered())
      {
        Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
        Expect.Call (_mockSecurityProvider.GetRevision()).Return (1);
      }
      _mocks.ReplayAll();

      ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> expected = _provider.GetCache ();
      ICache<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]> actual = null;

      ThreadRunner.Run (delegate { actual = _provider.GetCache(); });

      _mocks.VerifyAll();
      Assert.AreNotSame (expected, actual);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage =
            "The 'Remotion.Security.RevisionBasedAccessTypeCacheProvider' requires a security provider implementing the "
            + "'Remotion.Security.IRevisionBasedSecurityProvider' interface, but the 'Remotion.Security.NullSecurityProvider' only implements the "
            + "'System.IServiceProvider' interface. This exception might be caused if the security provider is set to 'None' but the "
            + "global accesstype-cache provider is still configured for revision based caching.")]
    public void GetCache_WithNullSecurityProvider ()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider();

      _provider.GetCache();
    }

    [Test]
    public void GetIsNull ()
    {
      Assert.IsFalse (_provider.IsNull);
    }

    [Test]
    public void SerializeInstanceNotInConfiguration ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      RevisionBasedAccessTypeCacheProvider provider = new RevisionBasedAccessTypeCacheProvider ("MyProvider", config);
      SecurityContext securityContext = SecurityContext.CreateStateless(typeof (SecurableObject));
      AccessType[] accessTypes = new[] { AccessType.Get (TestAccessTypes.Fifth) };
      ISecurityPrincipal securityPrincipal = new SecurityPrincipal ("foo", null, null, null);
      provider.GetCache().GetOrCreateValue (Tuple.Create ((ISecurityContext) securityContext, securityPrincipal), delegate { return accessTypes; });

      RevisionBasedAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);

      Assert.AreEqual ("MyProvider", deserializedProvider.Name);
      Assert.AreEqual ("The Description", deserializedProvider.Description);
      Assert.IsInstanceOf (typeof (LockingCacheDecorator<Tuple<ISecurityContext, ISecurityPrincipal>, AccessType[]>), deserializedProvider.GetCache ());
      Assert.AreNotSame (provider.GetCache(), deserializedProvider.GetCache());
      Assert.IsFalse (((IGlobalAccessTypeCacheProvider) deserializedProvider).IsNull);

      AccessType[] newAccessTypes;
      bool result = deserializedProvider.GetCache().TryGetValue (Tuple.Create ((ISecurityContext) securityContext, securityPrincipal), out newAccessTypes);
      Assert.IsTrue (result);
      Assert.AreNotSame (accessTypes, newAccessTypes);
      Assert.AreEqual (1, newAccessTypes.Length);
      Assert.That (newAccessTypes, Is.EquivalentTo (accessTypes));
    }

    [Test]
    public void SerializeInstanceFromConfiguration ()
    {
      var provider = (RevisionBasedAccessTypeCacheProvider) SecurityConfiguration.Current.GlobalAccessTypeCacheProviders["RevisionBased"];

      RevisionBasedAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);
      Assert.AreSame (provider, deserializedProvider);
    }
  }
}
