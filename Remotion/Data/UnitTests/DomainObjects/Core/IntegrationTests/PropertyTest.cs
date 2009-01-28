// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class PropertyTest : ClientTransactionBaseTest
  {
    [Test]
    public void SetValidEnumValue ()
    {
      var instance = ClassWithAllDataTypes.NewObject ();
      instance.EnumProperty = ClassWithAllDataTypes.EnumType.Value0;
      Assert.That (instance.EnumProperty, Is.EqualTo (ClassWithAllDataTypes.EnumType.Value0));
    }

    [Test]
    [ExpectedException (typeof (InvalidEnumValueException))]
    public void SetInvalidEnumValue ()
    {
      var instance = ClassWithAllDataTypes.NewObject ();
      instance.EnumProperty = (ClassWithAllDataTypes.EnumType) (-1);
    }
    
    [Test]
    public void EnumNotDefiningZero ()
    {
      var instance = ClassWithEnumNotDefiningZero.NewObject ();
      Assert.That (instance.EnumValue, Is.EqualTo (TestDomain.EnumNotDefiningZero.First));
    }

  }
}