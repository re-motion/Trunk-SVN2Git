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
