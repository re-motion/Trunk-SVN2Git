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
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class AccessTypeDefinition_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AccessTypeDefinition>
  {
    public override void ToText (AccessTypeDefinition accessTypeDefinition, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("accessTypeDefinition", accessTypeDefinition);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      string shortName = accessTypeDefinition.DisplayName;

      if (toTextBuilder.OutputComplexity >= ToTextBuilderBase.ToTextBuilderOutputComplexityLevel.Complex)
      {
        toTextBuilder.ib<AccessTypeDefinition> ("").elements (shortName, accessTypeDefinition.Value).se ();
      }
      else
      {
        toTextBuilder.ib<AccessTypeDefinition> ("").e (shortName).se ();
      }
    }

   
  }
}