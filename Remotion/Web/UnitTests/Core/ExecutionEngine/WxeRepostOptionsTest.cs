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
using System;
using System.Web.UI;
using NUnit.Framework;
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
