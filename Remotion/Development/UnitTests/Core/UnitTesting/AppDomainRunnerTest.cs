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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class AppDomainRunnerTest
  {
    [Test]
    public void ArgumentsArePassedInCorrectly ()
    {
      AppDomainRunner.Run (delegate (object[] args)
        {
          Assert.AreEqual (2, args.Length);
          Assert.AreEqual (args[0], "Foo");
          Assert.AreEqual (args[1], 4);
        }, "Foo", 4);

      AppDomainRunner.Run (delegate (object[] args)
        {
          Assert.AreEqual (0, args.Length);
        });
    }

    [Test]
    public void SpecificAppBase ()
    {
      AppDomainRunner.Run (@"C:\", delegate (object[] args)
      {
        Assert.That (AppDomain.CurrentDomain.BaseDirectory, Is.EqualTo (@"C:\"));
      });
    }

    [Test]
    public void AppDomainIsCreated ()
    {
      AppDomain current = AppDomain.CurrentDomain;
      AppDomainRunner.Run (
          delegate (object[] args)
          {
            Assert.That (AppDomain.CurrentDomain.FriendlyName, Is.EqualTo ("AppDomainRunner - AppDomain"));
            Assert.AreNotSame (args[0], AppDomain.CurrentDomain);
          },
          current);
    }

    [Test]
    public void TypesFromCurrentAssemblyCanBeAccessed ()
    {
      AppDomainRunner.Run (delegate { new AppDomainRunnerTest(); });
    }

    [Test]
    public void TypesFromCurrentAssemblyCanBeAccessed_EvenWithDifferentBaseDirectory ()
    {
      AppDomainRunner.Run (@"c:\", delegate { new AppDomainRunnerTest (); });
    }

    [Test]
    public void TypesFromRunnerBaseAssemblyCanBeAccessed_EvenWithDifferentBaseDirectory ()
    {
      AppDomainRunner.Run (@"c:\", delegate { Dev.Null = typeof (AppDomainRunnerBase); });
    }

    [Test]
    public void DoesntChangeCurrentSetup ()
    {
      string dynamicBaseBefore = AppDomain.CurrentDomain.SetupInformation.DynamicBase;
      AppDomainRunner.Run (delegate { new AppDomainRunnerTest (); });
      Assert.AreEqual (dynamicBaseBefore, AppDomain.CurrentDomain.SetupInformation.DynamicBase);
    }
  }
}
