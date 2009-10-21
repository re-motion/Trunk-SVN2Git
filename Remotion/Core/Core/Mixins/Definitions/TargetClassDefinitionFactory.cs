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
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Validation;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  /// <summary>
  /// Creates <see cref="TargetClassDefinition"/> objects from <see cref="ClassContext"/> instances, validating them before returning.
  /// </summary>
  /// <remarks>
  /// This class acts as a facade for <see cref="TargetClassDefinitionBuilder"/> and <see cref="Validator"/>.
  /// </remarks>
  public static class TargetClassDefinitionFactory
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (LogManager));

    // This doesn't hold any state and can thus safely be used from multiple threads at the same time
    private static readonly TargetClassDefinitionBuilder s_definitionBuilder = new TargetClassDefinitionBuilder();

    public static TargetClassDefinition CreateTargetClassDefinition (ClassContext context)
    {
      s_log.DebugFormat ("Creating a class definition for: {0}.", context);

      using (StopwatchScope.CreateScope (s_log, LogLevel.Debug, "Time needed to create and validate class definition: {elapsed}."))
      {
        TargetClassDefinition definition = s_definitionBuilder.Build (context);
        Validate (definition);
        return definition;
      }
    }

    private static void Validate (TargetClassDefinition definition)
    {
      DefaultValidationLog log = Validator.Validate (definition);
      if (log.GetNumberOfFailures () > 0 || log.GetNumberOfUnexpectedExceptions () > 0)
        throw new ValidationException (log);
    }
  }
}
