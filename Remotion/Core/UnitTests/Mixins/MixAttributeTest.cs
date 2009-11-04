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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class MixAttributeTest
  {
    [Test]
    public void Default_MixinKind_IsExtending ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object));
      Assert.That (attribute1.MixinKind, Is.EqualTo (MixinKind.Extending));
    }

    [Test]
    public void Equals_True ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object))
                         {
                             MixinKind = MixinKind.Used,
                             AdditionalDependencies = new[] {typeof (int), typeof (double)},
                             IntroducedMemberVisibility = MemberVisibility.Public,
                             SuppressedMixins = new[] {typeof (float), typeof (DateTime)}
                         };

      var attribute2 = new MixAttribute (typeof (string), typeof (object))
      {
        MixinKind = MixinKind.Used,
        AdditionalDependencies = new[] { typeof (int), typeof (double) },
        IntroducedMemberVisibility = MemberVisibility.Public,
        SuppressedMixins = new[] { typeof (float), typeof (DateTime) }
      };

      Assert.That (attribute1, Is.EqualTo (attribute2));
    }

    [Test]
    public void Equals_False_TargetType ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object));
      var attribute2 = new MixAttribute (typeof (int), typeof (object));

      Assert.That (attribute1, Is.Not.EqualTo (attribute2));
    }

    [Test]
    public void Equals_False_MixinType ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object));
      var attribute2 = new MixAttribute (typeof (string), typeof (int));

      Assert.That (attribute1, Is.Not.EqualTo (attribute2));
    }

    [Test]
    public void Equals_False_MixinKind ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object)) { MixinKind = MixinKind.Extending };
      var attribute2 = new MixAttribute (typeof (string), typeof (object)) { MixinKind = MixinKind.Used };

      Assert.That (attribute1, Is.Not.EqualTo (attribute2));
    }

    [Test]
    public void Equals_False_AdditionalDependencies ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object)) { AdditionalDependencies = new[] {typeof (int)} };
      var attribute2 = new MixAttribute (typeof (string), typeof (object));

      Assert.That (attribute1, Is.Not.EqualTo (attribute2));
    }

    [Test]
    public void Equals_False_IntroducedMemberVisibility ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object)) { IntroducedMemberVisibility = MemberVisibility.Private };
      var attribute2 = new MixAttribute (typeof (string), typeof (object)) { IntroducedMemberVisibility = MemberVisibility.Public };

      Assert.That (attribute1, Is.Not.EqualTo (attribute2));
    }

    [Test]
    public void Equals_False_SuppressedMixins ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object)) { SuppressedMixins = new[] {typeof (int)} };
      var attribute2 = new MixAttribute (typeof (string), typeof (object));

      Assert.That (attribute1, Is.Not.EqualTo (attribute2));
    }

    [Test]
    public void EnsureAllPropertiesAreTested()
    {
      var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
      var properties = from p in typeof (MixAttribute).GetProperties (bindingFlags)
                       where p.GetSetMethod () != null
                       select p;
      var fields = typeof (MixAttribute).GetFields (bindingFlags);
      var ctorArgs = from ctor in typeof (MixAttribute).GetConstructors (bindingFlags)
                     from parameter in ctor.GetParameters()
                     select parameter;

      Assert.That (properties.Count () + fields.Count () + ctorArgs.Count (), Is.EqualTo (6), "New equality tests are likely needed.");
    }

    [Test]
    public void GetHashCode_Equal ()
    {
      var attribute1 = new MixAttribute (typeof (string), typeof (object))
      {
        MixinKind = MixinKind.Used,
        AdditionalDependencies = new[] { typeof (int), typeof (double) },
        IntroducedMemberVisibility = MemberVisibility.Public,
        SuppressedMixins = new[] { typeof (float), typeof (DateTime) }
      };

      var attribute2 = new MixAttribute (typeof (string), typeof (object))
      {
        MixinKind = MixinKind.Used,
        AdditionalDependencies = new[] { typeof (int), typeof (double) },
        IntroducedMemberVisibility = MemberVisibility.Public,
        SuppressedMixins = new[] { typeof (float), typeof (DateTime) }
      };

      Assert.That (attribute1.GetHashCode (), Is.EqualTo (attribute2.GetHashCode ()));
    }
  }
}
