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
using Remotion.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;
using Remotion.Data.Linq.Parsing.FieldResolving;
using Remotion.Mixins.Definitions.Building;

namespace Core.Documentation.TheDesignOfTheRemotionFramework
{
  /// <summary>
  /// This class tells you about the design of the remotion framework.
  /// </summary>
  internal partial class AaaInfoAboutTheDesignOfTheRemotionFramework
  {
    /// <summary>
    /// Builder classes (e.g. <see cref="FileBuilder"/>, <see cref="AttributeDefinitionBuilder"/>) have the
    /// following characteristics:
    /// <list type="number">
    /// <item>
    /// Builder classes are called "SomethingBuilder" (e.g. <see cref="FieldSourcePathBuilder"/>).
    /// </item>
    /// <item>etc</item>
    /// </list>
    /// </summary>    
    private class Builder { }
  }
}