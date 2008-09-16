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
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public class SequenceStateHolder
  {
    public SequenceStateHolder ()
    {
      Counter = 0;
      Name = "";
      SequenceType = "";
      SequencePrefix = "";
      ElementPrefix = "";
      ElementPostfix = "";
      Separator = "";
      SequencePostfix = "";
    }

    public SequenceStateHolder (string name, string sequencePrefix, string elementPrefix, string elementPostfix,
      string separator, string sequencePostfix, bool sequenceStartWritten)
    {
      ArgumentUtility.CheckNotNull ("sequencePrefix", sequencePrefix);
      ArgumentUtility.CheckNotNull ("elementPrefix", elementPrefix);
      ArgumentUtility.CheckNotNull ("elementPostfix", elementPostfix);
      ArgumentUtility.CheckNotNull ("separator", separator);
      ArgumentUtility.CheckNotNull ("sequencePostfix", sequencePostfix);

      Counter = 0;
      Name = name;
      SequencePrefix = sequencePrefix;
      ElementPrefix = elementPrefix;
      ElementPostfix = elementPostfix;
      Separator = separator;
      SequencePostfix = sequencePostfix;
      SequenceStartWritten = sequenceStartWritten;
    }


    public string Name { get; set; }

    public string SequenceType
    {
      get; set;
    }

    public string SequencePrefix { get; set; }

    public string ElementPostfix { get; private set; }

    public string Separator { get; private set; }

    public string ElementPrefix { get; set; }

    public string SequencePostfix { get; private set; }

    public bool SequenceStartWritten { get; set; }


    /// <summary>
    /// The current position in the sequence.
    /// </summary>
    public int Counter { get; private set; }

    // TODO: Split into base class and two child classes, one for each ToTextBuilder (?)
    // (Might want to support SequencePrefix etc in XML in the future though, to
    // store in tag attribute to be ableb to (XSLT) transform to pure text output)
    public string ElementTag { get; set; }
    public string SequenceTag { get; set; }

    /// <summary>
    /// Move to the next position in the sequence.
    /// </summary>
    public void IncrementCounter ()
    {
      ++Counter;
    }
  }
}