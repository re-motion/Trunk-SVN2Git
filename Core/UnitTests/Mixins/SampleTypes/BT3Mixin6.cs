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
using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IBT3Mixin6ThisDependencies : IBaseType31, IBaseType32, IBaseType33, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6BaseDependencies : IBaseType34, IBT3Mixin4
  {
  }

  public interface IBT3Mixin6 { }

  [Extends(typeof(BaseType3))]
  [Serializable]
  public class BT3Mixin6<TThis, TBase> : Mixin<TThis, TBase>, IBT3Mixin6
      where TThis : class, IBT3Mixin6ThisDependencies
      where TBase : class, IBT3Mixin6BaseDependencies
  {
  }
}
