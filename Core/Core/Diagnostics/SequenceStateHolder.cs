/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;

namespace Remotion.Diagnostics
{
  public class SequenceStateHolder
  {
    private int _sequenceCounter;
    private readonly string _sequencePrefix;
    private readonly string _firstElementPrefix;
    private readonly string _otherElementPrefix;
    private readonly string _elementPostfix;
    private readonly string _sequencePostfix;

    public SequenceStateHolder (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      ArgumentUtility.CheckNotNull ("sequencePrefix", sequencePrefix);
      ArgumentUtility.CheckNotNull ("firstElementPrefix", firstElementPrefix);
      ArgumentUtility.CheckNotNull ("otherElementPrefix", otherElementPrefix);
      ArgumentUtility.CheckNotNull ("elementPostfix", elementPostfix);
      ArgumentUtility.CheckNotNull ("sequencePostfix", sequencePostfix);

      _sequenceCounter = 0;
      _sequencePrefix = sequencePrefix;
      _firstElementPrefix = firstElementPrefix;
      _otherElementPrefix = otherElementPrefix;
      _elementPostfix = elementPostfix;
      _sequencePostfix = sequencePostfix;
    }

    public string SequencePrefix
    {
      get { return _sequencePrefix; }
    }

    public string ElementPostfix
    {
      get { return _elementPostfix; }
    }

    public string OtherElementPrefix
    {
      get { return _otherElementPrefix; }
    }

    public string FirstElementPrefix
    {
      get { return _firstElementPrefix; }
    }

    public string SequencePostfix
    {
      get { return _sequencePostfix; }
    }

    /// <summary>
    /// The current position in the sequence.
    /// </summary>
    public int Counter
    {
      get { return _sequenceCounter; }
    }

    /// <summary>
    /// Move to the next position in the sequence.
    /// </summary>
    public void IncrementCounter ()
    {
      ++_sequenceCounter;
    }
  }
}