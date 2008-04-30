namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class NonInstantiableClassWithAutomaticRelatedCollectionSetter : DomainObject
  {
    public static NonInstantiableClassWithAutomaticRelatedCollectionSetter NewObject ()
    {
      return NewObject<NonInstantiableClassWithAutomaticRelatedCollectionSetter> ().With ();
    }

    protected NonInstantiableClassWithAutomaticRelatedCollectionSetter()
    {
    }

    [DBBidirectionalRelation ("RelatedObjects")]
    public abstract NonInstantiableClassWithAutomaticRelatedCollectionSetter Parent { get; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<NonInstantiableClassWithAutomaticRelatedCollectionSetter> RelatedObjects { get; set; }
  }
}