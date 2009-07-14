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
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMother;
using Remotion.Diagnostics.ToText;
using Remotion.Scripting.UnitTests.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyBuilderTest
  {


    [Test]
    public void Ctor ()
    {
      var typeArbiter = new TypeLevelTypeArbiter(new Type[0]);
      var proxiedType = typeof (string);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("Ctor"));
      Assert.That (stableBindingProxyBuilder.ProxiedType, Is.EqualTo (proxiedType));
      Assert.That (stableBindingProxyBuilder.GetTypeFilter (), Is.EqualTo (typeArbiter));
    }

    [Test]
    public void BuildClassMethodToInterfaceMethodsMap_OneExplicitInterfaceMethod ()
    {
      var typeIAmbigous2 = typeof (IAmbigous2);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { typeIAmbigous2 });
      // Note: ProxiedChild implements IAmbigous1 and IAmbigous2
      var proxiedType = typeof(ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("BuildClassMethodToInterfaceMethodsMap_OneExplicitInterfaceMethod"));

      var classMethodToInterfaceMethodsMap = stableBindingProxyBuilder.GetClassMethodToInterfaceMethodsMap ();

      //To.ConsoleLine.e (classMethodToInterfaceMethodsMap);

      var stringTimesIAmbigous2ClassMethod = GetAnyInstanceMethod(proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous2.StringTimes");
      var stringTimesIAmbigous2InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous2, "StringTimes");

      //To.ConsoleLine.e (stringTimesMethod).nl ().e (stringTimesIAmbigous2InterfaceMethod);

      Assert.That (classMethodToInterfaceMethodsMap.Count, Is.EqualTo (1));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIAmbigous2ClassMethod].ToList(), Is.EquivalentTo (ListMother.New(stringTimesIAmbigous2InterfaceMethod)));
    }

    [Test]
    public void BuildClassMethodToInterfaceMethodsMap_TwoExplicitInterfaceMethods ()
    {
      var typeIAmbigous1 = typeof (IAmbigous1);
      var typeIAmbigous2 = typeof (IAmbigous2);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { typeIAmbigous2, typeof (Proxied), typeIAmbigous1 });
      // Note: ProxiedChild implements IAmbigous1 and IAmbigous2
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("BuildClassMethodToInterfaceMethodsMap_TwoExplicitInterfaceMethods"));
      //stableBindingProxyBuilder.BuildProxyType();

      var classMethodToInterfaceMethodsMap = stableBindingProxyBuilder.GetClassMethodToInterfaceMethodsMap ();

      var stringTimesIAmbigous1ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.StringTimes");
      Assert.That (stringTimesIAmbigous1ClassMethod, Is.Not.Null);
      var stringTimesIAmbigous1InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous1, "StringTimes");
      Assert.That (stringTimesIAmbigous1InterfaceMethod, Is.Not.Null);

      var stringTimesIAmbigous2ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous2.StringTimes");
      Assert.That (stringTimesIAmbigous2ClassMethod, Is.Not.Null);
      var stringTimesIAmbigous2InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous2, "StringTimes");
      Assert.That (stringTimesIAmbigous2InterfaceMethod, Is.Not.Null);

      Assert.That (classMethodToInterfaceMethodsMap.Count, Is.EqualTo (2));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIAmbigous1ClassMethod].ToList (), Is.EquivalentTo (ListMother.New (stringTimesIAmbigous1InterfaceMethod)));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIAmbigous2ClassMethod].ToList (), Is.EquivalentTo (ListMother.New (stringTimesIAmbigous2InterfaceMethod)));
    }

    [Test]
    public void BuildClassMethodToInterfaceMethodsMap_TwoInterfaceMethodsImplementedAsOnePublicMethod ()
    {
      var typeIProcessText1 = typeof (IProcessText1);
      var typeIProcessText2 = typeof (IProcessText2);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { typeIProcessText2, typeof (Proxied), typeIProcessText1 });
      // Note: ProxiedChildChild implements IProcessText1 and IProcessText2 through one public member
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("BuildClassMethodToInterfaceMethodsMap_TwoInterfaceMethodsImplementedAsOnePublicMethod"));

      var classMethodToInterfaceMethodsMap = stableBindingProxyBuilder.GetClassMethodToInterfaceMethodsMap ();

      var stringTimesIProcessText1ClassMethod = GetAnyInstanceMethod (proxiedType, "ProcessText");
      Assert.That (stringTimesIProcessText1ClassMethod, Is.Not.Null);

      var stringTimesIProcessText1InterfaceMethod = GetAnyInstanceMethod (typeIProcessText1, "ProcessText");
      Assert.That (stringTimesIProcessText1InterfaceMethod, Is.Not.Null);
      var stringTimesIProcessText2InterfaceMethod = GetAnyInstanceMethod (typeIProcessText2, "ProcessText");
      Assert.That (stringTimesIProcessText2InterfaceMethod, Is.Not.Null);

      Assert.That (classMethodToInterfaceMethodsMap.Count, Is.EqualTo (1));
      Assert.That (classMethodToInterfaceMethodsMap[stringTimesIProcessText1ClassMethod].ToList (), Is.EquivalentTo (ListMother.New (stringTimesIProcessText1InterfaceMethod, stringTimesIProcessText2InterfaceMethod)));
    }


    [Test]
    public void GetInterfaceMethodsToClassMethod ()
    {
      var typeIAmbigous2 = typeof (IAmbigous2);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { typeIAmbigous2 });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("GetInterfaceMethodsToClassMethod"));

      var stringTimesIAmbigous1ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous1.StringTimes");
      var stringTimesIAmbigous2ClassMethod = GetAnyInstanceMethod (proxiedType, "Remotion.Scripting.UnitTests.TestDomain.IAmbigous2.StringTimes");
      var stringTimesIAmbigous2InterfaceMethod = GetAnyInstanceMethod (typeIAmbigous2, "StringTimes");
     
      Assert.That (stableBindingProxyBuilder.GetInterfaceMethodsToClassMethod (stringTimesIAmbigous1ClassMethod).ToList (), Is.Empty);
      Assert.That (stableBindingProxyBuilder.GetInterfaceMethodsToClassMethod (stringTimesIAmbigous2ClassMethod).ToList (), Is.EquivalentTo (ListMother.New (stringTimesIAmbigous2InterfaceMethod)));
    }


    [Test]
    public void GetFirstKnownBaseType ()
    {
      AssertGetFirstKnownBaseType( typeof (ProxiedChildChild), new TypeLevelTypeArbiter (new[] { typeof (Proxied) }), typeof (Proxied));
      AssertGetFirstKnownBaseType (typeof (ProxiedChildChild), 
        new TypeLevelTypeArbiter (new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild) }), typeof (ProxiedChildChild));
      AssertGetFirstKnownBaseType (typeof (ProxiedChildChild), new TypeLevelTypeArbiter (new[] { typeof (string), typeof (Object), typeof (Type) }), typeof (Object));
    }

    private void AssertGetFirstKnownBaseType (Type proxiedType, TypeLevelTypeArbiter typeArbiter, Type expectedType)
    {
      var moduleScopeStub = MockRepository.GenerateStub<ModuleScope>();
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, moduleScopeStub);
      Assert.That (stableBindingProxyBuilder.GetFirstKnownBaseType (), Is.EqualTo (expectedType));
    }


    [Test]
    public void IsMethodKnownInClass_MethodNotHiddenThroughNew ()
    {
      var moduleScopeStub = MockRepository.GenerateStub<ModuleScope> ();
      var baseType = typeof (Proxied);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { baseType });
      var proxiedType = typeof (ProxiedChildChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, moduleScopeStub);

      var methodFromBaseType = GetAnyInstanceMethod (baseType, "Sum");
      var method = GetAnyInstanceMethod (proxiedType, "Sum");

      Assert.That (methodFromBaseType, Is.Not.EqualTo (method));
      // methodInfo.GetBaseDefinition() bug (results should be equal).
      Assert.That (methodFromBaseType.GetBaseDefinition (), Is.Not.EqualTo (method.GetBaseDefinition ()));

      // "Sum" is defined in Proxied
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (baseType, "Sum"), Is.True);
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (typeof (ProxiedChild), "Sum"), Is.True);
      AssertIsMethodKnownInClass (baseType, method, Is.True);

      // "OverrideMe" is overridden in ProxiedChild
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (baseType, "OverrideMe"), Is.True);
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (typeof (ProxiedChild), "OverrideMe"), Is.True);
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (proxiedType, "OverrideMe"), Is.True);

      // "PrependName" is redefined with new in ProxiedChildChild
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (baseType, "PrependName"), Is.True);
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (typeof (ProxiedChild), "PrependName"), Is.True);
      AssertIsMethodKnownInClass (baseType, GetAnyInstanceMethod (proxiedType, "PrependName"), Is.False);
    }

    private void AssertIsMethodKnownInClass (Type baseType, MethodInfo method, Constraint constraint)
    {
      // "DeclaringType.IsAssignableFrom"-workaround for mi.GetBaseDefinition() bug.
      bool isKnownInBaseType = method.GetBaseDefinition().DeclaringType.IsAssignableFrom (baseType);
      Assert.That (isKnownInBaseType, constraint);
    }


    [Test]
    public void BuildProxyType_ObjectMethods ()
    {
      var knownBaseType = typeof (Proxied);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { knownBaseType });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("BuildProxyType_ObjectMethods"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType ();

      var proxied = new ProxiedChild ();
      var proxy = Activator.CreateInstance (proxyType, proxied);

      Assert.That (InvokeMethod (proxy, "ToString"), Is.EqualTo (proxied.ToString ()));
      Assert.That (InvokeMethod (proxy, "GetType"), Is.EqualTo (proxied.GetType ()));
      Assert.That (InvokeMethod (proxy, "GetHashCode"), Is.EqualTo (proxied.GetHashCode ()));
      Assert.That (InvokeMethod (proxy, "Equals", proxied), Is.True);
      Assert.That (InvokeMethod (proxy, "Equals", proxy), Is.False);
    }

    public Object InvokeMethod (object instance, string name, params object[] arguments)
    {
      return instance.GetType().InvokeMember (
          name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, instance, arguments);
    }


    [Test]
    public void BuildProxyType_CheckMembers ()
    {
      var knownBaseType = typeof (Proxied);
      var typeArbiter = new TypeLevelTypeArbiter (new[] { knownBaseType });
      var proxiedType = typeof (ProxiedChild);
      var stableBindingProxyBuilder = new StableBindingProxyBuilder (proxiedType, typeArbiter, CreateModuleScope ("BuildProxyType"));
      var proxyType = stableBindingProxyBuilder.BuildProxyType();

      var knownBaseTypeMethods = knownBaseType.GetMethods ().Where (m => m.IsSpecialName == false);
      var proxyMethods = proxyType.GetMethods ().Where (m => m.IsSpecialName == false && m.DeclaringType != typeof(Object));

      Assert.That (knownBaseTypeMethods.Count (), Is.EqualTo (proxyMethods.Count()));

      //To.ConsoleLine.e (knownBaseTypeMethods).nl (2).e (proxyMethods);

      AssertHasSameMethod (knownBaseType, proxyType, "GetName");
      AssertHasSameMethod (knownBaseType, proxyType, "Sum", typeof(Int32[]));
      AssertHasSameMethod (knownBaseType, proxyType, "PrependName", typeof (String));
      AssertHasSameMethod (knownBaseType, proxyType, "SumMe", typeof(Int32[]));
      
      // TODO: Check why comparison fails (even though method exists in proxy)
      //AssertHasSameMethod (knownBaseType, proxyType, "OverrideMe", typeof (String));

      AssertHasSameGenericMethod (knownBaseType, proxyType, "GenericToString", 2);
      AssertHasSameGenericMethod (knownBaseType, proxyType, "OverloadedGenericToString", 1);
      AssertHasSameGenericMethod (knownBaseType, proxyType, "OverloadedGenericToString", 2);

      // {System.String GetName(),System.String Sum(Int32[]),System.String PrependName(System.String),System.String ToString(),
      //System.String SumMe(Int32[]),
      //System.String GenericToString[T0,T1](T0, T1),System.String OverloadedGenericToString[T0](T0),
      //System.String OverloadedGenericToString[T0,T1](T0, T1),
      //System.String OverrideMe(System.String),Boolean Equals(System.Object),Int32 GetHashCode(),System.Type GetType()} 
    }

    private void AssertHasSameMethod (Type type0, Type type1, string methodName, params Type[] parameterTypes)
    {
      var methodFromType0 = type0.GetMethod (methodName, parameterTypes);
      Assert.That (methodFromType0, Is.Not.Null);
      var methodFromType1 = type1.GetMethod (methodName, parameterTypes);
      Assert.That (methodFromType1, Is.Not.Null);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromType0, methodFromType1), Is.True);
    }

    private void AssertHasSameGenericMethod (Type type0, Type type1, string methodName, int numberOfGenericArguments)
    {
      var methodFromType0 = GetGenericMethod(type0, methodName, numberOfGenericArguments);
      Assert.That (methodFromType0, Is.Not.Null);
      var methodFromType1 = GetGenericMethod (type1, methodName, numberOfGenericArguments);
      Assert.That (methodFromType1, Is.Not.Null);
      //ScriptingHelper.ToConsoleLine (methodFromType0); ScriptingHelper.ToConsoleLine (methodFromType1);
      Assert.That (MethodInfoEqualityComparer.Get.Equals (methodFromType0, methodFromType1), Is.True);
    }

    public static MethodInfo GetGenericMethod (Type type, string methodName, int numberOfGenericArguments)
    {
      return GetGenericMethods (type, methodName, numberOfGenericArguments).Single();
    } 
    
    public static IEnumerable<MethodInfo> GetGenericMethods (Type type, string methodName, int numberOfGenericArguments)
    {
      return type.GetMethods().Where (m => m.Name == methodName && m.IsGenericMethod && m.GetGenericArguments().Length == numberOfGenericArguments);
    }

    //private void AssertHasSameMethodsAs (Type type, Type[] types)
    //{
    //  Assert.That (type.GetMethods ().Length, Is.EqualTo ((types.SelectMany (t => t.GetMethods ()).Count ())));
    //  throw new NotImplementedException();
    //}


    public ModuleScope CreateModuleScope(string namePostfix)
    {
      string name = "Remotion.Scripting.CodeGeneration.Generated.StableBindingProxyBuilderTest" + namePostfix;
      string nameSigned = name + ".Signed";
      string nameUnsigned = name + ".Unsigned";
      const string ext = ".dll";
      return new ModuleScope (true, nameSigned, nameSigned + ext, nameUnsigned, nameUnsigned + ext);
    }

    private void SaveAndVerifyModuleScopeAssembly (ModuleScope moduleScope)
    {
      ArgumentUtility.CheckNotNull ("moduleScope", moduleScope);
      if (moduleScope.StrongNamedModule != null)
        SaveAndVerifyModuleScopeAssembly (moduleScope, true);
      if (moduleScope.WeakNamedModule != null)
        SaveAndVerifyModuleScopeAssembly (moduleScope, false);
    }

    private void SaveAndVerifyModuleScopeAssembly (ModuleScope moduleScope, bool strongNamed)
    {
      var path = moduleScope.SaveAssembly (strongNamed);
      PEVerifier.VerifyPEFile (path);
    }

    private MethodInfo GetAnyInstanceMethod (Type type, string name)
    {
      return type.GetMethod (name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }
  }
}