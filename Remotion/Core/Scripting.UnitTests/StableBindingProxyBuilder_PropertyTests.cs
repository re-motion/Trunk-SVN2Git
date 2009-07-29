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
using Microsoft.Scripting;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyBuilder_PropertyTests : StableBindingProxyBuilderTest
  {
    private const BindingFlags _nonPublicInstanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    private const BindingFlags _publicInstanceFlags = BindingFlags.Instance | BindingFlags.Public;
    private const BindingFlags _allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;


    [Test]
    public void BuildProxyType_PublicProperty ()
    {
      //var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownBaseTypes = new[] { typeof (ProxiedChildChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NameProperty, Is.EqualTo ("ProxiedChildChildChild::NameProperty PC"));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("NameProperty", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChild::NameProperty PC"));
    }

    [Test]
    public void BuildProxyType_PublicProperty_WithSetter ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NameProperty, Is.EqualTo ("ProxiedChildChildChild::NameProperty PC"));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("NameProperty", _publicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild::NameProperty PC"));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChild::NameProperty XXyyZZ-ProxiedChild::NameProperty"));
    }



    [Test]
    public void BuildProxyType_VirtualPublicProperty ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.NamePropertyVirtual, Is.EqualTo ("ProxiedChildChildChild::NamePropertyVirtual PC"));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("NamePropertyVirtual", _publicInstanceFlags);


      // Note: Virtual properties remain virtual, so the proxy calls go to ProxiedChildChildChild, not the first known base type of ProxiedChild.

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.False);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChildChild::NamePropertyVirtual PC"));
    }

    [Test]
    public void BuildProxyType_VirtualPublicProperty_WithSetter ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      //var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes; //knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_PublicProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");
      object proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (proxied.MutableNamePropertyVirtual, Is.EqualTo ("ProxiedChildChild::MutableNamePropertyVirtual PC"));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select (pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty ("MutableNamePropertyVirtual", _publicInstanceFlags);

      
      // Note: Virtual properties remain virtual, so the proxy calls go to ProxiedChildChild, not the first known base type of ProxiedChild 
      // (ProxiedChildChildChild does not override MutableNamePropertyVirtual).

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.CanRead, Is.True);
      Assert.That (proxyPropertyInfo.CanWrite, Is.True);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChild::MutableNamePropertyVirtual PC"));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo ("ProxiedChildChild::MutableNamePropertyVirtual XXyyZZ-ProxiedChildChild::MutableNamePropertyVirtual"));
    }


    [Test]
    public void BuildProxyType_ExplicitInterfaceProperty ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownInterfaceTypes = new[] { typeof (IProperty) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);

      const string expectedPropertyValue = "ProxiedChild::IAmbigous1::MutableNameProperty PC";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select(pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IProperty.MutableNameProperty", _nonPublicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
      //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

      ((IProperty) proxied).MutableNameProperty = "aBc";
      const string expectedPropertyValue2 = "ProxiedChild::IAmbigous1::MutableNameProperty aBc-ProxiedChild::IAmbigous1::MutableNameProperty";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue2));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue2));

      proxyPropertyInfo.SetValue (proxy,"XXyyZZ" ,null);
      const string expectedPropertyValue3 = "ProxiedChild::IAmbigous1::MutableNameProperty XXyyZZ-ProxiedChild::IAmbigous1::MutableNameProperty";
      Assert.That (((IProperty) proxied).MutableNameProperty, Is.EqualTo (expectedPropertyValue3));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue3));
    }


    [Test]
    [ExpectedException (ExceptionType = typeof (MissingMemberException), ExpectedMessage = "'ProxiedChild' object has no attribute 'PropertyAmbigous'")]
    public void AmbigousExplicitInterfaceProperties_Without_Proxy ()
    {
      var proxied = new ProxiedChild ("PC");
      ExecuteScriptAccessPropertyAmbigous(proxied);
    }

    private void ExecuteScriptAccessPropertyAmbigous (Object obj)
    {
      ExecuteScriptExpression<string> ("p0.PropertyAmbigous", obj);
    }

    [Test]
    public void AmbigousExplicitInterfaceProperties_With_Proxy ()
    {
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (
          proxiedType, new TypeLevelTypeFilter (new[] { typeof (IPropertyAmbigous2) }), CreateModuleScope ("AmbigousExplicitInterfaceProperties_With_Proxy"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType();

      var proxied = new ProxiedChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);

      // Proxy works, since only IPropertyAmbigous2 is exposed.
      ExecuteScriptAccessPropertyAmbigous (proxy);

      // Proxied fails, since call to PropertyAmbigous is ambigous.
      try
      {
        ExecuteScriptAccessPropertyAmbigous (proxied);
      }
      catch (MissingMemberException e)
      {
        Assert.That (e.Message, Is.EqualTo ("'ProxiedChild' object has no attribute 'PropertyAmbigous'"));
      }
    }


    [Test]
    public void BuildProxyType_AmbigousExplicitInterfaceProperties ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChild) };
      var knownInterfaceTypes = new[] { typeof (IProperty), typeof (IPropertyAmbigous2) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      Assert.That (proxyType.GetInterface ("IPropertyAmbigous2"), Is.Not.Null);
      Assert.That (proxyType.GetInterface ("IPropertyAmbigous1"), Is.Null);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);


      const string expectedPropertyValue = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous PC";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue));

      // Script call to proxy is not ambigous
      Assert.That (ExecuteScriptExpression<string> ("p0.PropertyAmbigous", proxy), Is.EqualTo (expectedPropertyValue));
      
      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select(pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());

      var proxyPropertyInfo = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous2.PropertyAmbigous", _nonPublicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
      //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

      ((IPropertyAmbigous2) proxied).PropertyAmbigous = "aBc";
      const string expectedPropertyValue2 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous aBc-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue2));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue2));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      const string expectedPropertyValue3 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous XXyyZZ-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue3));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue3));

      var proxyPropertyInfo2 = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous1.PropertyAmbigous", _nonPublicInstanceFlags);
      Assert.That (proxyPropertyInfo2, Is.Null);
    }

    [Test]
    public void BuildProxyType_AmbigousExplicitInterfaceProperties_With_PublicInterfaceImplementation ()
    {
      var knownBaseTypes = new[] { typeof (ProxiedChildChild) };
      var knownInterfaceTypes = new[] { typeof (IProperty), typeof (IPropertyAmbigous2) };
      var knownTypes = knownBaseTypes.Union (knownInterfaceTypes).ToArray ();
      var typeFilter = new TypeLevelTypeFilter (knownTypes);
      var proxiedType = typeof (ProxiedChildChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeFilter, CreateModuleScope ("BuildProxyType_ExplicitInterfaceProperty"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      Assert.That (proxyType.GetInterface ("IPropertyAmbigous2"), Is.Not.Null);
      Assert.That (proxyType.GetInterface ("IPropertyAmbigous1"), Is.Null);

      // Create proxy instance, initializing it with class to be proxied
      var proxied = new ProxiedChildChildChild ("PC");

      object proxy = Activator.CreateInstance (proxyType, proxied);

      const string expectedPropertyValue = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous PC";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue));

      //To.ConsoleLine.e ("proxyType.GetAllProperties()", proxyType.GetAllProperties ()).nl ().e (proxyType.GetAllProperties ().Select(pi => pi.Attributes)).nl (2).e ("proxyType.GetAllMethods()", proxyType.GetAllMethods ());


      var proxyPropertyInfoPublicProperty = proxyType.GetProperty ("PropertyAmbigous", _publicInstanceFlags);
      Assert.That (proxyPropertyInfoPublicProperty, Is.Not.Null);

      Assert.That (ExecuteScriptExpression<string> ("p0.PropertyAmbigous", proxy), Is.EqualTo ("ProxiedChildChild::PropertyAmbigous PC"));

      var proxyPropertyInfo = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous2.PropertyAmbigous", _nonPublicInstanceFlags);

      Assert.That (proxyPropertyInfo, Is.Not.Null);
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue));
      //AssertPropertyInfoEqual (proxyPropertyInfo, propertyInfo);

      ((IPropertyAmbigous2) proxied).PropertyAmbigous = "aBc";
      const string expectedPropertyValue2 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous aBc-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue2));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue2));

      proxyPropertyInfo.SetValue (proxy, "XXyyZZ", null);
      const string expectedPropertyValue3 = "ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous XXyyZZ-ProxiedChildChild::IPropertyAmbigous2::PropertyAmbigous";
      Assert.That (((IPropertyAmbigous2) proxied).PropertyAmbigous, Is.EqualTo (expectedPropertyValue3));
      Assert.That (proxyPropertyInfo.GetValue (proxy, null), Is.EqualTo (expectedPropertyValue3));

      var proxyPropertyInfo2 = proxyType.GetProperty (
        "Remotion.Scripting.UnitTests.TestDomain.ProxiedChild.Remotion.Scripting.UnitTests.TestDomain.IPropertyAmbigous1.PropertyAmbigous", _nonPublicInstanceFlags);
      Assert.That (proxyPropertyInfo2, Is.Null);

    }


    public TResult ExecuteScriptExpression<TResult> (string expressionScriptSourceCode, params object[] scriptParameter)
    {
      const ScriptLanguageType scriptLanguageType = ScriptLanguageType.Python;
      var engine = ScriptingHost.GetScriptEngine (scriptLanguageType);
      var scriptSource = engine.CreateScriptSourceFromString (expressionScriptSourceCode, SourceCodeKind.Expression);
      var compiledScript = scriptSource.Compile ();
      var scriptScope = ScriptingHost.GetScriptEngine (scriptLanguageType).CreateScope();

      for (int i = 0; i < scriptParameter.Length; i++)
      {
        scriptScope.SetVariable ("p" + i, scriptParameter[i]);
      }
      return compiledScript.Execute<TResult> (scriptScope);
    }

  }
}