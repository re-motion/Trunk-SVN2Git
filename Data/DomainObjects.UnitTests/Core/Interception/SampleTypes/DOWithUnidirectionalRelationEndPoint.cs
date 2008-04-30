using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class DOWithUnidirectionalRelationEndPoint : DomainObject
  {
    public abstract DOWithVirtualProperties RelatedObject { get; set; }
  }
}