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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
    public void PublicWrapperGeneratedForProtectedOverrider ()
    {
      Type type = ((IMixinTarget) CreateMixedObject<ClassOverridingMixinMembersProtected> (typeof (MixinWithAbstractMembers)).With()).Mixins[0].GetType();
      MethodInfo wrappedMethod = typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", BindingFlags.NonPublic | BindingFlags.Instance);

      var wrapper = (from method in type.GetMethods (BindingFlags.Instance | BindingFlags.Public)
                                      let attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (method, false)
                                      where attribute != null && type.Module.ResolveMethod (attribute.WrappedMethodRefToken).Equals (wrappedMethod)
                                      select method).Single();
      Assert.That (wrapper, Is.Not.Null);
    }

    [Test]
    [Ignore ("TODO: FS - add test")]
    public void PublicWrapperGeneratedForOverriddenProtected ()
    {
      Assert.Fail ("TODO");
    }

    [Test]
    [Ignore ("TODO: FS - add test")]
    public void NoPublicWrapperGeneratedForInfrastructureMembers ()
    {
      Assert.Fail ("TODO");
    }
  }
}
