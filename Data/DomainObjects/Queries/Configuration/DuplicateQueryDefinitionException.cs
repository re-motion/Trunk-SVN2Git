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

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  public class DuplicateQueryDefinitionException : Exception
  {
    public readonly QueryDefinition QueryDefinition;
    public readonly QueryDefinition Duplicate;

    public DuplicateQueryDefinitionException (QueryDefinition queryDefinition, QueryDefinition duplicate)
        : base (GetMessage (queryDefinition))
    {
      QueryDefinition = queryDefinition;
      Duplicate = duplicate;
    }

    private static string GetMessage (QueryDefinition queryDefinition)
    {
      return string.Format ("The query with ID '{0}' has a duplicate.", queryDefinition.ID);
    }
  }
}
