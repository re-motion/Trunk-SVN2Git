// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ConcreteMixinTypeIdentifierTest
  {
    private MemberInfo _externalOverrider1;
    private MemberInfo _externalOverrider2;
    private MemberInfo _wrappedProtectedMember1;
    private MemberInfo _wrappedProtectedMember2;

    [SetUp]
    public void SetUp ()
    {
      _externalOverrider1 = typeof (ClassOverridingSingleMixinMethod).GetMethod ("AbstractMethod");
      _externalOverrider2 = typeof (ClassOverridingMixinMembers).GetProperty ("AbstractProperty");
      _wrappedProtectedMember1 = typeof (MixinWithProtectedOverrider).GetMethod ("VirtualMethod", BindingFlags.NonPublic | BindingFlags.Instance);
      _wrappedProtectedMember2 = typeof (MixinWithProtectedOverrider).GetProperty ("VirtualProperty", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [Test]
    public void Equals_True ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1), 
          SetOf (_externalOverrider1, _externalOverrider2 ), 
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));

      Assert.That (one, Is.EqualTo (two));
    }

    [Test]
    public void Equals_True_OrderOfExternalOverridersIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider2, _externalOverrider1 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));

      Assert.That (one, Is.EqualTo (two));
    }

    [Test]
    public void Equals_True_OrderOfWrappedProtectedMembersIsIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember2, _wrappedProtectedMember1 ));

      Assert.That (one, Is.EqualTo (two));
    }

    [Test]
    public void Equals_False_DifferentTypes ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin2),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));

      Assert.That (one, Is.Not.EqualTo (two));
    }

    [Test]
    public void Equals_False_DifferentExternalOverriders ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));

      Assert.That (one, Is.Not.EqualTo (two));
    }

    [Test]
    public void Equals_False_DifferentWrappedProtectedMembers ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1 ));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2 ),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2 ));

      Assert.That (one, Is.Not.EqualTo (two));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2));

      Assert.That (one.GetHashCode (), Is.EqualTo (two.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_EqualObjects_OrderOfExternalOverridersIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider2, _externalOverrider1),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2));

      Assert.That (one.GetHashCode (), Is.EqualTo (two.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_EqualObjects_OrderOfWrappedProtectedMembersIsIrrelevant ()
    {
      var one = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2),
          SetOf (_wrappedProtectedMember1, _wrappedProtectedMember2));
      var two = new ConcreteMixinTypeIdentifier (
          typeof (BT1Mixin1),
          SetOf (_externalOverrider1, _externalOverrider2),
          SetOf (_wrappedProtectedMember2, _wrappedProtectedMember1));

      Assert.That (one.GetHashCode (), Is.EqualTo (two.GetHashCode ()));
    }

    private HashSet<T> SetOf<T> (params T[] items)
    {
      return new HashSet<T> (items);
    }
  }
}