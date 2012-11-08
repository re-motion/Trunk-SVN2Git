// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities.DependencySort;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Definitions.DependencySorting
{
  [TestFixture]
  public class MixinDefinitionSorterTest
  {
    private TargetClassDefinition _targetClassDefinition;

    private MixinDefinition _mixinDefinition1;
    private MixinDefinition _mixinDefinition2;
    private MixinDefinition _mixinDefinition3;
    private MixinDefinition _mixinDefinition4;
    private HashSet<MixinDefinition>[] _fakeGroupings;

    [SetUp]
    public void SetUp ()
    {
      _targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (NullTarget));

      _mixinDefinition4 = DefinitionObjectMother.CreateMixinDefinition (_targetClassDefinition, typeof (NullMixin4));
      _mixinDefinition2 = DefinitionObjectMother.CreateMixinDefinition (_targetClassDefinition, typeof (NullMixin2));
      _mixinDefinition1 = DefinitionObjectMother.CreateMixinDefinition (_targetClassDefinition, typeof (NullMixin));
      _mixinDefinition3 = DefinitionObjectMother.CreateMixinDefinition (_targetClassDefinition, typeof (NullMixin3));

      _fakeGroupings = new[] { 
          new HashSet<MixinDefinition> { _mixinDefinition1, _mixinDefinition4  }, 
          new HashSet<MixinDefinition> { _mixinDefinition3, _mixinDefinition2 } };
    }

    [Test]
    public void SortMixins_PartitionsAndSortsAlphabetically ()
    {
      var mixinDefinitions = new[] { _mixinDefinition4, _mixinDefinition2, _mixinDefinition1, _mixinDefinition3 };

      var fakeSortedGroup1 = new[] { _mixinDefinition4, _mixinDefinition1 };
      var fakeSortedGroup2 = new[] { _mixinDefinition3, _mixinDefinition2 };

      var mockRepository = new MockRepository ();

      var innerGrouperMock = mockRepository.StrictMock<IDependentMixinGrouper>();
      innerGrouperMock.Expect (mock => mock.GroupMixins (mixinDefinitions)).Return (_fakeGroupings);

      var innerSorterMock = mockRepository.StrictMock<IDependentObjectSorter<MixinDefinition>>();
      using (mockRepository.Ordered ())
      {
        innerSorterMock.Expect (mock => mock.SortDependencies (_fakeGroupings[0])).Return (fakeSortedGroup1);
        innerSorterMock.Expect (mock => mock.SortDependencies (_fakeGroupings[1])).Return (fakeSortedGroup2);
      }

      mockRepository.ReplayAll();

      var sorter = new MixinDefinitionSorter (innerGrouperMock, innerSorterMock);
      var sorted = sorter.SortMixins (mixinDefinitions);

      mockRepository.VerifyAll ();

      Assert.That (sorted, Is.EqualTo (new[] { _mixinDefinition3, _mixinDefinition2, _mixinDefinition4, _mixinDefinition1 }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The following group of mixins contains circular dependencies:\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin',\r\n"
        + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin4'.")]
    public void SortMixins_CircularDependencies ()
    {
      var mockRepository = new MockRepository ();

      var innerGrouperMock = mockRepository.StrictMock<IDependentMixinGrouper> ();
      innerGrouperMock.Expect (mock => mock.GroupMixins (_targetClassDefinition.Mixins)).Return (_fakeGroupings);

      var innerSorterMock = mockRepository.StrictMock<IDependentObjectSorter<MixinDefinition>> ();
      using (mockRepository.Ordered ())
      {
        innerSorterMock
            .Expect (mock => mock.SortDependencies (_fakeGroupings[0]))
            .Throw (new CircularDependenciesException<MixinDefinition> ("bla", _fakeGroupings[0]));
      }

      mockRepository.ReplayAll ();

      var sorter = new MixinDefinitionSorter (innerGrouperMock, innerSorterMock);
      sorter.SortMixins (_targetClassDefinition.Mixins);
    }
  }
}
