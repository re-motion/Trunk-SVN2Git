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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.Definitions.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.Definitions.Building
{
  [TestFixture]
  public class RequirementsAnalyzerTest
  {
    private RequirementsAnalyzer _thisAnalyzer;
    private RequirementsAnalyzer _baseAnalyzer;

    [SetUp]
    public void SetUp ()
    {
      _thisAnalyzer = new RequirementsAnalyzer (MixinGenericArgumentFinder.ThisArgumentFinder);
      _baseAnalyzer = new RequirementsAnalyzer (MixinGenericArgumentFinder.BaseArgumentFinder);
    }

    [Test]
    public void GetRequirements_NoGenericArgumentFound ()
    {
      var result = _baseAnalyzer.GetRequirements (typeof (BT3Mixin2));
      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetRequirements_FollowsConstraintsOfGenericParameter ()
    {
      var result = _thisAnalyzer.GetRequirements (typeof (BT3Mixin3<,>));
      Assert.That (result, Is.EquivalentTo (new[] { typeof (IBaseType33) }));
    }

    [Test]
    public void GetRequirements_NonGenericParameter_NonInterface ()
    {
      var result = _thisAnalyzer.GetRequirements (typeof (BT3Mixin3<BaseType3, BaseType3>));
      Assert.That (result, Is.EquivalentTo (new[] { typeof (BaseType3) }));
    }

    [Test]
    public void GetRequirements_NonGenericParameter_Interface ()
    {
      var result = _thisAnalyzer.GetRequirements (typeof (BT3Mixin3<IBaseType33, BaseType3>));
      Assert.That (result, Is.EquivalentTo (new[] { typeof (IBaseType33) }));
    }

    [Test]
    public void GetRequirements_InterfaceIsRecursed ()
    {
      var result = _thisAnalyzer.GetRequirements (typeof (BT3Mixin3<IBaseType34, BaseType3>));
      Assert.That (result, Is.EquivalentTo (new[] { typeof (IBaseType33), typeof (IBaseType34) }));
    }

    [Test]
    public void GetRequirements_InterfaceIsRecursed_Twice ()
    {
      var result = _thisAnalyzer.GetRequirements (typeof (BT3Mixin3<ICBaseType3BT3Mixin4, BaseType3>));
      Assert.That (result, Is.EquivalentTo (new[] { 
          typeof (ICBaseType3BT3Mixin4), 
          typeof (IBT3Mixin4), 
          typeof (ICBaseType3), 
          typeof (IBaseType31), 
          typeof (IBaseType32), 
          typeof (IBaseType33), 
          typeof (IBaseType34), 
          typeof (IBaseType35) }));
    }

    [Test]
    public void GetRequirements_RemovesDuplicates ()
    {
      var result = _thisAnalyzer.GetRequirements (typeof (GenericMixinWithSameRequirementTwice<>));
      Assert.That (result, Is.EquivalentTo (new[] { typeof (IBaseType33), typeof (IBaseType34) }));
      Assert.That (result.Length, Is.EqualTo (2));
    }
  }
}
