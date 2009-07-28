// Copyright (C) 2005 - 2009 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using System.Collections.Generic;
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
    public void Binder_SelectMethod ()
    {
    }


    public void AssertMethodBound (MethodInfo method, MethodInfo[] candidateMethods)
    {

      var parameterTypes = method.GetParameters ().Select (pi => pi.ParameterType).ToArray ();
      //To.ConsoleLine.e (method.Name).nl ().e (candidateMethods).nl ().e (parameterTypes);

      // Note: SelectMethod needs the candidateMethods already to have been filtered by name, otherwise AmbiguousMatchException|s may occur.
      candidateMethods = candidateMethods.Where (mi => (mi.Name == method.Name)).ToArray ();

      var boundMethod = Type.DefaultBinder.SelectMethod (BindingFlags.Instance | BindingFlags.Public,
        candidateMethods, parameterTypes, null);

      Assert.That (method, Is.SameAs((boundMethod)));
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
      Type type, Type candidateMethodsType, int expectedNumberOfMethods)
    {
      var metadataTokenToMethodInfoMap = BuildMetadataTokenToMethodInfoMap (candidateMethodsType);


      const string name = "PrependName";
      var candidateMethods = candidateMethodsType.GetPublicInstanceMethods (name, typeof (string));
      var methods = type.GetPublicInstanceMethods (name, typeof (string));
        
      Assert.That (candidateMethods.Length, Is.EqualTo (expectedNumberOfMethods));

      var methodWhichExistsInCanditateMethodsType = methods.Where (mi => mi.DeclaringType == candidateMethodsType).Single();
      var methodFromCanditateMethodsType = candidateMethods.Where (mi => 
        MethodInfoFromRelatedTypesEqualityComparer.Get.Equals (mi,methodWhichExistsInCanditateMethodsType)).Single();

      // Using the MethodInfo from the candidate methods type works
      Assert.That (StableBindingProxyBuilder.IsMethodBound (
        methodFromCanditateMethodsType, candidateMethods), Is.True);

      Assert.That (StableBindingProxyBuilder.IsMethodBound (
        GetCorrespondingMethod (metadataTokenToMethodInfoMap, methodWhichExistsInCanditateMethodsType), 
        candidateMethods), Is.True);

      // Using the MethodInfo from the type directly fails
      // TODO: Adapt IsMethodBound to work as above
      Assert.That (StableBindingProxyBuilder.IsMethodBound (
        methodWhichExistsInCanditateMethodsType, candidateMethods), Is.False);

      foreach (var method in methods)
      {
        if (!MethodInfoFromRelatedTypesEqualityComparer.Get.Equals (method, methodWhichExistsInCanditateMethodsType))
          Assert.That (StableBindingProxyBuilder.IsMethodBound (method, candidateMethods), Is.False);
      }
    }


    public Dictionary<StableMethodMetadataToken, MethodInfo> BuildMetadataTokenToMethodInfoMap (Type type)
    {
      return type.GetMethods (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToDictionary (
          mi => new StableMethodMetadataToken(mi), mi => mi);
    }

    private MethodInfo GetCorrespondingMethod (Dictionary<StableMethodMetadataToken, MethodInfo> dictionary, MethodInfo method)
    {
      var stableMetadataToken = new StableMethodMetadataToken(method);
      MethodInfo correspondingMethod;
      dictionary.TryGetValue (stableMetadataToken, out correspondingMethod);
      return correspondingMethod;
    }
  }
}