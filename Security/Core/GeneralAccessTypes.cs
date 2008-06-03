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

namespace Remotion.Security
{
  [AccessType]
  public enum GeneralAccessTypes
  {
    [PermanentGuid ("1D6D25BC-4E85-43ab-A42D-FB5A829C30D5")]
    Create = 0,
    [PermanentGuid ("62DFCD92-A480-4d57-95F1-28C0F5996B3A")]
    Read = 1,
    [PermanentGuid ("11186122-6DE0-4194-B434-9979230C41FD")]
    Edit = 2,
    [PermanentGuid ("305FBB40-75C8-423a-84B2-B26EA9E7CAE7")]
    Delete = 3,
    [PermanentGuid ("67CEA479-0BE7-4e2f-B2E0-BB1FCC9EA1D6")]
    Search = 4,
    [PermanentGuid ("203B7478-96F1-4bf1-B4EA-5BDD1206252C")]
    Find = 5
  }
}
