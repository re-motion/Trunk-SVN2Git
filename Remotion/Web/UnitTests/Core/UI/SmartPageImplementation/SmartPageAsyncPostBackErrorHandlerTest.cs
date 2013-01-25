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
using System.Collections;
using System.Web;
using NUnit.Framework;
using Remotion.Web.UI;
using Remotion.Web.UI.SmartPageImplementation;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.SmartPageImplementation
{
  [TestFixture]
  public class SmartPageAsyncPostBackErrorHandlerTest
  {
    [Test]
    public void HandleError_IsCustomErrorEnabledFalse_SetsAspNetDetailedErrorMessage ()
    {
      var contextStub = MockRepository.GenerateStub<HttpContextBase>();
      contextStub.Stub (_ => _.Items).Return (new Hashtable());
      contextStub.Stub (_ => _.IsCustomErrorEnabled).Return (false);

      var handler = new SmartPageAsyncPostBackErrorHandler (contextStub);

      Assert.That (() => handler.HandleError (new ApplicationException ("The error")), Throws.TypeOf<AsyncUnhandledException>());

      var message = contextStub.Items[ControlHelper.AsyncPostBackErrorMessageKey];
      Assert.That (message, Is.StringStarting (@"

            <span><H1>"));
      Assert.That (message, Is.StringContaining ("[ApplicationException: The error]"));
      Assert.That (message, Is.StringEnding (@"<br>

    "));
    }

    [Test]
    public void HandleError_IsCustomErrorEnabledTrue_SetsNonSensitiveErrorMessage ()
    {
      var contextStub = MockRepository.GenerateStub<HttpContextBase>();
      contextStub.Stub (_ => _.Items).Return (new Hashtable());
      contextStub.Stub (_ => _.IsCustomErrorEnabled).Return (true);
      
      var requstStub = MockRepository.GenerateStub<HttpRequestBase>();
      requstStub.Stub (_ => _.ApplicationPath).Return ("Application/Path");
      contextStub.Stub (_ => _.Request).Return (requstStub);

      var handler = new SmartPageAsyncPostBackErrorHandler (contextStub);
      var exceptionMessage = "The error";

      Assert.That (() => handler.HandleError (new ApplicationException (exceptionMessage)), Throws.TypeOf<AsyncUnhandledException>());

      var message = contextStub.Items[ControlHelper.AsyncPostBackErrorMessageKey];
      Assert.That (message, Is.StringStarting (@"

            <span><h1>"));
      Assert.That (message, Is.Not.StringContaining (exceptionMessage));
      Assert.That (message, Is.StringContaining ("Application/Path"));
      Assert.That (message, Is.StringEnding (@"<br/>

    "));
    }

  }
}