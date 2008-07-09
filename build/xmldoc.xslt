<?xml version="1.0" encoding="utf-8"?>
<!-- Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 !
 ! This program is free software: you can redistribute it and/or modify it under 
 ! the terms of the re:motion license agreement in license.txt. If you did not 
 ! receive it, please visit http://www.re-motion.org/licensing.
 ! 
 ! Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 ! WITHOUT WARRANTY OF ANY KIND, either express or implied. 
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