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
using System.Linq;
using System.Web;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine.ExceptionHandling
{
  public partial class TestForm : WxePage
  {
    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      Stack.Text = SeparatedStringBuilder.Build (
          "</br>",
          CurrentFunction.CreateSequence (f => f.ParentFunction).Select (f => f.GetType().Name));
    }

    protected void OpenSubFunctionButton_Click (object sender, EventArgs e)
    {
      if (!IsReturningPostBack)
        ExecuteFunction (new TestFunction(), WxeCallArguments.Default);
    }

    protected void ThrowExceptionButton_Click (object sender, EventArgs e)
    {
      ThrowInnerExceptionWithNesting();
    }

    protected void ThrowHttpExceptionButton_Click (object sender, EventArgs e)
    {
      try
      {
        ThrowInnerExceptionWithNesting();
      }
      catch (Exception ex)
      {
        throw new HttpException ("Test outer exception", ex);
      }
    }
    
    protected void ThrowExceptionFromSubFunctionButton_Click (object sender, EventArgs e)
    {
      ExecuteFunction (new ThrowingFunction(), WxeCallArguments.Default);
    }
    
    protected void ThrowExceptionForMissingPageButton_Click (object sender, EventArgs e)
    {
      ExecuteFunction (new MissingPageFunction(), WxeCallArguments.Default);
    }

    private void ThrowInnerExceptionWithNesting ()
    {
      try
      {
        ThrowInnerException();
      }
      catch (Exception ex)
      {
        throw new ApplicationException ("Test inner exception with nesting", ex);
      }
    }

    private void ThrowInnerException ()
    {
      throw new ApplicationException ("Test inner exception");
    }
  }
}