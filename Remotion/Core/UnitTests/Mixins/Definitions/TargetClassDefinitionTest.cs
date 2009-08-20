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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.Definitions
{
  [TestFixture]
  public class TargetClassDefinitionTest
  {
    [Test]
    public void ChildSpecificAccept_CallsVisitForClass ()
    {
      var visitorMock = MockRepository.GenerateMock<IDefinitionVisitor> ();
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1));

      targetClassDefinition.Accept (visitorMock);

      visitorMock.AssertWasCalled (mock => mock.Visit (targetClassDefinition));
    }

    [Test]
    public void ChildSpecificAccept ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1));
      var mixinDefinition = targetClassDefinition.Mixins[0];
      var requiredFaceTypeDefinition = DefinitionObjectMother.CreateRequiredFaceTypeDefinition (targetClassDefinition, typeof (IBT1Mixin1));
      var requiredBaseCallTypeDefinition = DefinitionObjectMother.CreateRequiredBaseCallTypeDefinition (targetClassDefinition, typeof (IBT1Mixin1));
      var requiredMixinTypeDefinition = DefinitionObjectMother.CreateRequiredMixinTypeDefinition (targetClassDefinition, typeof (BT1Mixin2));

      var visitorMock = MockRepository.GenerateMock<IDefinitionVisitor> ();
      using (visitorMock.GetMockRepository ().Ordered ())
      {
        visitorMock.Expect (mock => mock.Visit (targetClassDefinition));
        visitorMock.Expect (mock => mock.Visit (mixinDefinition));
        visitorMock.Expect (mock => mock.Visit (requiredFaceTypeDefinition));
        visitorMock.Expect (mock => mock.Visit (requiredBaseCallTypeDefinition));
        visitorMock.Expect (mock => mock.Visit (requiredMixinTypeDefinition));
      }

      visitorMock.Replay ();

      targetClassDefinition.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void ChildSpecificAccept_CallsVisitForMixins ()
    {
      var visitorMock = MockRepository.GenerateMock<IDefinitionVisitor> ();
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1));

      targetClassDefinition.Accept (visitorMock);

      visitorMock.AssertWasCalled (mock => mock.Visit (targetClassDefinition.Mixins[0]));
    }

    [Test]
    public void HasMixinWithConfiguredType_True ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType3));

      var nonGenericMixinType = typeof (BT3Mixin1);
      DefinitionObjectMother.CreateMixinDefinition (targetClassDefinition, nonGenericMixinType);
      
      var closedMixinType =  targetClassDefinition.MixinTypeCloser.GetClosedMixinType (typeof (BT3Mixin3<,>));
      DefinitionObjectMother.CreateMixinDefinition (targetClassDefinition, closedMixinType);

      Assert.That (targetClassDefinition.HasMixinWithConfiguredType (nonGenericMixinType), Is.True);
      Assert.That (targetClassDefinition.HasMixinWithConfiguredType (closedMixinType), Is.True);
      Assert.That (targetClassDefinition.HasMixinWithConfiguredType (closedMixinType.GetGenericTypeDefinition()), Is.True);
    }

    [Test]
    public void HasMixinWithConfiguredType_False ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType3));

      Assert.That (targetClassDefinition.HasMixinWithConfiguredType (typeof (BT3Mixin1)), Is.False);
      Assert.That (targetClassDefinition.HasMixinWithConfiguredType (typeof (BT3Mixin3<,>)), Is.False);
    }

    [Test]
    public void GetMixinByConfiguredType_NonNull ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType3));

      var nonGenericMixinType = typeof (BT3Mixin1);
      var nonGenericMixinDefinition = DefinitionObjectMother.CreateMixinDefinition (targetClassDefinition, nonGenericMixinType);

      var closedMixinType = targetClassDefinition.MixinTypeCloser.GetClosedMixinType (typeof (BT3Mixin3<,>));
      var closedMixinDefinition = DefinitionObjectMother.CreateMixinDefinition (targetClassDefinition, closedMixinType);

      Assert.That (targetClassDefinition.GetMixinByConfiguredType (nonGenericMixinType), Is.SameAs (nonGenericMixinDefinition));
      Assert.That (targetClassDefinition.GetMixinByConfiguredType (closedMixinType), Is.SameAs (closedMixinDefinition));
      Assert.That (targetClassDefinition.GetMixinByConfiguredType (closedMixinType.GetGenericTypeDefinition ()), Is.SameAs (closedMixinDefinition));
    }

    [Test]
    public void GetMixinByConfiguredType_Null ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType3));

      Assert.That (targetClassDefinition.GetMixinByConfiguredType (typeof (BT3Mixin1)), Is.Null);
      Assert.That (targetClassDefinition.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)), Is.Null);
    }
  }
}
