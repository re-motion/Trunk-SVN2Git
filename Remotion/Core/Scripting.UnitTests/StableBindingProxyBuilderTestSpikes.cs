// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyBuilderTestSpikes : StableBindingProxyBuilderTest
  {
    [Test]
    [Explicit]
    public void BuildProxyType_FirstKnownBaseTypeThenKnownInterfaces_ProxiedChild ()
    {
      var knownBaseType = typeof (Proxied);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { knownBaseType });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("BuildProxyType_FirstKnownBaseTypeThenKnownInterfaces_ProxiedChild"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType_FirstKnownBaseTypeThenKnownInterfaces ();

      //Assert.That(GetAnyInstanceMethod (proxyType, "GetName"),Is.Not.Null);
      To.ConsoleLine.e (knownBaseType.GetMethods ()).nl (2).e (proxyType.GetMethods ());
    }

    [Test]
    [Explicit]
    public void BuildProxyType_FirstKnownBaseTypeThenKnownInterfaces_ProxiedChildChild ()
    {
      var knownBaseType = typeof (Proxied);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { knownBaseType, typeof (IPrependName), typeof (INotInProxied) });
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("BuildProxyType_FirstKnownBaseTypeThenKnownInterfaces_ProxiedChildChild"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType_FirstKnownBaseTypeThenKnownInterfaces ();

      //Assert.That(GetAnyInstanceMethod (proxyType, "GetName"),Is.Not.Null);
      var knownBaseTypeMethods = knownBaseType.GetMethods ();
      var proxyTypeMethods = proxyType.GetMethods ();
      To.ConsoleLine.e (knownBaseTypeMethods).nl (2).e (proxyTypeMethods);

      var proxied = new Proxied ();
      To.ConsoleLine.e (PrivateInvoke.InvokePublicMethod (proxied, "PrependName", "xyz"));

      var proxiedChildChild = new ProxiedChildChild ();
      var proxy = Activator.CreateInstance (proxyType, proxiedChildChild);
      To.ConsoleLine.e (PrivateInvoke.InvokePublicMethod (proxy, "PrependName", "xyz"));

      Assert.That (proxyTypeMethods.Select (m => m.Name).ToArray (), List.Contains ("NotInProxied"));
      To.ConsoleLine.e (PrivateInvoke.InvokePublicMethod (proxy, "NotInProxied"));
    }
  }
}