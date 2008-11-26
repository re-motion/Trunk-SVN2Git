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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class CurrentPropertyManagerTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      CleanupProperties ();
    }

    public override void TearDown ()
    {
      base.TearDown ();
      CleanupProperties ();
    }

    private void CleanupProperties ()
    {
      while (CurrentPropertyManager.CurrentPropertyName != null)
        CurrentPropertyManager.PropertyAccessFinished ();
    }

    [Test]
    public void CurrentPropertyName_Null()
    {
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.Null);
    }

    [Test]
    public void PreparePropertyAccess()
    {
      CurrentPropertyManager.PreparePropertyAccess ("prop");
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.EqualTo ("prop"));
    }

    [Test]
    public void PropertyAccessFinished()
    {
      CurrentPropertyManager.PreparePropertyAccess ("prop");
      CurrentPropertyManager.PropertyAccessFinished ();

      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "There is no property to finish.")]
    public void PropertyAccessFinished_NoCurrentProperty ()
    {
      CurrentPropertyManager.PropertyAccessFinished ();
    }

    [Test]
    public void PropertyStack ()
    {
      CurrentPropertyManager.PreparePropertyAccess ("1");
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.EqualTo ("1"));
      CurrentPropertyManager.PreparePropertyAccess ("2");
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.EqualTo ("2"));
      CurrentPropertyManager.PreparePropertyAccess ("3");
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.EqualTo ("3"));

      CurrentPropertyManager.PropertyAccessFinished ();
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.EqualTo ("2"));
      CurrentPropertyManager.PropertyAccessFinished ();
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.EqualTo ("1"));
      CurrentPropertyManager.PropertyAccessFinished ();
      Assert.That (CurrentPropertyManager.CurrentPropertyName, Is.Null);
    }

    [Test]
    public void GetAndCheckCurrentPropertyName()
    {
      CurrentPropertyManager.PreparePropertyAccess ("prop");
      Assert.That (CurrentPropertyManager.GetAndCheckCurrentPropertyName (), Is.EqualTo ("prop"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "There is no current property or it hasn't been properly initialized. Is the surrounding property virtual?")]
    public void GetAndCheckCurrentPropertyName_NoCurrentProperty ()
    {
      CurrentPropertyManager.GetAndCheckCurrentPropertyName ();
    }
  }
}