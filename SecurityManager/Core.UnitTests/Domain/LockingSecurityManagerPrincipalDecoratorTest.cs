// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class LockingSecurityManagerPrincipalDecoratorTest : DomainTest
  {
    private OrganizationalStructureTestHelper _helper;
    private ISecurityManagerPrincipal _innerPrincipalMock;
    private LockingSecurityManagerPrincipalDecorator _decorator;

    public override void SetUp ()
    {
      base.SetUp();

      _helper = new OrganizationalStructureTestHelper();
      _helper.Transaction.EnterNonDiscardingScope();

      _innerPrincipalMock = MockRepository.GenerateMock<ISecurityManagerPrincipal>();
      _decorator = new LockingSecurityManagerPrincipalDecorator (_innerPrincipalMock);
    }

    [Test]
    public void Get_Members ()
    {
      var tenant = _helper.CreateTenant ("TheTenant", "Tenant UID");
      var group = _helper.CreateGroup ("TheGroup", "Group UID", null, tenant);
      var user = _helper.CreateUser ("UserName", "FN", "LN", null, group, tenant);

      var substitution = Substitution.NewObject();
      substitution.SubstitutingUser = user;
      substitution.SubstitutedUser = _helper.CreateUser ("SubstitutingUser", "FN", "LN", null, group, tenant);

      ExpectSynchronizedDelegation (mock => mock.Tenant, TenantProxy.Create (tenant));
      ExpectSynchronizedDelegation (mock => mock.User, UserProxy.Create (user));
      ExpectSynchronizedDelegation (mock => mock.Substitution, SubstitutionProxy.Create (substitution));
    }

    [Test]
    public void Refresh ()
    {
      ExpectSynchronizedDelegation (mock => mock.Refresh());
    }

    [Test]
    public void GetTenants ()
    {
      ExpectSynchronizedDelegation (mock => mock.GetTenants (true), new TenantProxy[0]);
    }

    [Test]
    public void GetActiveSubstitutions ()
    {
      ExpectSynchronizedDelegation (mock => mock.GetActiveSubstitutions(), new SubstitutionProxy[0]);
    }

    [Test]
    public void GetSecurityPrincipal ()
    {
      ExpectSynchronizedDelegation (mock => mock.GetSecurityPrincipal (), MockRepository.GenerateStub<ISecurityPrincipal>());
    }

    [Test]
    public void Serialization ()
    {
      ISecurityManagerPrincipal decorator =
          new LockingSecurityManagerPrincipalDecorator (
              new SecurityManagerPrincipal (
                  new ObjectID (typeof (Tenant), Guid.NewGuid()),
                  new ObjectID (typeof (User), Guid.NewGuid()),
                  null));

      var deserialized = Serializer.SerializeAndDeserialize (decorator);

      Assert.That (deserialized.IsNull, Is.EqualTo (decorator.IsNull));
    }

    [Test]
    public void Get_IsNull ()
    {
      _innerPrincipalMock.Stub (mock => mock.IsNull).Return (false);

      Assert.That (((INullObject) _decorator).IsNull, Is.False);
    }

    private void ExpectSynchronizedDelegation<TResult> (Func<ISecurityManagerPrincipal, TResult> action, TResult fakeResult)
    {
      _innerPrincipalMock
          .Expect (mock => action (mock))
          .Return (fakeResult)
          .WhenCalled (mi => LockingSecurityManagerPrincipalDecoratorTestHelper.CheckLockIsHeld (_decorator));
      _innerPrincipalMock.Replay();

      TResult actualResult = action (_decorator);

      _innerPrincipalMock.VerifyAllExpectations();
      Assert.That (actualResult, Is.SameAs (fakeResult));
    }

    private void ExpectSynchronizedDelegation (Action<ISecurityManagerPrincipal> action)
    {
      _innerPrincipalMock
          .Expect (action)
          .WhenCalled (mi => LockingSecurityManagerPrincipalDecoratorTestHelper.CheckLockIsHeld (_decorator));
      _innerPrincipalMock.Replay ();

      action (_decorator);

      _innerPrincipalMock.VerifyAllExpectations ();
    }
  }

  public static class LockingSecurityManagerPrincipalDecoratorTestHelper
  {
    public static void CheckLockIsHeld (LockingSecurityManagerPrincipalDecorator lockingDecorator)
    {
      ArgumentUtility.CheckNotNull ("lockingDecorator", lockingDecorator);

      bool lockAcquired = TryAcquireLockFromOtherThread (lockingDecorator);
      Assert.That (lockAcquired, Is.False, "Parallel thread should have been blocked.");
    }

    public static void CheckLockIsNotHeld (LockingSecurityManagerPrincipalDecorator lockingDecorator)
    {
      ArgumentUtility.CheckNotNull ("lockingDecorator", lockingDecorator);

      bool lockAcquired = TryAcquireLockFromOtherThread (lockingDecorator);
      Assert.That (lockAcquired, Is.True, "Parallel thread should not have been blocked.");
    }

    private static bool TryAcquireLockFromOtherThread (LockingSecurityManagerPrincipalDecorator lockingDecorator)
    {
      var lockObject = PrivateInvoke.GetNonPublicField (lockingDecorator, "_lock");
      Assert.That (lockObject, Is.Not.Null);

      bool lockAcquired = true;
      ThreadRunner.Run (() => lockAcquired = Monitor.TryEnter (lockObject, TimeSpan.Zero));
      return lockAcquired;
    }
  }
}