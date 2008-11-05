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
using System.Collections.Generic;
using System.Text;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.CodeGenerator
{
  public class XmlFragmentContext
  {
    public readonly StringBuilder XmlFragment;
    /// <summary> line number of the first XML segment line </summary>
    public readonly int FirstLineNumber;
    /// <summary>  beginning line position for each line </summary>
    public readonly List<int> Indents;

    public XmlFragmentContext (StringBuilder xmlFragment, int firstLineNumber, List<int> indents)
    {
      ArgumentUtility.CheckNotNull ("xmlFragment", xmlFragment);
      ArgumentUtility.CheckNotNull ("indents", indents);

      XmlFragment = xmlFragment;
      FirstLineNumber = firstLineNumber;
      Indents = indents;
    }
  }
}