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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpanderApplicationSettingsTest
  {
    [Test]
    public void DefaultValuesTest ()
    {
      var settings = new AclExpanderApplicationSettings();
      Assert.That (settings.Directory, Is.EqualTo ("."));
      Assert.That (settings.UseMultipleFileOutput, Is.EqualTo (false));
      Assert.That (settings.UserFirstName, Is.Null);
      Assert.That (settings.UserLastName, Is.Null);
      Assert.That (settings.UserName, Is.Null);
    }

    [Test]
    public void ToTextTest ()
    {
      var settings = new AclExpanderApplicationSettings ();
      Assert.That (To.String.e (settings).CheckAndConvertToString (), Is.EqualTo (@"(user=null,last=null,first=null,dir=""."",culture=""de-AT"",multifile=False,verbose=False)"));
    }

  }
}