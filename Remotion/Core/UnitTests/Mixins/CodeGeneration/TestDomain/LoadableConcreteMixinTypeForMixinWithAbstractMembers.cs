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
using Remotion.Mixins.CodeGeneration;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration.TestDomain
{
  [ConcreteMixinType (
      new object[] {
          typeof (MixinWithAbstractMembers),
          new object[] { 
              new object[] { typeof (ClassOverridingMixinMembers), "AbstractMethod", "System.String AbstractMethod(Int32)" },
              new object[] { typeof (ClassOverridingMixinMembers), "RaiseEvent", "System.String RaiseEvent()" },
              new object[] { typeof (ClassOverridingMixinMembers), "get_AbstractProperty", "System.String get_AbstractProperty()" },
              new object[] { typeof (ClassOverridingMixinMembers), "add_AbstractEvent", "Void add_AbstractEvent(System.Func`1[System.String])" },
              new object[] { typeof (ClassOverridingMixinMembers), "remove_AbstractEvent", "Void remove_AbstractEvent(System.Func`1[System.String])" },
          },
          new object[0]
      })]
  public class LoadableConcreteMixinTypeForMixinWithAbstractMembers
  { }
}
