<?xml version="1.0" encoding="UTF-8" ?>
<!-- This file is part of the re-motion Core Framework (www.re-motion.org)
 ! Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
 ! 
 ! The re-motion Core Framework is free software; you can redistribute it 
 ! and/or modify it under the terms of the GNU Lesser General Public License 
 ! version 3.0 as published by the Free Software Foundation.
 ! 
 ! re-motion is distributed in the hope that it will be useful, 
 ! but WITHOUT ANY WARRANTY; without even the implied warranty of 
 ! MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
 ! GNU Lesser General Public License for more details.
 ! 
 ! You should have received a copy of the GNU Lesser General Public License
 ! along with re-motion; if not, see http://www.gnu.org/licenses.
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/doc">
    <doc>
      <xsl:copy-of select="assembly" />
      
      <xsl:for-each select="members">
        <members>
          <xsl:for-each select="member">
            <member name="{@name}">
              <xsl:copy-of select="summary" />
              <xsl:copy-of select="param" />
            </member>
          </xsl:for-each>
        </members>
      </xsl:for-each>
    </doc>
  </xsl:template>
    
</xsl:stylesheet>