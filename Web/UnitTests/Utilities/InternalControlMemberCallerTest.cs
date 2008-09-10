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
using System.Web.UI;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Utilities
{
  [TestFixture]
  public class InternalControlMemberCallerTest
  {
    private InternalControlMemberCaller _memberCaller;

    [SetUp]
    public void SetUp ()
    {
      _memberCaller = new InternalControlMemberCaller();
    }

    [Test]
    public void InitRecursive ()
    {
      MockRepository mockRepository = new MockRepository();
      Page namingContainer = new Page();
      Control parentControlMock = mockRepository.PartialMock<Control> ();
      Control childControlMock = mockRepository.PartialMock<Control> ();

      using (mockRepository.Ordered())
      {
        childControlMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnInit", EventArgs.Empty));
        parentControlMock.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnInit", EventArgs.Empty));
      }

      mockRepository.ReplayAll();

      namingContainer.Controls.Add (parentControlMock);
      parentControlMock.Controls.Add (childControlMock);
      _memberCaller.InitRecursive (parentControlMock, namingContainer);

      mockRepository.VerifyAll();
    }

    [Test]
    public void GetControlState ()
    {
      Control control = new Control();
      Assert.That (_memberCaller.GetControlState (control), Is.EqualTo (ControlState.Constructed));
    }

    [Test]
    public void SetControlState ()
    {
      Control control = new Control ();
      _memberCaller.SetControlState (control, ControlState.Initialized);
      Assert.That (_memberCaller.GetControlState (control), Is.EqualTo (ControlState.Initialized));
    }

    [Test]
    public void ControlStateNames_AreEquvialentToInternalControlStateNames ()
    {
      Assert.That (Enum.GetNames (InternalControlMemberCaller.InternalControlStateType), Is.EqualTo (Enum.GetNames (typeof (ControlState))));
    }

    [Test]
    public void ControlStateValues_AreEquvialentToInternalControlStateValues ()
    {
      Assert.That (Enum.GetValues (InternalControlMemberCaller.InternalControlStateType).Cast<int>().ToArray(),
          Is.EqualTo (Enum.GetValues (typeof (ControlState)).Cast<int> ().ToArray ()));
    }
  }
}