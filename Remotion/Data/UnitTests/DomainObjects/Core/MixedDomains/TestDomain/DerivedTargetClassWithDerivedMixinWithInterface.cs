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
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  /// <summary>
  /// This class inherits a mixin from its base class that introduces an interface, and it itself has a mixin derived from that base class mixin.
  /// The mixin implicitly introduces the same interface. This can create name clashes.
  /// </summary>
  [Uses (typeof (DerivedMixinAddingPersistentProperties))]
  public class DerivedTargetClassWithDerivedMixinWithInterface : TargetClassForPersistentMixin
  {
  }
}