// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Queries
{
  [TestFixture]
  public class QueryFileElementTest
  {
    [Test]
    public void GetRootedPath_WithFullPath_ReturnsFullPath ()
    {
      string path = @"c:\foo\bar.txt";
      Assert.That (QueryFileElement.GetRootedPath (path), Is.EqualTo (path));
    }

    [Test]
    public void GetRootedPath_WithRootedRelativePath_ReturnsFullPath ()
    {
      string path = @"\foo\bar.txt";
      string fullPath = Path.GetFullPath (path);
      Assert.That (QueryFileElement.GetRootedPath (path), Is.EqualTo (fullPath));
    }

    [Test]
    public void GetRootedPath_WithUnrootedPath_ReturnsPathRelativeToAppBase ()
    {
      string path = @"foo\bar.txt";
      string fullPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, @"foo\bar.txt");
      Assert.That (QueryFileElement.GetRootedPath (path), Is.EqualTo (fullPath));
    }

    [Test]
    public void GetRootedPath_WithUnrootedPath_ReturnsPathRelativeToAppBase_InSeparateAddDomain ()
    {
      AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
      setup.ApplicationBase = @"c:\";
      setup.DynamicBase = Path.GetTempPath ();
      new AppDomainRunner (setup, delegate
      {
        string path = @"foo\bar.txt";
        string fullPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, @"foo\bar.txt");
        Assert.That (QueryFileElement.GetRootedPath (path), Is.EqualTo (fullPath));
      }).Run();
    }
  }
}
