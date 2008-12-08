// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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
