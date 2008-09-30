using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Container of <see cref="AclExpansionEntry"/>|s returned by <see cref="AclExpander"/>.
  /// </summary>
  public class AclExpansion : IEnumerable<AclExpansionEntry>
  {
    private readonly AclExpansionEntry[] _aclExpansionEntries;

    public AclExpansion (AclExpansionEntry[] aclExpansionEntries)
    {
      ArgumentUtility.CheckNotNull ("aclExpansionEntries", aclExpansionEntries);
      _aclExpansionEntries = aclExpansionEntries;
    }

    public IEnumerator<AclExpansionEntry> GetEnumerator ()
    {
      return ((IEnumerable<AclExpansionEntry>) _aclExpansionEntries).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}