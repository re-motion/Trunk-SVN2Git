/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;

namespace Remotion.Diagnostics.ToText
{
  internal class ToTextBuilderArrayToTextProcessor : OuterProductProcessorBase
  {
    protected readonly Array _array;
    private readonly ToTextBuilder _toTextBuilder;

    public ToTextBuilderArrayToTextProcessor (Array rectangularArray, ToTextBuilder toTextBuilder)
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
        _toTextBuilder.AppendSequenceBegin ("", _toTextBuilder.Settings.ArrayPrefix, _toTextBuilder.Settings.ArrayFirstElementPrefix,
          _toTextBuilder.Settings.ArrayOtherElementPrefix, _toTextBuilder.Settings.ArrayElementPostfix, _toTextBuilder.Settings.ArrayPostfix);

        //_toTextBuilder.AppendSequenceBegin ("", "A2 ", "AE2 ", "~AE2 ", "_AE2 ", "_A2");  // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

      }
      return true;
    }



    public override bool DoAfterLoop ()
    {
      if (!ProcessingState.IsInnermostLoop)
      {
        _toTextBuilder.AppendSequenceEnd ();
      }
      return true;
    }



  }
}