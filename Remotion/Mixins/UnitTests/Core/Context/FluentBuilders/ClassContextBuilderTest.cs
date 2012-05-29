// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Remotion.Mixins.Context.Suppression;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Mixins.UnitTests.Core.Context.FluentBuilders
{
  [TestFixture]
  public class ClassContextBuilderTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _parentBuilderMock;
    private ClassContextBuilder _classBuilder;
    private ClassContextBuilder _classBuilderMock;
    private MixinContextBuilder _mixinBuilderMock;
    
    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _parentBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration)null);
      _classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2));
      _classBuilderMock = _mockRepository.StrictMock<ClassContextBuilder> (_parentBuilderMock, typeof (BaseType2));
      _mixinBuilderMock = _mockRepository.StrictMock<MixinContextBuilder> (_classBuilderMock, typeof (BT2Mixin1));
    }

    private Type[] GetMixinTypes ()
    {
      return GetMixinTypes (_classBuilder);
    }

    private Type[] GetMixinTypes (ClassContextBuilder classBuilder)
    {
      return classBuilder.MixinContextBuilders.Select (mcb => mcb.MixinType).ToArray();
    }

    [Test]
    public void Initialization_Standalone ()
    {
      var classBuilder = new ClassContextBuilder (typeof (BaseType2));
      Assert.AreSame (typeof (BaseType2), classBuilder.TargetType);
      Assert.IsNotNull (classBuilder.Parent);
      Assert.That (_classBuilder.MixinContextBuilders, Is.Empty);
      Assert.That (_classBuilder.CompleteInterfaces.ToArray (), Is.Empty);

      ClassContext classContext = _classBuilder.BuildClassContext (new ClassContext[0]);
      Assert.AreEqual (0, classContext.Mixins.Count);
      Assert.AreEqual (0, classContext.CompleteInterfaces.Count);
    }

    [Test]
    public void Initialization_WithNoParentContext ()
    {
      Assert.AreSame (typeof (BaseType2), _classBuilder.TargetType);
      Assert.AreSame (_parentBuilderMock, _classBuilder.Parent);
      Assert.That (_classBuilder.MixinContextBuilders, Is.Empty);
      Assert.That (_classBuilder.CompleteInterfaces.ToArray (), Is.Empty);
      
      ClassContext classContext = _classBuilder.BuildClassContext(new ClassContext[0]);
      Assert.AreEqual (0, classContext.Mixins.Count);
      Assert.AreEqual (0, classContext.CompleteInterfaces.Count);
    }

    [Test]
    public void Clear ()
    {
      var classBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType1));
      classBuilder.AddMixin<BT1Mixin2> ();
      classBuilder.AddCompleteInterface<IBaseType31> ();
      
      Assert.That (classBuilder.MixinContextBuilders, Is.Not.Empty);
      Assert.That (classBuilder.CompleteInterfaces, Is.Not.Empty);
      Assert.That (classBuilder.SuppressInheritance, Is.False);

      Assert.AreSame (classBuilder, classBuilder.Clear());
      Assert.That (classBuilder.MixinContextBuilders, Is.Empty);
      Assert.That (classBuilder.CompleteInterfaces.ToArray (), Is.Empty);
      Assert.That (classBuilder.SuppressInheritance, Is.True);
    }

    [Test]
    public void AddMixin_NonGeneric ()
    {
      MixinContextBuilder mixinBuilder = _classBuilder.AddMixin (typeof (BT2Mixin1));
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilder.MixinType);
      Assert.AreSame (_classBuilder, mixinBuilder.Parent);
      Assert.That (_classBuilder.MixinContextBuilders, Has.Member(mixinBuilder));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Remotion.Mixins.UnitTests.Core.TestDomain.BT2Mixin1 is already configured as a "
        + "mixin for type Remotion.Mixins.UnitTests.Core.TestDomain.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddMixin_Twice ()
    {
      _classBuilder.AddMixin (typeof (BT2Mixin1)).AddMixin (typeof (BT2Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithVirtualMethod`1 is "
        + "already configured as a mixin for type Remotion.Mixins.UnitTests.Core.TestDomain.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddMixin_Twice_Generic1 ()
    {
      _classBuilder.AddMixin (typeof (GenericMixinWithVirtualMethod<>)).AddMixin (typeof (GenericMixinWithVirtualMethod<>));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithVirtualMethod`1 is "
        + "already configured as a mixin for type Remotion.Mixins.UnitTests.Core.TestDomain.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddMixin_Twice_Generic2 ()
    {
      _classBuilder.AddMixin (typeof (GenericMixinWithVirtualMethod<object>)).AddMixin (typeof (GenericMixinWithVirtualMethod<>));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithVirtualMethod`1 is "
        + "already configured as a mixin for type Remotion.Mixins.UnitTests.Core.TestDomain.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddMixin_Twice_Generic3 ()
    {
      _classBuilder.AddMixin (typeof (GenericMixinWithVirtualMethod<>)).AddMixin (typeof (GenericMixinWithVirtualMethod<object>));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Remotion.Mixins.UnitTests.Core.TestDomain.GenericMixinWithVirtualMethod`1 is "
        + "already configured as a mixin for type Remotion.Mixins.UnitTests.Core.TestDomain.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddMixin_Twice_Generic4 ()
    {
      _classBuilder.AddMixin (typeof (GenericMixinWithVirtualMethod<string>)).AddMixin (typeof (GenericMixinWithVirtualMethod<object>));
    }

    [Test]
    public void AddMixin_Generic ()
    {
      Expect.Call (_classBuilderMock.AddMixin<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_mixinBuilderMock, _classBuilderMock.AddMixin<BT2Mixin1>());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddMixins_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT3Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.AddMixin (typeof (BT3Mixin2))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddMixins_Generic2 ()
    {
      Expect.Call (_classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1>())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddMixins_Generic3 ()
    {
      Expect.Call (_classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddOrderedMixins_NonGeneric ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddOrderedMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      var mixinBuilders = new List<MixinContextBuilder> (_classBuilder.MixinContextBuilders);
      Assert.AreEqual (3, mixinBuilders.Count);
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilders[0].MixinType);
      Assert.That (mixinBuilders[0].Dependencies, Is.Empty);
      Assert.AreSame (typeof (BT3Mixin1), mixinBuilders[1].MixinType);
      Assert.That(mixinBuilders[1].Dependencies, Is.EquivalentTo(new object[] { typeof(BT2Mixin1) }));
      Assert.AreSame (typeof (BT3Mixin2), mixinBuilders[2].MixinType);
      Assert.That (mixinBuilders[2].Dependencies, Is.EquivalentTo (new object[] { typeof (BT3Mixin1) }));
    }

    [Test]
    public void AddOrderedMixins_Generic2 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddOrderedMixins<BT2Mixin1, BT3Mixin1>());
      var mixinBuilders = new List<MixinContextBuilder> (_classBuilder.MixinContextBuilders);
      Assert.AreEqual (2, mixinBuilders.Count);
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilders[0].MixinType);
      Assert.That (mixinBuilders[0].Dependencies, Is.Empty);
      Assert.AreSame (typeof (BT3Mixin1), mixinBuilders[1].MixinType);
      Assert.That (mixinBuilders[1].Dependencies, Is.EquivalentTo (new object[] { typeof (BT2Mixin1) }));
    }

    [Test]
    public void AddOrderedMixins_Generic3 ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddOrderedMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2>());
      var mixinBuilders = new List<MixinContextBuilder> (_classBuilder.MixinContextBuilders);
      Assert.AreEqual (3, mixinBuilders.Count);
      Assert.AreSame (typeof (BT2Mixin1), mixinBuilders[0].MixinType);
      Assert.That (mixinBuilders[0].Dependencies, Is.Empty);
      Assert.AreSame (typeof (BT3Mixin1), mixinBuilders[1].MixinType);
      Assert.That (mixinBuilders[1].Dependencies, Is.EquivalentTo (new object[] { typeof (BT2Mixin1) }));
      Assert.AreSame (typeof (BT3Mixin2), mixinBuilders[2].MixinType);
      Assert.That (mixinBuilders[2].Dependencies, Is.EquivalentTo (new object[] { typeof (BT3Mixin1) }));
    }

    [Test]
    public void EnsureMixin_NonGeneric ()
    {
      MixinContextBuilder builder = _classBuilder.EnsureMixin (typeof (BT2Mixin1));
      Assert.AreEqual (typeof (BT2Mixin1), builder.MixinType);
      Type[] mixinTypes = GetMixinTypes();
      Assert.That (mixinTypes, Is.EquivalentTo (new object[] { typeof (BT2Mixin1) }));
      Assert.AreSame (builder, _classBuilder.EnsureMixin (typeof (BT2Mixin1)));
      Assert.That (mixinTypes, Is.EquivalentTo (new object[] { typeof (BT2Mixin1) }));
    }

    [Test]
    public void EnsureMixin_Inheritance ()
    {
      var contextWithMixin = ClassContextObjectMother.Create(typeof (BaseType3), typeof (NullTarget));
      
      MixinContextBuilder builder = _classBuilder.EnsureMixin (typeof (DerivedNullTarget));
      Assert.AreEqual (typeof (DerivedNullTarget), builder.MixinType);
      Type[] mixinTypes = GetMixinTypes ();
      Assert.That (mixinTypes, Is.EquivalentTo (new object[] { typeof (DerivedNullTarget) }));

      ClassContext builtContext = _classBuilder.BuildClassContext (new[] {contextWithMixin});
      Assert.AreEqual (1, builtContext.Mixins.Count);
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (DerivedNullTarget)));
      Assert.IsFalse (builtContext.Mixins.ContainsKey (typeof (NullTarget)));
    }

    [Test]
    public void EnsureMixin_Generic ()
    {
      Expect.Call (_classBuilderMock.EnsureMixin<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_mixinBuilderMock, _classBuilderMock.EnsureMixin<BT2Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void EnsureMixins_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT2Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT3Mixin1))).Return (_mixinBuilderMock);
      Expect.Call (_classBuilderMock.EnsureMixin (typeof (BT3Mixin2))).Return (_mixinBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void EnsureMixins_Generic2 ()
    {
      Expect.Call (_classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1> ())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void EnsureMixins_Generic3 ()
    {
      Expect.Call (_classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.EnsureMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.EnsureMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterface_NonGeneric ()
    {
      Assert.AreSame (_classBuilder, _classBuilder.AddCompleteInterface (typeof (IBT6Mixin1)));
      Assert.That (_classBuilder.CompleteInterfaces.ToArray(), Is.EquivalentTo (new object[] { typeof (IBT6Mixin1) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Remotion.Mixins.UnitTests.Core.TestDomain.IBT6Mixin1 is already configured as a "
        + "complete interface for type Remotion.Mixins.UnitTests.Core.TestDomain.BaseType2.", MatchType = MessageMatch.Contains)]
    public void AddCompleteInterface_Twice ()
    {
      _classBuilder.AddCompleteInterface (typeof (IBT6Mixin1)).AddCompleteInterface (typeof (IBT6Mixin1));
    }

    [Test]
    public void AddCompleteInterface_Generic ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterface<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT2Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterface<BT2Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterfaces_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT2Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT3Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.AddCompleteInterface (typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterfaces_Generic2 ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1> ())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void AddCompleteInterfaces_Generic3 ()
    {
      Expect.Call (_classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.AddCompleteInterfaces (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.AddCompleteInterfaces<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixin_Rule ()
    {
      var ruleStub = MockRepository.GenerateStub<IMixinSuppressionRule> ();

      Assert.That (_classBuilder.SuppressedMixins, Is.Empty);
      _classBuilder.SuppressMixin (ruleStub);
      Assert.That (_classBuilder.SuppressedMixins, Is.EquivalentTo (new[] { ruleStub }));
    }
    
    [Test]
    public void SuppressMixin_NonGeneric ()
    {
      Assert.That (_classBuilder.SuppressedMixins, Is.Empty);
      _classBuilder.SuppressMixin (typeof (BT1Mixin1));
      _classBuilder.SuppressMixin (typeof (BT2Mixin1));
      Assert.That (
          _classBuilder.SuppressedMixins.Cast<MixinTreeSuppressionRule>().Select (r => r.MixinBaseTypeToSuppress).ToArray(),
          Is.EquivalentTo (new object[] { typeof (BT2Mixin1), typeof (BT1Mixin1) }));
    }

    [Test]
    public void SuppressMixin_Generic ()
    {
      Expect.Call (_classBuilderMock.SuppressMixin<BT2Mixin1> ()).CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT2Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixin<BT2Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixins_NonGeneric ()
    {
      Expect.Call (_classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)))
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT2Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT3Mixin1))).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.SuppressMixin (typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2)));
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixins_Generic2 ()
    {
      Expect.Call (_classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1> ())
           .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void SuppressMixins_Generic3 ()
    {
      Expect.Call (_classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ())
          .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      Expect.Call (_classBuilderMock.SuppressMixins (typeof (BT2Mixin1), typeof (BT3Mixin1), typeof (BT3Mixin2))).Return (_classBuilderMock);

      _mockRepository.Replay (_classBuilderMock);
      Assert.AreSame (_classBuilderMock, _classBuilderMock.SuppressMixins<BT2Mixin1, BT3Mixin1, BT3Mixin2> ());
      _mockRepository.Verify (_classBuilderMock);
    }

    [Test]
    public void BuildContext_NoInheritance ()
    {
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2>();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2>();

      ClassContext builtContext = _classBuilder.BuildClassContext ();
      
      Assert.AreEqual (2, builtContext.Mixins.Count);
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin2)));

      Assert.AreEqual (2, builtContext.CompleteInterfaces.Count);
      Assert.IsTrue (builtContext.CompleteInterfaces.ContainsKey (typeof (IBT6Mixin1)));
      Assert.IsTrue (builtContext.CompleteInterfaces.ContainsKey (typeof (IBT6Mixin2)));
    }

    [Test]
    public void BuildContext_SuppressedInheritance ()
    {
      ClassContext inheritedContext = new ClassContextBuilder (typeof (BaseType2))
          .AddMixin (typeof (BT3Mixin1))
          .AddCompleteInterface (typeof (BT1Mixin2))
          .BuildClassContext();

      _classBuilder.Clear ();
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2> ();

      ClassContext builtContext = _classBuilder.BuildClassContext (new[] { inheritedContext });

      Assert.AreEqual (2, builtContext.Mixins.Count);
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin2)));

      Assert.AreEqual (2, builtContext.CompleteInterfaces.Count);
      Assert.IsTrue (builtContext.CompleteInterfaces.ContainsKey (typeof (IBT6Mixin1)));
      Assert.IsTrue (builtContext.CompleteInterfaces.ContainsKey (typeof (IBT6Mixin2)));
    }

    [Test]
    public void BuildContext_WithInheritance ()
    {
      ClassContext inheritedContext = new ClassContextBuilder (typeof (BaseType7))
          .AddMixin (typeof (BT7Mixin1))
          .AddCompleteInterface (typeof (BT1Mixin2))
          .BuildClassContext();
      
      _classBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();
      _classBuilder.AddCompleteInterfaces<IBT6Mixin1, IBT6Mixin2> ();

      ClassContext builtContext = _classBuilder.BuildClassContext (new[] { inheritedContext });

      Assert.AreEqual (3, builtContext.Mixins.Count);
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT7Mixin1)));

      Assert.AreEqual (3, builtContext.CompleteInterfaces.Count);
      Assert.IsTrue (builtContext.CompleteInterfaces.ContainsKey (typeof (IBT6Mixin1)));
      Assert.IsTrue (builtContext.CompleteInterfaces.ContainsKey (typeof (IBT6Mixin2)));
      Assert.IsTrue (builtContext.CompleteInterfaces.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildContext_ExtendParentContext ()
    {
      var classContextBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2));
      classContextBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();

      var parentContext = ClassContextObjectMother.Create(typeof (BaseType2), typeof (BT2Mixin1));
      ClassContext builtContext = classContextBuilder.BuildClassContext (new[] { parentContext });
      
      Assert.AreEqual (3, builtContext.Mixins.Count);
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT2Mixin1)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildContext_ReplaceParentContext ()
    {
      var classContextBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2));
      classContextBuilder.Clear ().AddMixins<BT1Mixin1, BT1Mixin2> ();

      var parentContext = ClassContextObjectMother.Create(typeof (BaseType2), typeof (BT2Mixin1));
      ClassContext builtContext = classContextBuilder.BuildClassContext (new[] { parentContext });

      Assert.AreEqual (2, builtContext.Mixins.Count);
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildContext_Suppression ()
    {
      var classContextBuilder = new ClassContextBuilder (_parentBuilderMock, typeof (BaseType2));
      classContextBuilder.AddMixins<BT1Mixin1, BT1Mixin2> ();

      classContextBuilder.SuppressMixins (typeof (IBT1Mixin1), typeof (BT5Mixin1), typeof (BT3Mixin3<,>));

      var inheritedContext = ClassContextObjectMother.Create(typeof (BaseType2), typeof (BT3Mixin1), typeof (BT3Mixin3<IBaseType33, IBaseType33>));
      var parentContext = ClassContextObjectMother.Create(typeof (BaseType2), typeof (BT5Mixin1), typeof (BT5Mixin2));
      ClassContext builtContext = classContextBuilder.BuildClassContext (new[] { inheritedContext, parentContext });

      Assert.AreEqual (3, builtContext.Mixins.Count);
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT3Mixin1)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT5Mixin2)));
      Assert.IsTrue (builtContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void ParentMembers ()
    {
      _mockRepository.BackToRecordAll();
      
      var r1 = new ClassContextBuilder (new MixinConfigurationBuilder (null), typeof (object));
      var r2 = new MixinConfiguration ();
      var r3 = _mockRepository.StrictMock<IDisposable> ();

      using (_mockRepository.Ordered ())
      {
        Expect.Call (_parentBuilderMock.ForClass<object> ()).Return (r1);
        Expect.Call (_parentBuilderMock.ForClass<string>()).Return (r1);
        Expect.Call (_parentBuilderMock.BuildConfiguration()).Return (r2);
        Expect.Call (_parentBuilderMock.EnterScope()).Return (r3);
      }
      
      _mockRepository.ReplayAll ();
      
      Assert.AreSame (r1, _classBuilder.ForClass<object> ());
      Assert.AreSame (r1, _classBuilder.ForClass<string> ());
      Assert.AreSame (r2, _classBuilder.BuildConfiguration ());
      Assert.AreSame (r3, _classBuilder.EnterScope ());

      _mockRepository.VerifyAll ();
    }
  }
}
