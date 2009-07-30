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
using System.Collections;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Reflection;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class SpikeTests
  {
    class Base
    {
      public virtual void Test ()
      {
      }
    }

    class Derived1 : Base
    {
      public sealed override void Test ()
      {
        base.Test ();
      }
    }

    class Derived1Child : Derived1
    {
      public new void Test ()
      {
        base.Test ();
      }
    }

    class Derived2 : Proxied
    {
    }

    [Test]
    [Explicit]
    public void name ()
    {
      var method1 = typeof (Base).GetMethod ("Test");
      var method2 = typeof (Derived1).GetMethod ("Test");
      var method3 = typeof (Derived2).GetMethod ("Test");

      Assert.That (method1, Is.EqualTo (method2.GetBaseDefinition ()));
      Assert.That (method1, Is.EqualTo (method3.GetBaseDefinition ()));
      Assert.That (method2.GetBaseDefinition (), Is.EqualTo (method3.GetBaseDefinition ()));
    }

    [Test]
    [Explicit]
    public void name2 ()
    {
      var method1 = typeof (Proxied).GetMethod ("Sum");
      var method3 = typeof (Derived2).GetMethod ("Sum");

      Assert.That (method1.GetBaseDefinition (), Is.EqualTo (method3.GetBaseDefinition ()));
    }

    [Test]
    [Explicit]
    public void Binder_SelectMethod ()
    {
      var type = typeof (ProxiedChildChildChild);
      const string name = "PrependName";
      var methods = type.GetMethods (BindingFlags.Instance | BindingFlags.Public).Where (
        mi => (mi.Name == name)).ToArray ();
      var method = methods[0];
      var parameterTypes = method.GetParameters ().Select (pi => pi.ParameterType).ToArray ();
      //To.ConsoleLine.e (ScriptingHelper.GetAnyPublicInstanceMethodArray (typeof (ProxiedChildChildChild), "PrependName"));
      var boundMethod = Type.DefaultBinder.SelectMethod (BindingFlags.Instance | BindingFlags.Public, methods, parameterTypes, null);

      ScriptingHelper.ToConsoleLine ("PrependName", typeof (Proxied), typeof (ProxiedChild),
        typeof (ProxiedChildChild), typeof (ProxiedChildChildChild));
      To.ConsoleLine.nl ();

      ScriptingHelper.ToConsoleLine ((MethodInfo) boundMethod);
    }


    [Test]
    [Explicit]
    public void MethodsMetaDataToken ()
    {
      var types = new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild), typeof (ProxiedChildChildChild) };
      var methodNames = new[] { "PrependName", "PrependNameVirtual" };

      //var instance = new ProxiedChildChildChild("x");

      foreach (var methodName in methodNames)
      {
        To.ConsoleLine.nl (2).e (methodName).s (": ");
        foreach (var type in types)
        {
          type.GetAllInstanceMethods (methodName, typeof (string)).Process (mi => To.ConsoleLine.e (mi.MetadataToken).e (mi.GetBaseDefinition ().MetadataToken).e (mi.DeclaringType.FullName + mi.Name));
          //To.ConsoleLine.e (method.MetadataToken).e (method.GetBaseDefinition ().MetadataToken).e (method.DeclaringType.FullName + method.Name).e (method.Invoke (instance, new[] {"name"}));
          //To.ConsoleLine.e (method.MetadataToken).e (method.GetBaseDefinition ().MetadataToken).e (method.DeclaringType.FullName + method.Name);
        }
      }
    }


    [Test]
    [Explicit]
    public void PropertyMethodsMetaDataToken ()
    {
      var types = new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild), typeof (ProxiedChildChildChild) };
      var methodNames = new[] { "get_Name", "get_NameVirtual" };

      //var instance = new ProxiedChildChildChild("x");

      foreach (var methodName in methodNames)
      {
        To.ConsoleLine.nl (2).e (methodName).s (": ");
        foreach (var type in types)
        {
          type.GetAllInstanceMethods (methodName).Process (mi => To.ConsoleLine.e (mi.MetadataToken).e (mi.GetBaseDefinition ().MetadataToken).e (mi.DeclaringType.FullName + mi.Name));
          //To.ConsoleLine.e (method.MetadataToken).e (method.GetBaseDefinition ().MetadataToken).e (method.DeclaringType.FullName + method.Name).e (method.Invoke (instance, new[] {"name"}));
          //To.ConsoleLine.e (method.MetadataToken).e (method.GetBaseDefinition ().MetadataToken).e (method.DeclaringType.FullName + method.Name);
        }
      }
    }


    [Test]
    [Explicit]
    public void PropertiesMetaDataToken ()
    {
      var types = new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild), typeof (ProxiedChildChildChild) };
      var propertyNames = new[] { "Name", "MutableName", "ReadonlyName", "WriteonlyName" , "NameVirtual", "MutableNameVirtual", "ReadonlyNameVirtual", "WriteonlyNameVirtual" };

      foreach (var propertyName in propertyNames)
      {
        To.ConsoleLine.nl(2).e (propertyName).s(": ");
        foreach (var type in types)
        {
          type.GetAllProperties (propertyName).Process (pi => To.ConsoleLine.e (pi.MetadataToken).e (GetPropertyGetterSetterMetaDataToken (pi, true)).e (GetPropertyGetterSetterMetaDataToken (pi, false)));
          //var property = type.GetAllProperties (propertyName).Last ();
          //To.ConsoleLine.e (property.MetadataToken).e (GetPropertyGetterSetterMetaDataToken (property, true)).e (GetPropertyGetterSetterMetaDataToken (property, false));
        }
      }
    }


    [Test]
    [Explicit]
    public void PropertiesGetterSetter ()
    {
      var types = new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild), typeof (ProxiedChildChildChild) };

      foreach (var type in types)
      {
        To.ConsoleLine.nl (2).e (type.Name).s (": ");
        type.GetAllMethods ().Where (mi => mi.IsSpecialName).Process (mi => To.ConsoleLine.e (mi.Name).e (mi.GetBaseDefinition ().MetadataToken));
      }
    }


    [Test]
    [Explicit]
    public void GetterSetterInInterfaceMap ()
    {
      var type = typeof (ProxiedChild);
      var interfaceMapping = type.GetInterfaceMap (typeof (IAmbigous1));
      interfaceMapping.InterfaceMethods.Process (mi => To.ConsoleLine.e (mi.Name));
    }


    [Test]
    [Explicit]
    public void AllProperties ()
    {
      var types = new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild), typeof (ProxiedChildChildChild) };

      foreach (var type in types)
      {
        To.ConsoleLine.nl (2).e (type.Name).s (": ");
        type.GetAllProperties ().Process (pi => To.ConsoleLine.e (pi.Name).e (pi.MetadataToken));
      }
    }

    [Test]
    [Explicit]
    public void ExplicitInterfaceProperties ()
    {
      var types = new[] { typeof (Proxied), typeof (ProxiedChild), typeof (ProxiedChildChild), typeof (ProxiedChildChildChild) };

      foreach (var type in types)
      {
        To.ConsoleLine.nl (2).e (type.Name).s (": ");
        type.GetProperties (BindingFlags.Instance | BindingFlags.NonPublic).Process (pi => To.ConsoleLine.e (pi.Name).e(pi.GetAccessors(true)).e (pi.MetadataToken));
        To.ConsoleLine.nl();
        type.GetProperties (BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).Process (pi => To.ConsoleLine.e (pi.Name).e (pi.GetAccessors (true)).e (pi.MetadataToken));
      }
    }


    //[Test]
    //public void DiscoverScriptContext ()
    //{
    //  const bool excludeGlobalTypes = true;
    //  // TODO: Use ContextAwareTypeDiscoveryUtility.GetInstance() instead to make use of assembly caching
    //  ITypeDiscoveryService _typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (
    //      new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, excludeGlobalTypes));

    //  ICollection types = _typeDiscoveryService.GetTypes (typeof (T), excludeGlobalTypes);

    //  foreach (Type type in types)
    //  {
    //    if (RetrieveTextHandlerAttribute (type) != null)
    //    {
    //      Type baseType = type.BaseType;
    //      //Assertion.IsTrue (baseType.Name == "ToTextSpecificTypeHandler`1");
    //      Assertion.IsTrue (baseType.Name == baseTypeName); // Note: This check disallows use of derived type handlers
    //      // TODO: Refactor check to use Type objects and IsAssignableFrom instead.
    //      // TODO: Throw exception instead if attribute is attached to wrong type
    //      // Idea: If IToTextSpecificHandler had a membler "HandledType", the check (and the following two lines) could be removed.
    //      Type[] genericArguments = baseType.GetGenericArguments ();
    //      Type handledType = genericArguments[0];

    //      handlerMap[handledType] = (T) Activator.CreateInstance (type);
    //    }
    //  }
    //  return handlerMap;
    //}

    public int GetPropertyGetterSetterMetaDataToken (PropertyInfo property, bool getGetter)
    {
      //property.
      var method = getGetter ? property.GetGetMethod() : property.GetSetMethod();
      int token = -1;
      if(method != null)
      {
        token = method.GetBaseDefinition().MetadataToken;
      }
      return token;
    }
  }
}