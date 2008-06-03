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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using System.Reflection;

namespace Remotion.Development.UnitTests.UnitTesting
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
    public void AppDomainIsCreated ()
    {
      AppDomain current = AppDomain.CurrentDomain;
      AppDomainRunner.Run (delegate (object[] args) { Assert.AreNotSame (args[0], AppDomain.CurrentDomain); }, current);
    }

    [Test]
    public void TypesFromCurrentAssemblyCanBeAccessed ()
    {
      AppDomainRunner.Run (delegate { new AppDomainRunnerTest(); });
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
