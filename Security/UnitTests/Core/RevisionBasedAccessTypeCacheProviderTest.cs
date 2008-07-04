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
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Context;
using Rhino.Mocks;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;
using Remotion.Security.UnitTests.Core.Configuration;
using Remotion.Security.UnitTests.Core.SampleDomain;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class RevisionBasedAccessTypeCacheProviderTest
  {
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private IGlobalAccessTypeCacheProvider _provider;

    [SetUp]
    public void SetUp()
    {
      _provider = new RevisionBasedAccessTypeCacheProvider();

      _mocks = new MockRepository();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
    }

    [TearDown]
    public void TearDown()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      SafeContext.Instance.SetData (typeof (RevisionBasedAccessTypeCacheProvider).AssemblyQualifiedName + "_Revision", null);
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new RevisionBasedAccessTypeCacheProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }

    [Test]
    public void GetCache()
    {
      Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
      _mocks.ReplayAll();

      ICache<Tuple<ISecurityContext, string>, AccessType[]> actual = _provider.GetCache ();

      _mocks.VerifyAll();
      Assert.IsNotNull (actual);
    }

    [Test]
    public void GetCache_SameCacheTwice()
    {
      Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
      _mocks.ReplayAll();

      ICache<Tuple<ISecurityContext, string>, AccessType[]> expected = _provider.GetCache ();
      ICache<Tuple<ISecurityContext, string>, AccessType[]> actual = _provider.GetCache ();

      _mocks.VerifyAll();
      Assert.AreSame (expected, actual);
    }

    [Test]
    public void GetCache_InvalidateFromOtherThread()
    {
      using (_mocks.Ordered())
      {
        Expect.Call (_mockSecurityProvider.GetRevision()).Return (0);
        Expect.Call (_mockSecurityProvider.GetRevision()).Return (1);
      }
      _mocks.ReplayAll();

      ICache<Tuple<ISecurityContext, string>, AccessType[]> expected = _provider.GetCache ();
      ICache<Tuple<ISecurityContext, string>, AccessType[]> actual = null;

      ThreadRunner.Run (delegate { actual = _provider.GetCache(); });

      _mocks.VerifyAll();
      Assert.AreNotSame (expected, actual);
    }

    [Test]
    public void GetCache_WithNullSecurityProvider()
    {
      SecurityConfiguration.Current.SecurityProvider = new NullSecurityProvider();

      Assert.AreSame (_provider.GetCache(), _provider.GetCache());
    }

    [Test]
    public void GetIsNull()
    {
      Assert.IsFalse (_provider.IsNull);
    }

    [Test]
    public void SerializeInstanceNotInConfiguration ()
    {
      NameValueCollection config = new NameValueCollection ();
      config.Add ("description", "The Description");

      RevisionBasedAccessTypeCacheProvider provider = new RevisionBasedAccessTypeCacheProvider ("MyProvider", config);
      SecurityContext securityContext = new SecurityContext (typeof (SecurableObject));
      AccessType[] accessTypes = new AccessType[] { AccessType.Get (TestAccessTypes.Fifth) };
      provider.GetCache ().GetOrCreateValue (Tuple.NewTuple ((ISecurityContext) securityContext, "bla"), delegate { return accessTypes; });

      RevisionBasedAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);

      Assert.AreEqual ("MyProvider", deserializedProvider.Name);
      Assert.AreEqual ("The Description", deserializedProvider.Description);
      Assert.IsInstanceOfType (typeof (InterlockedCache<Tuple<ISecurityContext, string>, AccessType[]>), deserializedProvider.GetCache ());
      Assert.AreNotSame (provider.GetCache(), deserializedProvider.GetCache ());
      Assert.IsFalse (((IGlobalAccessTypeCacheProvider) deserializedProvider).IsNull);

      AccessType[] newAccessTypes;
      bool result = deserializedProvider.GetCache ().TryGetValue (Tuple.NewTuple ((ISecurityContext) securityContext, "bla"), out newAccessTypes);
      Assert.IsTrue (result);
      Assert.AreNotSame (accessTypes, newAccessTypes);
      Assert.AreEqual (1, newAccessTypes.Length);
      Assert.That (newAccessTypes, Is.EquivalentTo (accessTypes));
    }

    [Test]
    public void SerializeInstanceFromConfiguration ()
    {
      RevisionBasedAccessTypeCacheProvider provider =
          (RevisionBasedAccessTypeCacheProvider) SecurityConfiguration.Current.GlobalAccessTypeCacheProviders["RevisionBased"];
      
      RevisionBasedAccessTypeCacheProvider deserializedProvider = Serializer.SerializeAndDeserialize (provider);
      Assert.AreSame (provider, deserializedProvider);
    }
  }
}
