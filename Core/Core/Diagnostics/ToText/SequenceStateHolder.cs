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

namespace Remotion.Diagnostics.ToText
{
  public class SequenceStateHolder
  {
    private int _sequenceCounter;
    private readonly string _sequencePrefix;
    private readonly string _elementPrefix;
    private readonly string _elementPostfix;
    private readonly string _separator;
    private readonly string _sequencePostfix;

    public SequenceStateHolder ()
    {
      _sequenceCounter = 0;
      Name = "";
      SequenceType = "";
      _sequencePrefix = "";
      _elementPrefix = "";
      _elementPostfix = "";
      _separator = "";
      _sequencePostfix = "";
    }

    public SequenceStateHolder (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      ArgumentUtility.CheckNotNull ("sequencePrefix", sequencePrefix);
      ArgumentUtility.CheckNotNull ("elementPrefix", elementPrefix);
      ArgumentUtility.CheckNotNull ("elementPostfix", elementPostfix);
      ArgumentUtility.CheckNotNull ("separator", separator);
      ArgumentUtility.CheckNotNull ("sequencePostfix", sequencePostfix);

      _sequenceCounter = 0;
      Name = name;
      _sequencePrefix = sequencePrefix;
      _elementPrefix = elementPrefix;
      _elementPostfix = elementPostfix;
      _separator = separator;
      _sequencePostfix = sequencePostfix;
    }


    public string Name { get; set; }

    public string SequenceType
    {
      get; set;
    }

    public string SequencePrefix
    {
      get { return _sequencePrefix; }
    }

    public string ElementPostfix
    {
      get { return _elementPostfix; }
    }

    public string Separator
    {
      get { return _separator; }
    }

    public string ElementPrefix
    {
      get { return _elementPrefix; }
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