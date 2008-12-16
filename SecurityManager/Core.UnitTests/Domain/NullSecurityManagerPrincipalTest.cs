// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.
//
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class NullSecurityManagerPrincipalTest
  {
    [Test]
    public void Get_Members ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;

      Assert.That (principal.Tenant, Is.Null);
      Assert.That (principal.User, Is.Null);
      Assert.That (principal.Substitution, Is.Null);
    }

    [Test]
    public void Refresh ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;
      principal.Refresh();
    }

    [Test]
    public void GetSecurityPrincipal ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;

      Assert.That (principal.GetSecurityPrincipal().IsNull, Is.True);
    }

    [Test]
    public void Serialization ()
    {
      var principal = SecurityManagerPrincipal.Null;
     
      var deserializedPrincipal = Serializer.SerializeAndDeserialize (principal);

      Assert.That (principal, Is.SameAs (deserializedPrincipal));
    }

    [Test]
    public void Get_IsNull ()
    {
      ISecurityManagerPrincipal principal = SecurityManagerPrincipal.Null;
      Assert.That (principal.IsNull, Is.True);
    }
  }
}