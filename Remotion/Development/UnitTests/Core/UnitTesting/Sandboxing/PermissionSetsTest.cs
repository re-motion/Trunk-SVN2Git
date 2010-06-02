// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Net;
using System.Net.Mail;
using System.Security.Permissions;
using System.Web;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Sandboxing;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class PermissionSetsTest
  {
    [Test]
    public void GetMediumTrust ()
    {
      var mediumTrustPermissions = PermissionSets.GetMediumTrust (Environment.GetEnvironmentVariable ("TEMP"), Environment.MachineName);

      Assert.That (mediumTrustPermissions.Length, Is.EqualTo (11));
      Assert.That (((AspNetHostingPermission) mediumTrustPermissions[0]).Level, Is.EqualTo (AspNetHostingPermissionLevel.Medium));
      Assert.That (((DnsPermission) mediumTrustPermissions[1]).IsUnrestricted(), Is.True);
      Assert.That (mediumTrustPermissions[2], Is.TypeOf (typeof (EnvironmentPermission))); //TODO!?
      Assert.That (mediumTrustPermissions[3], Is.TypeOf (typeof (FileIOPermission))); //TODO!?
      Assert.That (
          ((IsolatedStorageFilePermission) mediumTrustPermissions[4]).UsageAllowed, Is.EqualTo (IsolatedStorageContainment.AssemblyIsolationByUser));
      Assert.That (((IsolatedStorageFilePermission) mediumTrustPermissions[4]).UserQuota, Is.EqualTo (9223372036854775807L));
      Assert.That (((PrintingPermission) mediumTrustPermissions[5]).Level, Is.EqualTo (PrintingPermissionLevel.DefaultPrinting));
      Assert.That (
          ((SecurityPermission) mediumTrustPermissions[6]).Flags,
          Is.EqualTo (
              SecurityPermissionFlag.Assertion | SecurityPermissionFlag.Execution | SecurityPermissionFlag.ControlThread
              | SecurityPermissionFlag.ControlPrincipal | SecurityPermissionFlag.RemotingConfiguration));
      Assert.That (((SmtpPermission) mediumTrustPermissions[7]).Access, Is.EqualTo (SmtpAccess.Connect));
      Assert.That (((SqlClientPermission) mediumTrustPermissions[8]).IsUnrestricted(), Is.True);
      Assert.That (mediumTrustPermissions[9], Is.TypeOf(typeof(WebPermission)));
      Assert.That (((ReflectionPermission) mediumTrustPermissions[10]).Flags, Is.EqualTo(ReflectionPermissionFlag.RestrictedMemberAccess));
    }
  }
}