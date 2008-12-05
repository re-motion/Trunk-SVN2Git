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
