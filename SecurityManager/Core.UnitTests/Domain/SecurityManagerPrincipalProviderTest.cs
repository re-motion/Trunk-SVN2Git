// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Specialized;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SecurityManagerPrincipalProviderTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    public override void TearDown ()
    {
      base.TearDown();
      SecurityManagerPrincipal.Current = SecurityManagerPrincipal.Null;
    }

    [Test]
    public void Initialize ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      SecurityManagerPrincipalProvider provider = new SecurityManagerPrincipalProvider ("Provider", config);

      Assert.That (provider.Name, Is.EqualTo ("Provider"));
      Assert.That ( provider.Description, Is.EqualTo("The Description"));
    }

    [Test]
    public void GetPrincipal ()
    {
      ISecurityPrincipal securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal>();
      ISecurityManagerPrincipal securityManagerPrincipalStub = MockRepository.GenerateStub<ISecurityManagerPrincipal>();
      securityManagerPrincipalStub.Stub (stub => stub.GetSecurityPrincipal()).Return (securityPrincipalStub);
      SecurityManagerPrincipal.Current = securityManagerPrincipalStub;

      SecurityManagerPrincipalProvider provider = new SecurityManagerPrincipalProvider ();

      Assert.That (provider.GetPrincipal(), Is.SameAs (securityPrincipalStub));
    }
  }
}
