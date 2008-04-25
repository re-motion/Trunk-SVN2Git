using System;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class LocalizedName
  {
    private string _referencedObjectID;
    private string _comment;
    private string _text;

    public LocalizedName (string referencedObjectID, string comment, string text)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("referencedObjectID", referencedObjectID);
      ArgumentUtility.CheckNotNullOrEmpty ("comment", comment);
      ArgumentUtility.CheckNotNull ("text", text);

      _referencedObjectID = referencedObjectID;
      _comment = comment;
      _text = text;
    }

    public string Text
    {
      get { return _text; }
    }

    public string Comment
    {
      get { return _comment; }
    }

    public string ReferencedObjectID
    {
      get { return _referencedObjectID; }
    }

    public override bool Equals (object obj)
    {
      LocalizedName otherName = obj as LocalizedName;
      if (otherName == null)
        return false;

      return otherName._comment.Equals (_comment) && otherName._referencedObjectID.Equals (_referencedObjectID) && otherName._text.Equals (_text);
    }

    public override int GetHashCode ()
    {
      return _comment.GetHashCode () ^ _referencedObjectID.GetHashCode () ^ _text.GetHashCode ();
    }
  }
}
