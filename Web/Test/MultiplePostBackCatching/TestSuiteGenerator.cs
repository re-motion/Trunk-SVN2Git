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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.Test.MultiplePostBackCatching
{
  public class TestSuiteGenerator
  {
    public static void GenerateTestCases (Page testSuitePage, TableRowCollection rows, string testPage, string testPrefix)
    {
      TestSuiteGenerator testSuiteGenerator = new TestSuiteGenerator (testSuitePage, testPage);
      rows.AddRange (testSuiteGenerator.CreateTestCases (testPrefix));
    }
    
    private readonly Page _testSuitePage;
    private readonly string _testPage;

    public TestSuiteGenerator (Page testSuitePage, string testPage)
    {
      ArgumentUtility.CheckNotNull ("page", testSuitePage);
      ArgumentUtility.CheckNotNullOrEmpty ("testPage", testPage);

      _testSuitePage = testSuitePage;
      _testPage = testPage;
    }

    public TableRow[] CreateTestCases (string prefix)
    {
      TestControlGenerator testControlGenerator = new TestControlGenerator (_testSuitePage, new PostBackEventHandler());
      List<TableRow> rows = new List<TableRow>();

      foreach (Control initialControl in testControlGenerator.GetTestControls (null))
      {
        if (testControlGenerator.IsEnabled (initialControl))
        {
          rows.Add (
              CreateTest (
                  CreateID (prefix, initialControl.ID),
                  UrlUtility.AddParameter (_testSuitePage.ResolveUrl (_testPage), TestExpectationsGenerator.TestCaseParameter, initialControl.ID)));
        }
      }

      return rows.ToArray();
    }

    private TableRow CreateTest (string title, string url)
    {
      TableRow row = new TableRow();
      TableCell cell = new TableCell();

      HyperLink hyperLink = new HyperLink();
      hyperLink.NavigateUrl = url;
      hyperLink.Text = title;

      cell.Controls.Add (hyperLink);
      row.Cells.Add (cell);

      return row;
    }

    private string CreateID (string prefix, string id)
    {
      return (string.IsNullOrEmpty (prefix) ? string.Empty : StringUtility.NullToEmpty (prefix) + "_") + id;
    }
  }
}
