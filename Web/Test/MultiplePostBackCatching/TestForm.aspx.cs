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
using Remotion.Web.UI;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public partial class TestForm : TestBasePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      TestExpectationsGenerator.GenerateExpectations (this, TestTable.Rows, "~/MultiplePostbackCatching/SutForm.aspx");
      HtmlHeadAppender.Current.SetTitle (TestExpectationsGenerator.GetTestCaseUrlParameter (this) ?? "All Multiple Postback Catcher Tests");
    }
  }
}
