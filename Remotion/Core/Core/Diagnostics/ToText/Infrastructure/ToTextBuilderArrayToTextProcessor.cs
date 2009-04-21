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

namespace Remotion.Diagnostics.ToText.Infrastructure
{
  internal class ToTextBuilderArrayToTextProcessor : OuterProductProcessorBase
  {
    protected readonly Array _array;
    private readonly IToTextBuilder _toTextBuilder;

    public ToTextBuilderArrayToTextProcessor (Array rectangularArray, IToTextBuilder toTextBuilder)
    {
      _array = rectangularArray;
      _toTextBuilder = toTextBuilder;
    }

    public override bool DoBeforeLoop ()
    {
      if (ProcessingState.IsInnermostLoop)
      {
        //_toTextBuilder.WriteElement (_array.GetValue (ProcessingState.DimensionIndices));
        _toTextBuilder.WriteElement (_array.GetValue (ProcessingState.DimensionIndices));
      }
      else
      {
        //_toTextBuilder.WriteSequenceLiteralBegin ("", _toTextBuilder.Settings.ArrayPrefix, _toTextBuilder.Settings.ArrayElementPrefix,
        //  _toTextBuilder.Settings.ArrayElementPostfix, _toTextBuilder.Settings.ArraySeparator, _toTextBuilder.Settings.ArrayPostfix);

        //_toTextBuilder.WriteSequenceLiteralBegin ("", "A2 ", "AE2 ", "~AE2 ", "_AE2 ", "_A2");  // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        _toTextBuilder.WriteSequenceArrayBegin ();
      }
      return true;
    }



    public override bool DoAfterLoop ()
    {
      if (!ProcessingState.IsInnermostLoop)
      {
        _toTextBuilder.WriteSequenceEnd ();
      }
      return true;
    }



  }
}