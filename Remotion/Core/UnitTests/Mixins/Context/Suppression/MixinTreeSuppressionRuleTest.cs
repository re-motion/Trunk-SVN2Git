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
using NUnit.Framework;
using Remotion.Mixins.Context.Suppression;
using Remotion.Mixins.Context;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Context.Suppression
{
  [TestFixture]
  public class MixinTreeSuppressionRuleTest
  {
    [Test]
    public void RemoveAffectedMixins_LeavesUnrelatedType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (string));
      var dictionary = CreateContextDictionary (typeof (int), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (int), typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesSameType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (string));
      var dictionary = CreateContextDictionary (typeof (string), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesDerivedType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (object));
      var dictionary = CreateContextDictionary (typeof (string), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.Empty);
    }

    [Test]
    public void RemoveAffectedMixins_KeepsBaseType ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (string));
      var dictionary = CreateContextDictionary (typeof (object), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (object), typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesGenericTypeSpecialization ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (List<>));
      var dictionary = CreateContextDictionary (typeof (List<string>), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (double) }));
    }

    [Test]
    public void RemoveAffectedMixins_RemovesDerivedGenericTypeSpecialization ()
    {
      var rule = new MixinTreeSuppressionRule (typeof (GenericMixinWithVirtualMethod<>));
      var dictionary = CreateContextDictionary (typeof (DerivedGenericMixin<string>), typeof (double));

      rule.RemoveAffectedMixins (dictionary);

      Assert.That (dictionary.Keys, Is.EquivalentTo (new[] { typeof (double) }));
    }

    private Dictionary<Type, MixinContext> CreateContextDictionary (params Type[] types)
    {
      var dictionary = new Dictionary<Type, MixinContext> ();
      foreach (var type in types)
      {
        dictionary.Add (type, new MixinContext (MixinKind.Extending, type, MemberVisibility.Private));
      }
      return dictionary;
    }
  }
}