// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingProxyBuilder_ReflectionTests
  {
    [Test]
    public void IsMethodBound ()
    {
      Assert_IsMethodBound (typeof (ProxiedChildChildChild), 3);
      Assert_IsMethodBound (typeof (ProxiedChildChild), 2);
      Assert_IsMethodBound (typeof (ProxiedChild), 1);
      Assert_IsMethodBound (typeof (Proxied), 1);
    }

    private void Assert_IsMethodBound (Type type, int expectedNumberOfMethods)
    {
      const string name = "PrependName";
      var methods = type.GetMethods (BindingFlags.Instance | BindingFlags.Public).Where (
          mi => (mi.Name == name) && mi.GetParameters ().Length == 1 && mi.GetParameters ()[0].ParameterType == typeof (string)).ToArray ();

      Assert.That (methods.Length, Is.EqualTo (expectedNumberOfMethods));

      var method = methods[0];

      // Note: For this to work, the first method with a matching name must be the one which was added in the leaf class.
      Assert.That (StableBindingProxyBuilder.IsMethodBound (method, methods), Is.True);
      for (int i = 1; i < methods.Length; i++)
      {
        Assert.That (StableBindingProxyBuilder.IsMethodBound (methods[i], methods), Is.False);
      }
    }
  


    [Test]
    public void IsMethodBound_MethodInfosNotFromSameTypeAsMethodToTest ()
    {
      //Assert_IsMethodBound_MethodInfosNotFromSameTypeAsMethodToTest (
      //  typeof (Proxied), typeof (Proxied), 1);
      Assert_IsMethodBound_MethodInfosNotFromSameTypeAsMethodToTest (
        typeof (ProxiedChildChildChild), typeof (Proxied), 1);
    }

    private void Assert_IsMethodBound_MethodInfosNotFromSameTypeAsMethodToTest (
      Type type, Type canditateMethodsType, int expectedNumberOfMethods)
    {
      const string name = "PrependName";
      var canditateMethods = canditateMethodsType.GetPublicInstanceMethods (name, typeof (string));
      var methods = type.GetPublicInstanceMethods (name, typeof (string));
        
      Assert.That (canditateMethods.Length, Is.EqualTo (expectedNumberOfMethods));

      var methodWhichExistsInCanditateMethodsType = methods.Where (mi => mi.DeclaringType == canditateMethodsType).Single();
      var methodFromCanditateMethodsType = canditateMethods.Where (mi => 
        MethodInfoFromRelatedTypesEqualityComparer.Get.Equals (mi,methodWhichExistsInCanditateMethodsType)).Single();

      // Using the MethodInfo from the canditate methods type works
      Assert.That (StableBindingProxyBuilder.IsMethodBound (
        methodFromCanditateMethodsType, canditateMethods), Is.True);
      // Using the MethodInfo from the type directly fails
      // TODO: Adapt IsMethodBound to work as above
      Assert.That (StableBindingProxyBuilder.IsMethodBound (
        methodWhichExistsInCanditateMethodsType, canditateMethods), Is.False);

      foreach (var method in methods)
      {
        if (!MethodInfoFromRelatedTypesEqualityComparer.Get.Equals (method, methodWhichExistsInCanditateMethodsType))
          Assert.That (StableBindingProxyBuilder.IsMethodBound (method, canditateMethods), Is.False);
      }
    }
  }
}