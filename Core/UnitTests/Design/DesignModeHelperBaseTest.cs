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
using Rhino.Mocks;

namespace Remotion.UnitTests.Design
{
  [TestFixture]
  public class DesignModeHelperBaseTest
  {
    private MockRepository _mockRepository;
    private IDesignerHost _mockDesignerHost;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _mockDesignerHost = _mockRepository.StrictMock<IDesignerHost>();
    }

    [Test]
    public void Initialize()
    {
      _mockRepository.ReplayAll();

      DesignModeHelperBase stubDesginerHelper = new StubDesignModeHelper (_mockDesignerHost);

      _mockRepository.VerifyAll();
      Assert.That (stubDesginerHelper.DesignerHost, Is.SameAs (_mockDesignerHost));
    }
  }
}
