using System;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public class NamespaceChangingNameProvider : INameProvider
  {
    public static readonly NamespaceChangingNameProvider Instance = new NamespaceChangingNameProvider ();

    private NamespaceChangingNameProvider ()
    {
    }

    public string GetNewTypeName (ClassDefinitionBase configuration)
    {
      string originalNamespace = configuration.Type.Namespace;
      int restStart = originalNamespace.Length > 0 ? originalNamespace.Length + 1 : 0;
      string originalRest = configuration.Type.FullName.Substring (restStart);

      string maskedRest = originalRest.Replace ("[[", "{");
      maskedRest = maskedRest.Replace ("]]", "}");
      maskedRest = maskedRest.Replace (", ", "/");
      maskedRest = maskedRest.Replace (",", "/");
      maskedRest = maskedRest.Replace ('.', '_');

      return string.Format ("{0}.MixedTypes.{1}", originalNamespace, maskedRest);
    }
  }
}