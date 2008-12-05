// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain
{
  public class DerivedClassWithTwoBaseReferencesViaMixins : TargetClassReceivingTwoReferencesToDerivedClass
  {
    public new static DerivedClassWithTwoBaseReferencesViaMixins NewObject ()
    {
      return NewObject<DerivedClassWithTwoBaseReferencesViaMixins> ().With ();
    }

    [DBBidirectionalRelation ("MyDerived1")]
    public virtual TargetClassReceivingTwoReferencesToDerivedClass MyBase1 { get; set; }
    [DBBidirectionalRelation ("MyDerived2")]
    public virtual TargetClassReceivingTwoReferencesToDerivedClass MyBase2 { get; set; }
  }
}
