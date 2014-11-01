using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  public static class CoypuElementScopeExtensions
  {
    /// <summary>
    /// This method reduces performance a little bit (<see cref="WebTestObjectContext"/>s which are never actually accessed by the web test are also
    /// resolved). However, it ensures that any <see cref="MissingHtmlException"/> is thrown when the <see cref="WebTestObjectContext"/> is created,
    /// which is always near the corresponding <c>parentScope.Find*()</c> method call. Otherwise, the <see cref="MissingHtmlException"/> would be
    /// thrown when the context's <see cref="Scope"/> is actually used for the first time, which may be quite some time later and the exception would
    /// provide a stack trace where the <c>parentScope.Find*()</c> call could not be found.
    /// </summary>
    /// <param name="scope">The <see cref="ElementScope"/> which is asserted to exist.</param>
    public static void EnsureExistence ([NotNull] this ElementScope scope)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);

      scope.Now();
    }

    /// <summary>
    /// Returns a child element of a control, specified by a <paramref name="idSuffix"/>.
    /// </summary>
    public static ElementScope FindChild ([NotNull] this ElementScope scope, [NotNull] string idSuffix)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNullOrEmpty ("idSuffix", idSuffix);

      var fullId = string.Format ("{0}_{1}", scope.Id, idSuffix);
      return scope.FindId (fullId);
    }
  }
}