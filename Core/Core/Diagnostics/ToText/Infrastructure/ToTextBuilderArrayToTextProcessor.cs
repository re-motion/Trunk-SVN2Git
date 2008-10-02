// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
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