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
namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class ProxiedChildChildGeneric<T0, T1> : ProxiedChildGeneric<T0, T1>
  {
    public new string ProxiedChildGenericToString<T2, T3> (T0 t0, T1 t1, T2 t2, T3 t3)
    {
      return "ProxiedChildChildGeneric: " + t0 + t1 + t2 + t3;
    }
  }
}