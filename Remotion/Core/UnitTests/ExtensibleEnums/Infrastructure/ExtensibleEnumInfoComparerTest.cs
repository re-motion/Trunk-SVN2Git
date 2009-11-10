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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Globalization;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;

namespace Remotion.UnitTests.ExtensibleEnums.Infrastructure
{
  [TestFixture]
  public class ExtensibleEnumInfoComparerTest
  {
    private MethodInfo _fakeMethod;
    private NullResourceManager _fakeResourceManager;
    private ExtensibleEnumInfoComparer<ExtensibleEnumInfo<Planet>> _comparer;

    [SetUp]
    public void SetUp ()
    {
      _fakeMethod = typeof (ColorExtensions).GetMethod ("Red");
      _fakeResourceManager = NullResourceManager.Instance;
      _comparer = ExtensibleEnumInfoComparer<ExtensibleEnumInfo<Planet>>.Instance;
    }

    [Test]
    public void Compare_PositionalKeys ()
    {
      var x = new ExtensibleEnumInfo<Planet> (new Planet ("x"), _fakeMethod, _fakeResourceManager, 2.0);
      var y = new ExtensibleEnumInfo<Planet> (new Planet ("y"), _fakeMethod, _fakeResourceManager, 1.0);

      Assert.That (_comparer.Compare (x, y), Is.EqualTo (1));
      Assert.That (_comparer.Compare (y, x), Is.EqualTo (-1));
    }

    [Test]
    public void Compare_EqualPositionalKeys_Alphabetic ()
    {
      var x = new ExtensibleEnumInfo<Planet> (new Planet ("x"), _fakeMethod, _fakeResourceManager, 1.0);
      var y = new ExtensibleEnumInfo<Planet> (new Planet ("y"), _fakeMethod, _fakeResourceManager, 1.0);

      Assert.That (_comparer.Compare (x, y), Is.EqualTo (-1));
      Assert.That (_comparer.Compare (y, x), Is.EqualTo (1));
    }

    [Test]
    public void Compare_EqualPositionalKeys_EqualID ()
    {
      var x = new ExtensibleEnumInfo<Planet> (new Planet ("x"), _fakeMethod, _fakeResourceManager, 1.0);
      var y = new ExtensibleEnumInfo<Planet> (new Planet ("x"), _fakeMethod, _fakeResourceManager, 1.0);

      Assert.That (_comparer.Compare (x, y), Is.EqualTo (0));
      Assert.That (_comparer.Compare (y, x), Is.EqualTo (0));
    }
  }
}