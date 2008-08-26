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
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Design;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Design
{
  [TestFixture]
  public class DesignerUtilityTest
  {
    [TearDown]
    public void TearDown ()
    {
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void GetDesignModeType_NoDesignMode ()
    {
      DesignerUtility.GetDesignModeType ("Foo");
    }

    [Test]
    public void GetDesignModeType_UsesHost ()
    {
      MockRepository mockRepository = new MockRepository ();
      IDesignModeHelper helperStub = mockRepository.Stub<IDesignModeHelper>();
      IDesignerHost hostMock = mockRepository.StrictMock<IDesignerHost> ();
      SetupResult.For (helperStub.DesignerHost).Return (hostMock);
      DesignerUtility.SetDesignMode (helperStub);

      Expect.Call (hostMock.GetType ("Foo")).Return (typeof (int));
      mockRepository.ReplayAll ();

      Assert.That (DesignerUtility.GetDesignModeType ("Foo"), Is.SameAs (typeof (int)));
      mockRepository.VerifyAll ();
    }
  }
}