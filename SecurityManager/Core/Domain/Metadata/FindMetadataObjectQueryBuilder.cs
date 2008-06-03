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
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.SecurityManager.Domain.Metadata
{
  public class FindMetadataObjectQueryBuilder
  {
    private class MetadataID
    {
      public static MetadataID Parse (string metadataID)
      {
        try
        {
          if (metadataID.Contains ("|"))
          {
            string[] metadataIDParts = metadataID.Split (new char[] { '|' }, 2);
            Guid metadataItemID = new Guid (metadataIDParts[0]);
            int stateValue = int.Parse (metadataIDParts[1]);

            return new MetadataID (metadataItemID, stateValue);
          }

          return new MetadataID (new Guid (metadataID), null);
        }
        catch (FormatException exception)
        {
          throw new ArgumentException (string.Format ("The metadata ID '{0}' is invalid.", metadataID), "metadataID", exception);
        }
      }

      private Guid _metadataItemID;
      private int? _stateValue;

      public MetadataID (Guid metadataItemID, int? stateValue)
      {
        _metadataItemID = metadataItemID;
        _stateValue = stateValue;
      }

      public Guid MetadataItemID
      {
        get { return _metadataItemID; }
      }

      public int? StateValue
      {
        get { return _stateValue; }
      }
    }

    public Query CreateQuery (string metadataReference)
    {
      MetadataID metadataID = MetadataID.Parse (metadataReference);

      if (metadataID.StateValue.HasValue)
        return CreateFindStateDefinitionQuery (metadataID.MetadataItemID, metadataID.StateValue.Value);

      return CreateFindMetadataObjectQuery (metadataID.MetadataItemID);
    }

    private Query CreateFindStateDefinitionQuery (Guid metadataItemID, int stateValue)
    {
      Query query = CreateBaseQuery ("Remotion.SecurityManager.Domain.Metadata.MetadataObject.Find.StateDefinition", metadataItemID);
      query.Parameters.Add ("@stateValue", stateValue);

      return query;
    }

    private Query CreateFindMetadataObjectQuery (Guid metadataItemID)
    {
      return CreateBaseQuery ("Remotion.SecurityManager.Domain.Metadata.MetadataObject.Find", metadataItemID);
    }

    private Query CreateBaseQuery (string queryID, Guid metadataItemID)
    {
      Query query = new Query (queryID);
      query.Parameters.Add ("@metadataItemID", metadataItemID);

      return query;
    }
  }
}
