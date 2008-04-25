using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public abstract class SerializableMappingObject : IObjectReference
  {
    public abstract object GetRealObject (StreamingContext context);
    protected abstract bool IsPartOfMapping { get; }
    protected abstract string IDForExceptions { get; }

    [OnSerializing]
    private void CheckWhenSerializing (StreamingContext context)
    {
      if (!IsPartOfMapping)
      {
        string message = string.Format ("The {0} '{1}' cannot be serialized because is is not part of the current mapping.",
            GetType ().Name, IDForExceptions);
        throw new SerializationException (message);
      }
    }
  }
}
