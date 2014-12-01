﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FileDownloadTest : IntegrationTest
  {
    [Test]
    public void Test ()
    {
      if (WebTestingConfiguration.Current.BrowserIsInternetExplorer())
        Assert.Ignore ("Not working on TeamCity yet.");

      const string fileName = "SampleFile.txt";

      var downloadHelper = NewDownloadHelper (fileName);
      downloadHelper.AssertFileDoesNotExistYet();

      var home = Start();
      var button = home.Scope.FindId ("body_DownloadButton");
      button.Click();

      downloadHelper.PerformDownload();
      downloadHelper.DeleteFile();
    }

    private RemotionPageObject Start ()
    {
      return Start ("FileDownloadTest.aspx");
    }
  }
}