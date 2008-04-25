using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Remotion.Security.UnitTests.TestDomain;
using Remotion.Utilities;
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class SecurityContextTest
  {
    [Test]
    public void CreateSecurityContextWithAbstractRole ()
    {
      Enum[] abstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };
      SecurityContext context = CreateTestSecurityContextWithAbstractRoles (abstractRoles);

      Assert.AreEqual (2, context.AbstractRoles.Length);
      Assert.Contains (new EnumWrapper (TestAbstractRoles.QualityEngineer), context.AbstractRoles);
      Assert.Contains (new EnumWrapper (TestAbstractRoles.Developer), context.AbstractRoles);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       ExpectedMessage = "Enumerated Type 'Remotion.Security.UnitTests.Core.SampleDomain.SimpleEnum' cannot be used as an abstract role. "
        + "Valid abstract roles must have the Remotion.Security.AbstractRoleAttribute applied.\r\nParameter name: abstractRoles")]
    public void CreateSecurityContextWithInvalidAbstractRole ()
    {
      // SimpleEnum does not have AbstractRoleAttribute
      Enum[] abstractRoles = new Enum[] { SimpleEnum.First };
      CreateTestSecurityContextWithAbstractRoles (abstractRoles);
    }

    [Test]
    public void CreateSecurityContextWithNullAbstractRoles ()
    {
      SecurityContext context = CreateTestSecurityContextWithAbstractRoles (null);
      Assert.AreEqual (0, context.AbstractRoles.Length);
    }

    [Test]
    public void CreateSecurityContextWithState ()
    {
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum> ();
      testStates.Add ("Confidentiality", TestSecurityState.Public);
      testStates.Add ("State", TestSecurityState.Secret);

      SecurityContext context = CreateTestSecurityContextWithStates (testStates);

      Assert.AreEqual (new EnumWrapper (TestSecurityState.Public), context.GetState ("Confidentiality"));
      Assert.AreEqual (new EnumWrapper (TestSecurityState.Secret), context.GetState ("State"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       ExpectedMessage = "Enumerated Type 'Remotion.Security.UnitTests.Core.SampleDomain.SimpleEnum' cannot be used as a security state. "
        + "Valid security states must have the Remotion.Security.SecurityStateAttribute applied.\r\nParameter name: states")]
    public void CreateSecurityContextWithInvalidState ()
    {
      // SimpleEnum does not have SecurityStateAttribute
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum> ();
      testStates.Add ("Confidentiality", TestSecurityState.Public);
      testStates.Add ("State", SimpleEnum.Second);

      CreateTestSecurityContextWithStates (testStates);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void CreateSecurityContextWithInvalidType ()
    {
      CreateTestSecurityContextForType (typeof (SimpleType));
    }

    [Test]
    public void GetClassName ()
    {
      SecurityContext context = CreateTestSecurityContext ();
      Assert.AreEqual ("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain", context.Class);
    }

    [Test]
    public void IsStateless_WithStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.IsFalse (context.IsStateless);
    }

    [Test]
    public void IsStateless_WithoutStates ()
    {
      SecurityContext context = CreateTestSecurityContext ();

      Assert.IsTrue (context.IsStateless);
    }

    [Test]
    public void ContainsState_ContextContainsDemandedState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.IsTrue (context.ContainsState ("Confidentiality"));
    }

    [Test]
    public void ContainsState_ContextDoesNotContainDemandedState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.IsFalse (context.ContainsState ("State"));
    }

    [Test]
    public void GetNumberOfStates_WithStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates (states);

      Assert.AreEqual (1, context.GetNumberOfStates ());
    }

    [Test]
    public void GetNumberOfStates_WithoutStates ()
    {
      SecurityContext context = CreateTestSecurityContext ();

      Assert.AreEqual (0, context.GetNumberOfStates ());
    }


    [Test]
    public void Equals_WithNull ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));

      Assert.IsFalse (context.Equals (null));
    }

    [Test]
    public void Equals_WithSame ()
    {
      SecurityContext context = new SecurityContext (typeof (File));

      Assert.IsTrue (context.Equals (context));
    }

    [Test]
    public void Equals_FullyQualified ()
    {
      Dictionary<string, Enum>leftStates = new Dictionary<string, Enum> ();
      leftStates.Add ("State", TestSecurityState.Public);
      leftStates.Add ("Confidentiality", TestSecurityState.Public);
      Enum[] leftAbstractRoles = new Enum[] { TestAbstractRoles.Developer, TestAbstractRoles.QualityEngineer };
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", leftStates, leftAbstractRoles);
      
      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum> ();
      rightStates.Add ("Confidentiality", TestSecurityState.Public);
      rightStates.Add ("State", TestSecurityState.Public);
      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", rightStates, rightAbstractRoles);

      Assert.IsTrue (left.Equals (right));
      Assert.IsTrue (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentClasses ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());
      SecurityContext right = new SecurityContext (typeof (PaperFile), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentOwners ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner1", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());
      SecurityContext right = new SecurityContext (typeof (File), "owner2", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentOwnerGroups ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup1", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup2", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentOwnerTenants ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant1", CreateTwoStates (), CreateTwoAbstractRoles ());
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant2", CreateTwoStates (), CreateTwoAbstractRoles ());

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentStatePropertyLength ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum> ();
      rightStates.Add ("Confidentiality", TestSecurityState.Public);
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", rightStates, CreateTwoAbstractRoles ());

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentStatePropertyNames ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum> ();
      rightStates.Add ("Confidentiality", TestSecurityState.Public);
      rightStates.Add ("State1", TestSecurityState.Public);
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", rightStates, CreateTwoAbstractRoles ());

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentStatePropertyValues ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum> ();
      rightStates.Add ("Confidentiality", TestSecurityState.Public);
      rightStates.Add ("State", TestSecurityState.Confidential);
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", rightStates, CreateTwoAbstractRoles ());

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void Equals_WithDifferentAbstractRoleLength ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer };
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), rightAbstractRoles);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }


    [Test]
    public void Equals_WithDifferentAbstractRoles ()
    {
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), CreateTwoAbstractRoles ());

      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Manager };
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates (), rightAbstractRoles);

      Assert.IsFalse (left.Equals (right));
      Assert.IsFalse (right.Equals (left));
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      SecurityContext left = new SecurityContext (typeof (SecurableObject));
      SecurityContext right = new SecurityContext (typeof (SecurableObject));

      Assert.IsTrue (left.Equals ((object) right));
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));

      Assert.IsFalse (context.Equals ((object) null));
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject));

      Assert.IsFalse (context.Equals (new object ()));
    }

    [Test]
    public void TestGetHashCode ()
    {
      Dictionary<string, Enum> leftStates = new Dictionary<string, Enum> ();
      leftStates.Add ("State", TestSecurityState.Public);
      leftStates.Add ("Confidentiality", TestSecurityState.Public);
      Enum[] leftAbstractRoles = new Enum[] { TestAbstractRoles.Developer, TestAbstractRoles.QualityEngineer };
      SecurityContext left = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", leftStates, leftAbstractRoles);

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum> ();
      rightStates.Add ("Confidentiality", TestSecurityState.Public);
      rightStates.Add ("State", TestSecurityState.Public);
      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };
      SecurityContext right = new SecurityContext (typeof (File), "owner", "ownerGroup", "ownerTenant", rightStates, rightAbstractRoles);

      Assert.AreEqual (left.GetHashCode(), right.GetHashCode());
    }

    [Test]
    public void Serialization ()
    {
      Dictionary<string, Enum> myStates = new Dictionary<string, Enum> ();
      myStates.Add ("State", TestSecurityState.Public);
      myStates.Add ("Confidentiality", TestSecurityState.Public);

      Enum[] myRoles = new Enum[] { TestAbstractRoles.Developer, TestAbstractRoles.QualityEngineer };

      SecurityContext context = new SecurityContext (typeof (SecurableObject), "myOwner", "myGroup", "myTenant", myStates, myRoles);
      SecurityContext deserializedContext = Serializer.SerializeAndDeserialize (context);

      Assert.AreNotSame (context, deserializedContext);
      Assert.AreEqual (context, deserializedContext);
    }

    private static Dictionary<string, Enum> CreateTwoStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum> ();
      states.Add ("Confidentiality", TestSecurityState.Public);
      states.Add ("State", TestSecurityState.Secret);
      return states;
    }

    private static Enum[] CreateTwoAbstractRoles ()
    {
      return new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };;
    }

    private SecurityContext CreateTestSecurityContextForType (Type type)
    {
      return CreateTestSecurityContext (type, null, null);
    }

    private SecurityContext CreateTestSecurityContextWithStates (IDictionary<string, Enum> states)
    {
      return CreateTestSecurityContext (states, null);
    }

    private SecurityContext CreateTestSecurityContextWithAbstractRoles (ICollection<Enum> abstractRoles)
    {
      return CreateTestSecurityContext (null, abstractRoles);
    }

    private SecurityContext CreateTestSecurityContext ()
    {
      return CreateTestSecurityContext (null, null);
    }

    private SecurityContext CreateTestSecurityContext (IDictionary<string, Enum> states, ICollection<Enum> abstractRoles)
    {
      return CreateTestSecurityContext (typeof (File), states, abstractRoles);
    }

    private SecurityContext CreateTestSecurityContext (Type type, IDictionary<string, Enum> states, ICollection<Enum> abstractRoles)
    {
      return new SecurityContext (type, "owner", "group", "tenant", states, abstractRoles);
    }
  }
}
