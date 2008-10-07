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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  public class FindMetadataObjectQueryBuilder
  {
    private struct MetadataID
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

      public readonly Guid MetadataItemID;
      public readonly int? StateValue;

      public MetadataID (Guid metadataItemID, int? stateValue)
      {
        MetadataItemID = metadataItemID;
        StateValue = stateValue;
      }
    }

    public IQueryable<MetadataObject> CreateQuery (string metadataReference)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("metadataReference", metadataReference);

      MetadataID metadataID = MetadataID.Parse (metadataReference);

      if (metadataID.StateValue.HasValue)
      {
        return (from state in QueryFactory.CreateQueryable<StateDefinition>()
               where state.StateProperty.MetadataItemID == metadataID.MetadataItemID && state.Value == metadataID.StateValue
               select state).Cast<MetadataObject>();
      }
      else
      {
        return from m in QueryFactory.CreateQueryable<MetadataObject>()
               where m.MetadataItemID == metadataID.MetadataItemID
               select m;
      }
    }
  }
}