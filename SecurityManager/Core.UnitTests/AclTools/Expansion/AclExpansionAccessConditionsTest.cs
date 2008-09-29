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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Diagnostics;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.Domain.Metadata;
using System.Reflection;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion
{
  [TestFixture]
  public class AclExpansionAccessConditionsTest : AclToolsTestBase
  {
    [Test]
    public void DefaultCtor ()
    {
      var accessConditions = new AclExpansionAccessConditions ();
      Assert.That (accessConditions.AbstractRole, Is.Null);
      Assert.That (accessConditions.OnlyIfAbstractRoleMatches, Is.EqualTo(false));
      Assert.That (accessConditions.OnlyIfGroupIsOwner, Is.EqualTo (false));
      Assert.That (accessConditions.OnlyIfTenantIsOwner, Is.EqualTo (false));
      Assert.That (accessConditions.OnlyIfUserIsOwner, Is.EqualTo (false));
    }

    [Test]
    public void Equals ()
    {
      BooleanMemberTest ((aeac,b) => aeac.OnlyIfAbstractRoleMatches = b);
      BooleanMemberTest ((aeac, b) => aeac.OnlyIfGroupIsOwner = b);
      BooleanMemberTest ((aeac, b) => aeac.OnlyIfTenantIsOwner = b);
      BooleanMemberTest ((aeac, b) => aeac.OnlyIfUserIsOwner = b);
    }


    [Test]
    public void Equals2 ()
    {
      MemberTest (new PropertyObject<AclExpansionAccessConditions, bool> (x => x.OnlyIfAbstractRoleMatches),true);
      MemberTest (new PropertyObject<AclExpansionAccessConditions, bool> (x => x.OnlyIfGroupIsOwner), true);
      MemberTest (new PropertyObject<AclExpansionAccessConditions, bool> (x => x.OnlyIfTenantIsOwner), true);
      MemberTest (new PropertyObject<AclExpansionAccessConditions, bool> (x => x.OnlyIfUserIsOwner), true);
      MemberTest (new PropertyObject<AclExpansionAccessConditions, AbstractRoleDefinition> (x => x.AbstractRole), TestHelper.CreateAbstractRoleDefinition("titatutest",11235) );
    }


    [Test]
    public void ToText ()
    {
      var accessConditions = new AclExpansionAccessConditions ();
      var result = To.String.e (accessConditions).CheckAndConvertToString ();
      //To.Console.s (result);
      Assert.That (result, NUnit.Framework.SyntaxHelpers.Text.Contains ("userMustOwn=False,groupMustOwn=False,tenantMustOwn=False,abstractRoleMustMatche=False,abstractRole=null"));
    }




    private void BooleanMemberTest (Action<AclExpansionAccessConditions, bool> setBoolProperty)
    {
      var accessConditions0 = new AclExpansionAccessConditions ();
      var accessConditions1 = new AclExpansionAccessConditions ();
      Assert.That (accessConditions0.Equals (accessConditions1), Is.True);
      setBoolProperty (accessConditions1, true);
      Assert.That (accessConditions0.Equals (accessConditions1), Is.False);
    }


    private void MemberTest<TProperty> (PropertyObject<AclExpansionAccessConditions, TProperty> boolProperty, TProperty notEqualValue)
    {
      var accessConditions0 = new AclExpansionAccessConditions ();
      var accessConditions1 = new AclExpansionAccessConditions ();
      Assert.That (accessConditions0.Equals (accessConditions1), Is.True);
      boolProperty.Set (accessConditions1, notEqualValue);
      Assert.That (accessConditions0.Equals (accessConditions1), Is.False);
    }


    public class PropertyObject<TClass, TProperty>
    {
      private readonly PropertyInfo _propertyInfo;

      public PropertyObject (Expression<Func<TClass, TProperty>> propertyLambda)
      {
        _propertyInfo = (PropertyInfo) ((MemberExpression) propertyLambda.Body).Member;
      }

      public TProperty Get (TClass obj)
      {
        return (TProperty) _propertyInfo.GetValue (obj, null);
      }

      public void Set (TClass obj, TProperty value)
      {
        _propertyInfo.SetValue (obj, value, null);
      }

    }


  }
}
