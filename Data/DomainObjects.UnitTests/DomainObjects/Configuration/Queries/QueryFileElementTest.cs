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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Configuration.Queries
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