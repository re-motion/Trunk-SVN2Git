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
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeRepostOptionsTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The 'sender' must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.")]
    public void Inititalize_IsNot_IPostBackDataHandler_Or_IPostBackDataHandler ()
    {
      new WxeRepostOptions (MockRepository.GenerateStub<Control>(), false);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Parameter name: sender", MatchType = MessageMatch.Contains)]
    public void Inititalize_NotUsesEventTarget_NotSuppressSender ()
    {
      new WxeRepostOptions (null, false);
    }

  }
}