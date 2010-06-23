// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
function StyleUtility()
{
}

StyleUtility.CreateBorderSpans = function (selector)
{
}

StyleUtility.OnResize = function(element) {

  var topRight = element.find('.topRight');
  var bottomLeft = element.find('.bottomLeft');
  var bottomRight = element.find('.bottomRight');

  StyleUtility.ShowBorderSpans(element, topRight[0], bottomLeft[0], bottomRight[0]);
}

StyleUtility.AddBrowserSwitch = function()
{
    var browser;
    var version = parseInt($.browser.version); ;
    if ($.browser.msie)
    {
      (version < 8) ? browser = 'msie' + version : browser = 'msie';
    }
    if ($.browser.mozilla)
    {
      browser = 'mozilla';
    }
    if ($.browser.webkit)
    {
        browser = 'webkit';
    }
    if ($.browser.opera)
    {
        browser = 'opera';
    }

    StyleUtility.AddPlatformSwitch();

    if (!$('body').hasClass(browser))
      $('body').addClass(browser);
}

StyleUtility.AddPlatformSwitch = function ()
{
  var platform = "unknown";
  if (navigator.appVersion.indexOf("Win") != -1) platform = "win";
  if (navigator.appVersion.indexOf("Mac") != -1) platform = "mac";
  if (navigator.appVersion.indexOf("X11") != -1) platform = "nix";

  if (!$('body').hasClass(platform))
    $('body').addClass(platform);
}
