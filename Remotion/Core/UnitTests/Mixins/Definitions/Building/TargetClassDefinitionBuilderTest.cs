// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building;
using Remotion.UnitTests.Mixins.Definitions.TestDomain;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.Definitions.Building
{
  [TestFixture]
  public class TargetClassDefinitionBuilderTest
  {
    private TargetClassDefinitionBuilder _builder;

    [SetUp]
    public void SetUp ()
    {
      _builder = new TargetClassDefinitionBuilder ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "contains generic parameters", MatchType = MessageMatch.Contains)]
    public void Build_ThrowsOnGenericTargetClass ()
    {
      _builder.Build (new ClassContext (typeof (BT3Mixin3<,>)));
    }

    [Test]
    public void Build_SetsContext ()
    {
      var classContext = new ClassContext (typeof (BaseType1));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.ConfigurationContext, Is.SameAs (classContext));
    }

    [Test]
    public void Build_AddsPublicMembers ()
    {
      var classContext = new ClassContext (typeof (ClassWithDifferentMemberVisibilities));
      
      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "PublicMethod").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "PublicProperty").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "PublicEvent").ToArray (), Is.Not.Empty);

      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "PropertyWithPrivateSetter").ToArray (), Is.Not.Empty);
    }

    [Test]
    public void Build_AddsProtectedMembers ()
    {
      var classContext = new ClassContext (typeof (ClassWithDifferentMemberVisibilities));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "ProtectedMethod").ToArray(), Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "ProtectedProperty").ToArray(), Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "ProtectedEvent").ToArray(), Is.Not.Empty);
    }

    [Test]
    public void Build_AddsProtectedInternalMembers ()
    {
      var classContext = new ClassContext (typeof (ClassWithDifferentMemberVisibilities));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "ProtectedInternalMethod").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "ProtectedInternalProperty").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "ProtectedInternalEvent").ToArray (), Is.Not.Empty);
    }

    [Test]
    public void Build_AddsExplicitInterfaceMembers ()
    {
      var classContext = new ClassContext (typeof (ClassWithExplicitInterfaceImplementation));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == typeof (IInterfaceWithAllKindsOfMembers).FullName + ".Method").ToArray (), 
                   Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == typeof (IInterfaceWithAllKindsOfMembers).FullName + ".Property").ToArray (), 
                   Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == typeof (IInterfaceWithAllKindsOfMembers).FullName + ".Event").ToArray (),
                   Is.Not.Empty);
    }

    [Test]
    public void Build_NoInternalOrPrivateMembers ()
    {
      var classContext = new ClassContext (typeof (ClassWithDifferentMemberVisibilities));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "PrivateMethod").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "PrivateProperty").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "PrivateEvent").ToArray (), Is.Empty);

      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "InternalMethod").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "InternalProperty").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "InternalEvent").ToArray (), Is.Empty);
    }

    [Test]
    public void Build_AddsAttributes ()
    {
      var classContext = new ClassContext (typeof (BaseType1));
      var targetClassDefinition = _builder.Build (classContext);
      
      Assert.That (targetClassDefinition.CustomAttributes, Is.Not.Empty);
    }

    [Test]
    public void Build_AddsCompleteInterfaces ()
    {
      var classContext = new ClassContext (typeof (BaseType6), new MixinContext[0], new[] { typeof (ICBT6Mixin1) });
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.RequiredTargetCallTypes.Select (f => f.Type).ToArray(), List.Contains (typeof (ICBT6Mixin1)));
    }

    [Test]
    public void Build_AddsMixins ()
    {
      var classContext = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.Mixins.Select (m => m.Type).ToArray (), List.Contains (typeof (BT1Mixin1)));
    }

    [Test]
    public void Build_SortsMixins ()
    {
      var classContext = new ClassContext (
          typeof (BaseType7), 
          typeof (BT7Mixin0), 
          typeof (BT7Mixin1), 
          typeof (BT7Mixin2), 
          typeof (BT7Mixin3), 
          typeof (BT7Mixin5), 
          typeof (BT7Mixin9), 
          typeof (BT7Mixin10));

      var targetClassDefinition = _builder.Build (classContext);

      // see MixinDependencySortingIntegrationTest.MixinDefinitionsAreSortedCorrectlySmall
      Assert.That (targetClassDefinition.Mixins.Select (m => m.Type).ToArray (), Is.EqualTo (new[] {
          typeof (BT7Mixin0), 
          typeof (BT7Mixin2), 
          typeof (BT7Mixin3), 
          typeof (BT7Mixin1), 
          typeof (BT7Mixin10), 
          typeof (BT7Mixin9), 
          typeof (BT7Mixin5)
      }));
    }

    [Test]
    public void Build_SetsIndexesOfSortedMixins ()
    {
      var classContext = new ClassContext (
          typeof (BaseType7),
          typeof (BT7Mixin0),
          typeof (BT7Mixin1),
          typeof (BT7Mixin2),
          typeof (BT7Mixin3),
          typeof (BT7Mixin5),
          typeof (BT7Mixin9),
          typeof (BT7Mixin10));

      var targetClassDefinition = _builder.Build (classContext);

      // see MixinDependencySortingIntegrationTest.MixinDefinitionsAreSortedCorrectlySmall
      Assert.That (targetClassDefinition.Mixins.Select (m => m.MixinIndex).ToArray (), Is.EqualTo (new[] { 0, 1, 2, 3, 4, 5, 6 }));
    }

    [Test]
    public void Build_AppliesRequiredTargetCallTypeMethods ()
    {
      var classContext = new ClassContext (typeof (BaseType3), typeof (BT3Mixin1));

      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.RequiredTargetCallTypes[typeof (IBaseType31)].Methods.Select (r => r.InterfaceMethod).ToArray (),
                   List.Contains (typeof (IBaseType31).GetMethod ("IfcMethod")));
    }

    [Test]
    public void Build_AppliesRequiredBaseTypeMethods ()
    {
      var classContext = new ClassContext (typeof (BaseType3), typeof (BT3Mixin1));

      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.RequiredNextCallTypes[typeof (IBaseType31)].Methods.Select (r => r.InterfaceMethod).ToArray (),
                   List.Contains (typeof (IBaseType31).GetMethod ("IfcMethod")));
    }

    [Test]
    public void Build_AnalyzesMethodOverrides ()
    {
      const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

      var classContext = new ClassContext (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var targetClassDefinition = _builder.Build (classContext);

      var overrider = targetClassDefinition.Methods[typeof (ClassOverridingMixinMembers).GetMethod ("AbstractMethod")];
      var overridden = targetClassDefinition.Mixins[0].Methods[typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", bindingFlags)];
      
      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overridden.Overrides.ToArray(), List.Contains (overrider));
    }

    [Test]
    public void Build_AnalyzesPropertyOverrides ()
    {
      const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

      var classContext = new ClassContext (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var targetClassDefinition = _builder.Build (classContext);

      var overrider = targetClassDefinition.Properties[typeof (ClassOverridingMixinMembers).GetProperty ("AbstractProperty")];
      var overridden = targetClassDefinition.Mixins[0].Properties[typeof (MixinWithAbstractMembers).GetProperty ("AbstractProperty", bindingFlags)];

      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overridden.Overrides.ToArray (), List.Contains (overrider));
    }

    [Test]
    public void Build_AnalyzesEventOverrides ()
    {
      const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

      var classContext = new ClassContext (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var targetClassDefinition = _builder.Build (classContext);

      var overrider = targetClassDefinition.Events[typeof (ClassOverridingMixinMembers).GetEvent ("AbstractEvent")];
      var overridden = targetClassDefinition.Mixins[0].Events[typeof (MixinWithAbstractMembers).GetEvent ("AbstractEvent", bindingFlags)];

      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overridden.Overrides.ToArray (), List.Contains (overrider));
    }

    [Test]
    public void Build_AnalyzesAttributeIntroductions ()
    {
      var classContext = new ClassContext (typeof (NullTarget), typeof (MixinAddingBT1Attribute));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.ReceivedAttributes.Select (a => a.AttributeType).ToArray (), List.Contains (typeof (BT1Attribute)));
    }

    [Test]
    public void Build_AnalyzesSuppressorsOnClass ()
    {
      var classContext = new ClassContext (typeof (ClassWithSuppressAttribute), typeof (MixinAddingBT1Attribute));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.ReceivedAttributes.Select (a => a.AttributeType).ToArray (), Is.Empty);
    }

    [Test]
    public void Build_AnalyzesSuppressorsOnMixins ()
    {
      var classContext = new ClassContext (typeof (NullTarget), typeof (MixinAddingBT1Attribute), typeof (MixinSuppressingBT1Attribute));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.ReceivedAttributes.Select (a => a.AttributeType).ToArray (), List.Not.Contains (typeof (BT1Attribute)));
    }

    [Test]
    public void Build_AnalyzesAttributeIntroductionsOnMembers ()
    {
      var classContext = new ClassContext (typeof (GenericTargetClass<string>), typeof (MixinAddingBT1AttributeToMember));
      var targetClassDefinition = _builder.Build (classContext);

      var methodInfo = typeof (GenericTargetClass<string>).GetMethod ("VirtualMethod");
      Assert.That (targetClassDefinition.Methods[methodInfo].ReceivedAttributes.Select (a => a.AttributeType).ToArray (), 
                   List.Contains (typeof (BT1Attribute)));
    }
  }
}
