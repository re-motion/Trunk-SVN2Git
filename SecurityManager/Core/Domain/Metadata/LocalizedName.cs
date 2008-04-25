using System;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class LocalizedName : BaseSecurityManagerObject
  {
    public static LocalizedName NewObject (string text, Culture culture, MetadataObject metadataObject)
    {
      return NewObject<LocalizedName> ().With (text, culture, metadataObject);
    }

    protected LocalizedName (string text, Culture culture, MetadataObject metadataObject)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("culture", culture);
      ArgumentUtility.CheckNotNull ("metadataObject", metadataObject);

      Text = text;
      Culture = culture;
      MetadataObject = metadataObject;
    }

    [StringProperty (IsNullable = false)]
    public abstract string Text { get; set; }

    [Mandatory]
    public abstract Culture Culture { get; protected set; }

    [DBBidirectionalRelation ("LocalizedNames")]
    [Mandatory]
    public abstract MetadataObject MetadataObject { get; protected set; }
  }
}
