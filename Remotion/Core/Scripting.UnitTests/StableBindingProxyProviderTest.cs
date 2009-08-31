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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyProviderTest
  {
    private readonly ScriptContext _scriptContext = ScriptContext.Create ("Remotion.Scripting.StableBindingProxyProviderTest",
      new TypeLevelTypeFilter (new[] { typeof (ProxiedChildChildChild) }));


    [Test]
    public void BuildProxyType ()
    {
      var typeFilter = new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) });
      var provider = new StableBindingProxyProvider (
        typeFilter, ScriptingHelper.CreateModuleScope ("GetTypeMemberProxy"));

      Assert.That (provider.TypeFilter, Is.SameAs (typeFilter));

      var proxied = new ProxiedChildChildChild ("abrakadava");
      const string attributeName = "PrependName";

      var proxyType = provider.BuildProxyType (proxied.GetType());

      var proxyMethod = proxyType.GetAllInstanceMethods(attributeName,typeof(string)).Single();

      Assert.That (proxyMethod, Is.Not.Null);
    }


    [Test]
    public void GetTypeMemberProxy ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), ScriptingHelper.CreateModuleScope ("GetTypeMemberProxy"));

      var proxied = new ProxiedChildChildChild ("abrakadava");
      const string attributeName = "PrependName";

      var typeMemberProxy = provider.GetAttributeProxy (proxied, attributeName);

      var customMemberTester = new GetCustomMemberTester (typeMemberProxy);

      var result = ScriptingHelper.ExecuteScriptExpression<string> ("p0.XYZ('simsalbum',2)", customMemberTester);
      Assert.That (result, Is.EqualTo ("ProxiedChild ProxiedChild: abrakadava simsalbum, THE NUMBER=2"));
    }

    [Test]
    public void BuildProxy ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), ScriptingHelper.CreateModuleScope ("BuildProxy"));

      var proxied = new ProxiedChildChildChild ("abrakadava");
      const string attributeName = "PrependName";

      var proxy = provider.BuildProxy (proxied);
      // Necessary since a newly built proxy has an empty proxied field
      // TODO: Introduce BuildProxyFromType(proxiedType)
      ScriptingHelper.SetProxiedFieldValue (proxy, proxied); 
   
      Assert.That (proxy, Is.Not.Null);

      var result = ScriptingHelper.ExecuteScriptExpression<string> ("p0.PrependName('simsalbum',2)", proxy);
      Assert.That (result, Is.EqualTo ("ProxiedChild ProxiedChild: abrakadava simsalbum, THE NUMBER=2"));
    }

    [Test]
    public void GetProxyType_IsCached ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (GetProxyTypeIsCachedTest) }), ScriptingHelper.CreateModuleScope ("BuildProxy"));

      var proxied = new GetProxyTypeIsCachedTest ("abrakadava");

      var proxyType =  provider.GetProxyType (proxied.GetType());
      Assert.That (proxyType, Is.Not.Null);
      Assert.That (provider.GetProxyType (proxied.GetType ()), Is.SameAs (proxyType));
    }

    [Test]
    public void GetProxy_IsCached ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (GetProxyTypeIsCachedTest) }), ScriptingHelper.CreateModuleScope ("BuildProxy"));

      var proxied0 = new GetProxyTypeIsCachedTest ("abrakadava");
      var proxied1 = new GetProxyTypeIsCachedTest ("simsalsabum");

      var proxy0 = provider.GetProxy (proxied0);
      Assert.That (proxy0, Is.Not.Null);
      var proxy1 = provider.GetProxy (proxied1);
      Assert.That (proxy0, Is.SameAs (proxy1));
    }

    [Test]
    public void GetProxy_IsCachedAndProxiedIsSet ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (GetProxyTypeIsCachedTest) }), ScriptingHelper.CreateModuleScope ("GetProxy_IsCachedAndProxiedSet"));

      var proxied0 = new GetProxyTypeIsCachedTest ("abrakadava");
      var proxied1 = new GetProxyTypeIsCachedTest ("simsalsabum");

      var proxy0 = provider.GetProxy (proxied0);
      Assert.That (proxy0, Is.Not.Null);

      var proxiedFieldValue0 = ScriptingHelper.GetProxiedFieldValue (proxy0);
      Assert.That (proxiedFieldValue0, Is.SameAs (proxied0));
      var proxy1 = provider.GetProxy (proxied1);
      Assert.That (proxy0, Is.SameAs (proxy1));
      Assert.That (ScriptingHelper.GetProxiedFieldValue (proxy1), Is.SameAs (proxied1));
    }

    [Test]
    public void GetAttributeProxy ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), ScriptingHelper.CreateModuleScope ("GetTypeMemberProxy"));

      var proxied0 = new ProxiedChildChildChild ("ABCccccccccccccccccc");
      var proxied1 = new ProxiedChildChildChild ("xyzzzzzzzzz");

      var typeMemberProxy0 = provider.GetAttributeProxy (proxied0, "PrependName");
      var customMemberTester0 = new GetCustomMemberTester (typeMemberProxy0);
      var result0 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.XYZ('simsalbum',2)", customMemberTester0);
      Assert.That (result0, Is.EqualTo ("ProxiedChild ProxiedChild: ABCccccccccccccccccc simsalbum, THE NUMBER=2"));


      var typeMemberProxy1 = provider.GetAttributeProxy (proxied1, "NamePropertyVirtual");

      var customMemberTester1 = new GetCustomMemberTester (typeMemberProxy1);
      var result1 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.ABCDEFG", customMemberTester1);
      Assert.That (result1, Is.EqualTo ("ProxiedChildChildChild::NamePropertyVirtual xyzzzzzzzzz"));

    }

    [Test]
    public void GetAttributeProxy_IsCached ()
    {
      var provider = new StableBindingProxyProvider (
        new TypeLevelTypeFilter (new[] { typeof (ProxiedChild) }), ScriptingHelper.CreateModuleScope ("GetTypeMemberProxy"));

      var proxied0 = new ProxiedChildChildChild ("ABCccccccccccccccccc");
      var proxied1 = new ProxiedChildChildChild ("xyzzzzzzzzz");

      const string attributeName = "PrependName";
      var typeMemberProxy0 = provider.GetAttributeProxy (proxied0, attributeName);
      var customMemberTester0 = new GetCustomMemberTester (typeMemberProxy0);
      var result0 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.XYZ('simsalbum',2)", customMemberTester0);
      Assert.That (result0, Is.EqualTo ("ProxiedChild ProxiedChild: ABCccccccccccccccccc simsalbum, THE NUMBER=2"));


      var typeMemberProxy1 = provider.GetAttributeProxy (proxied1, attributeName);

      Assert.That (typeMemberProxy0, Is.SameAs (typeMemberProxy1));

      var customMemberTester1 = new GetCustomMemberTester (typeMemberProxy1);
      var result1 = ScriptingHelper.ExecuteScriptExpression<string> ("p0.ABCDEFG('Schlaf')", customMemberTester1);
      Assert.That (result1, Is.EqualTo ("xyzzzzzzzzz Schlaf"));

    }


    [Test]
    [Explicit]
    public void GetAttributeProxyChainCallPerformance ()
    {
      // ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied, attributeName);

      var proxied0 = new ProxiedChildChildChild ("ABC");

      ScriptContext.SwitchAndHoldScriptContext (_scriptContext);
      var currentStableBindingProxyProvider = ScriptContext.Current.StableBindingProxyProvider;

      var nrLoopsArray = new[] { 1, 1, 10, 100, 1000, 10000 };
      ScriptingHelper.ExecuteAndTime ("Direct", nrLoopsArray, () => currentStableBindingProxyProvider.GetAttributeProxy (proxied0, "PrependName"));
      ScriptingHelper.ExecuteAndTime ("Indirect", nrLoopsArray, () => ScriptContext.Current.StableBindingProxyProvider.GetAttributeProxy (proxied0, "PrependName"));

      ScriptContext.ReleaseScriptContext (_scriptContext);
    }


  }




  /// <summary>
  /// Stores a pythonScriptEngine.Operations.GetMember(proxy, attributeName)-wrapper-object and returns it 
  /// in calls to GetCustomMember.
  /// </summary>
  public class GetCustomMemberTester
  {
    private readonly object _typeMemberProxy;

    public GetCustomMemberTester (Object typeMemberProxy)
    {
      _typeMemberProxy = typeMemberProxy;
    }

    [SpecialName]
    public object GetCustomMember (string name)
    {
      return _typeMemberProxy;
    }
  }


  public class GetProxyTypeIsCachedTest : ProxiedChildChildChild
  {
    public GetProxyTypeIsCachedTest (string name)
     : base (name)
    {
    }

  }
}