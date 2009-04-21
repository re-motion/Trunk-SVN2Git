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
using System.Collections.Generic;
using Remotion.Data.Linq.DataObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.Linq.SqlGeneration
{
  public class LetData
  {
    public LetColumnSource CorrespondingColumnSource;

    public LetData (IEvaluation evaluation, string name,LetColumnSource columnSource)
    {
      ArgumentUtility.CheckNotNull ("evaluation", evaluation);
      ArgumentUtility.CheckNotNull ("name", name);
      ArgumentUtility.CheckNotNull ("columnSource", columnSource);
      
      Evaluation = evaluation;
      Name = name;
      CorrespondingColumnSource = columnSource;
    }

    public IEvaluation Evaluation { get; private set; }
    public string Name { get; private set; }
  }
}
