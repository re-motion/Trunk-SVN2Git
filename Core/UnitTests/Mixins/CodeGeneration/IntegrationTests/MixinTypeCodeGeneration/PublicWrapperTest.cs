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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.Mixins;
using Remotion.Reflection.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class PublicWrapperTest : CodeGenerationBaseTest
  {
    [Test]
    public void PublicWrapperGeneratedForOverriddenProtected()
    {
      Type type = ((IMixinTarget) CreateMixedObject<ClassOverridingMixinMembersProtected> (typeof (MixinWithAbstractMembers)).With()).Mixins[0].GetType();
      MethodInfo wrappedMethod = typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", BindingFlags.NonPublic | BindingFlags.Instance);

      MethodInfo wrapper = GetWrapper (type, wrappedMethod);
      Assert.That (wrapper, Is.Not.Null);
    }

    [Test]
    public void PublicWrapperGeneratedForProtectedOverrider ()
    {
      Type type = ((IMixinTarget) CreateMixedObject<BaseType1> (typeof (MixinWithProtectedOverrider)).With ()).Mixins[0].GetType ();
      MethodInfo wrappedMethod = typeof (MixinWithProtectedOverrider).GetMethod ("VirtualMethod", BindingFlags.NonPublic | BindingFlags.Instance);

      MethodInfo wrapper = GetWrapper (type, wrappedMethod);
      Assert.That (wrapper, Is.Not.Null);
    }

    [Test]
    public void NoPublicWrapperGeneratedForInfrastructureMembers ()
    {
      Type type = ((IMixinTarget) CreateMixedObject<BaseType1> (typeof (MixinWithProtectedOverrider)).With ()).Mixins[0].GetType ();
      IEnumerable<MethodInfo> wrappedMethods = 
          from method in type.GetMethods (BindingFlags.Instance | BindingFlags.Public)
          let attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (method, false)
          let declaringType = attribute != null ? attribute.ResolveWrappedMethod (type).DeclaringType : null
          let declaringTypeDefinition = declaringType != null && declaringType.IsGenericType ? declaringType.GetGenericTypeDefinition() : declaringType
          where attribute != null && (declaringTypeDefinition == typeof (Mixin<>) || declaringTypeDefinition == typeof (Mixin<,>))
          select method;

      Assert.That (wrappedMethods.ToArray (), Is.Empty);
    }

    private MethodInfo GetWrapper (Type type, MethodInfo wrappedMethod)
    {
      return (from method in type.GetMethods (BindingFlags.Instance | BindingFlags.Public)
              let attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (method, false)
              where attribute != null && attribute.ResolveWrappedMethod (type).Equals (wrappedMethod)
              select method).Single ();
    }
  }
}
