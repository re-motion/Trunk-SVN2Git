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

// Original license header
/*
* Autocomplete - jQuery plugin 1.1pre
*
* Copyright (c) 2007 Dylan Verheul, Dan G. Switzer, Anjesh Tuladhar, JÃ¶rn Zaefferer
*
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* Revision: $Id: jquery.autocomplete.js 5785 2008-07-12 10:37:33Z joern.zaefferer $
*
*/

// ************************************************
// Changes have been commented with "// re-motion:"
// ************************************************

; (function($) {

    $.fn.extend({
        autocomplete: function(serviceUrl, serviceMethod, options) {
            var $input = $(this);
            options = $.extend({}, $.Autocompleter.defaults, {
                // re-motion: instead of a single URL property, use separate service URL and service method properties. 
                //           data cannot be inserted directly any more
                serviceUrl: serviceUrl,
                serviceMethod: serviceMethod,
                data: null,
                displayListDelay: $.Autocompleter.defaults.displayListDelay,
                autoFillDelay: $.Autocompleter.defaults.autoFillDelay,
                max: options && !options.scroll ? 10 : 150,
                // re-motion: clicking this control will display the dropdown list with an assumed input of '' (regardless of textbox value)
                dropDownButtonId: null,
                // re-motion: select first value in list unless textbox is empty
                selectFirst: function() { return $input.val() != ''; }
            }, options);

            // if highlight is set to false, replace it with a do-nothing function
            options.highlight = options.highlight || function(value) { return value; };

            // if the formatMatch option is not specified, then use formatItem for backwards compatibility
            options.formatMatch = options.formatMatch || options.formatItem;

            return this.each(function() {
                new $.Autocompleter(this, options);
            });
        },
        result: function(handler) {
            return this.bind("result", handler);
        },
        search: function(handler) {
            return this.trigger("search", [handler]);
        },
        flushCache: function() {
            return this.trigger("flushCache");
        },
        setOptions: function(options) {
            return this.trigger("setOptions", [options]);
        },
        unautocomplete: function() {
            return this.trigger("unautocomplete");
        }
    });

    $.Autocompleter = function(input, options) {

        var KEY = {
            UP: 38,
            DOWN: 40,
            DEL: 46,
            TAB: 9,
            RETURN: 13,
            ESC: 27,
            COMMA: 188,
            PAGEUP: 33,
            PAGEDOWN: 34,
            BACKSPACE: 8
        };

        // Create $ object for input element
        var $input = $(input).attr("autocomplete", "off").addClass(options.inputClass);

        var timeout;
        var autoFillTimeout;
        var previousValue = "";
        var cache = $.Autocompleter.Cache(options);
        var hasFocus = 0;
        var lastKeyPressCode;
        var config = {
            mouseDownOnSelect: false
        };
        var select = $.Autocompleter.Select(options, input, selectCurrent, config);

        var blockSubmit;

        // prevent form submit in opera when selecting with return key
        $.browser.opera && $(input.form).bind("submit.autocomplete", function() {
            if (blockSubmit) {
                blockSubmit = false;
                return false;
            }
        });



        // only opera doesn't trigger keydown multiple times while pressed, others don't work with keypress at all
        $input.bind(($.browser.opera ? "keypress" : "keydown") + ".autocomplete", function(event) {
            // track last key pressed
            lastKeyPressCode = event.keyCode;
            switch (event.keyCode) {
                case KEY.UP:
                    event.preventDefault();
                    if (select.visible()) {
                        select.prev();
                    } else {
                        onChange(0, true);
                    }
                    break;

                case KEY.DOWN:
                    event.preventDefault();
                    if (select.visible()) {
                        select.next();
                    } else {
                        onChange(0, true);
                    }
                    break;

                case KEY.PAGEUP:
                    event.preventDefault();
                    if (select.visible()) {
                        select.pageUp();
                    } else {
                        onChange(0, true);
                    }
                    break;

                case KEY.PAGEDOWN:
                    event.preventDefault();
                    if (select.visible()) {
                        select.pageDown();
                    } else {
                        onChange(0, true);
                    }
                    break;

                // matches also semicolon                    
                case options.multiple && $.trim(options.multipleSeparator) == "," && KEY.COMMA:
                case KEY.RETURN:
                    if (selectCurrent()) {
                        // stop default to prevent a form submit, Opera needs special handling
                        event.preventDefault();
                        blockSubmit = true;
                        return false;
                    }
                    // re-motion: allow deletion of current value by entering the empty string
                    else if ($input.val() == '') {
                        hideResults();
                        $input.trigger("result", { DisplayName: '', UniqueIdentifier: options.nullValue });
                        event.preventDefault();
                        blockSubmit = true;
                        return false;
                    }
                    break;
                // re-motion: do not block event bubbling for tab                  
                case KEY.TAB:
                    if (selectCurrent()) {
                    }
                    else if ($input.val() == '') {
                        hideResults();
                        $input.trigger("result", { DisplayName: '', UniqueIdentifier: options.nullValue });
                    }
                    break;
                case KEY.ESC:
                    select.hide();
                    break;

                default:
                    clearTimeout(timeout);
                    timeout = setTimeout(onChange, options.displayListDelay);

                    // re-motion: start the auto-fill enabler count-down
                    enableAutoFill();

                    break;
            }
        }).focus(function() {
            // track whether the field has focus, we shouldn't process any
            // results if the field no longer has focus
            hasFocus++;
        }).blur(function() {
            hasFocus = 0;
            if (!config.mouseDownOnSelect) {
                hideResults();
            }
        }).click(function() {
            // show select when clicking in a focused field
            if (hasFocus++ > 1 && !select.visible()) {
                onChange(1, true);
                //adjustSelection( $input.val() );

            }
        }).bind("search", function() {
            // TODO why not just specifying both arguments?
            var fn = (arguments.length > 1) ? arguments[1] : null;
            function findValueCallback(q, data) {
                var result;
                if (data && data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].result.toLowerCase() == q.toLowerCase()) {
                            result = data[i];
                            break;
                        }
                    }
                }
                if (typeof fn == "function") fn(result);
                else $input.trigger("result", result && [result.data, result.value]);
            }
            $.each(trimWords($input.val()), function(i, value) {
                request(value, findValueCallback, findValueCallback);
            });
        }).bind("flushCache", function() {
            cache.flush();
        }).bind("setOptions", function() {
            $.extend(options, arguments[1]);
            // if we've updated the data, repopulate
            if ("data" in arguments[1])
                cache.populate();
        }).bind("unautocomplete", function() {
            select.unbind();
            $input.unbind();
            $(input.form).unbind(".autocomplete");
        });

        // re-motion: bind onChange to the dropDownButton's click event
        var dropdownButton = $('#' + options.dropDownButtonId);
        if (dropdownButton.length > 0) {
            dropdownButton.click(function() {
                $input.focus();
                onChange(1, true);
                //adjustSelection( $input.val() );
                clearTimeout(timeout);
            });
        }

        // re-motion: when clicking the dropDownButton, highlight the currently selected value in the list
        function adjustSelection(selectedValue) {
            if (selectedValue != "") {
                var currentIndex = 0;
                var stopIndex = options.max;
                var finished = false;
                do {
                    var current = select.selected().result;
                    if (selectedValue == current)
                        finished = true;
                    else
                        select.next();

                    currentIndex++;
                } while (!finished && currentIndex < stopIndex)
            }
        }

        function selectCurrent() {
            var selected = select.selected();
            if (!selected)
                return false;

            var v = selected.result;

            previousValue = v;

            if (options.multiple) {
                var words = trimWords($input.val());
                if (words.length > 1) {
                    v = words.slice(0, words.length - 1).join(options.multipleSeparator) + options.multipleSeparator + v;
                }
                v += options.multipleSeparator;
            }

            $input.val(v);
            hideResultsNow();
            $input.trigger("result", [selected.data, selected.value]);

            // re-motion: set autoFill to false again and reset the timer
            options.autoFill = false;
            autoFillTimeout = null;

            return true;
        }

        // re-motion: auto-fill is always enabled, but only after autoFillDelay has passed while typing
        function enableAutoFill() {
            if (!autoFillTimeout)
                autoFillTimeout = setTimeout(function() { options.autoFill = true; }, options.autoFillDelay);
        }

        // re-motion: use obsolete first parameter to indicate whether the onChange event is triggered by input (0) or the dropdownButton (1)
        function onChange(isDropDown, skipPrevCheck) {
            if (lastKeyPressCode == KEY.DEL) {
                select.hide();
                return;
            }

            var currentValue = $input.val();
            if (!skipPrevCheck && currentValue == previousValue)
                return;

            previousValue = currentValue;

            currentValue = lastWord(currentValue);
            if (currentValue.length >= options.minChars) {
                $input.addClass(options.loadingClass);
                if (!options.matchCase)
                    currentValue = currentValue.toLowerCase();

                // re-motion: if triggered by dropDownButton, get the full list
                if (isDropDown == 1) {
                    request('', receiveData, hideResultsNow);
                } else {
                    request(currentValue, receiveData, hideResultsNow);
                }

            } else {
                stopLoading();
                select.hide();
            }
        };

        function trimWords(value) {
            if (!value) {
                return [""];
            }
            var words = value.split(options.multipleSeparator);
            var result = [];
            $.each(words, function(i, value) {
                if ($.trim(value))
                    result[i] = $.trim(value);
            });
            return result;
        }

        function lastWord(value) {
            if (!options.multiple)
                return value;
            var words = trimWords(value);
            return words[words.length - 1];
        }

        // fills in the input box w/the first match (assumed to be the best match)
        // q: the term entered
        // sValue: the first matching result
        function autoFill(q, sValue) {
            // autofill in the complete box w/the first match as long as the user hasn't entered in more data
            // if the last user key pressed was backspace, don't autofill
            if (options.autoFill && (lastWord($input.val()).toLowerCase() == q.toLowerCase()) && lastKeyPressCode != KEY.BACKSPACE) {
                // fill in the value (keep the case the user has typed)
                $input.val($input.val() + sValue.substring(lastWord(previousValue).length));
                // select the portion of the value not typed by the user (so the next character will erase)
                $.Autocompleter.Selection(input, previousValue.length, previousValue.length + sValue.length);
            }
        };

        function hideResults() {
            clearTimeout(timeout);
            timeout = setTimeout(hideResultsNow, 200);
        };

        function hideResultsNow() {
            var wasVisible = select.visible();
            select.hide();
            clearTimeout(timeout);
            stopLoading();
            if (options.mustMatch) {
                // call search and run callback
                $input.search(
				function(result) {
				    // if no value found, clear the input box
				    if (!result) {
				        if (options.multiple) {
				            var words = trimWords($input.val()).slice(0, -1);
				            $input.val(words.join(options.multipleSeparator) + (words.length ? options.multipleSeparator : ""));
				        }
				        else
				            $input.val("");
				    }
				}
			);
            }
            if (wasVisible)
            // position cursor at end of input field
                $.Autocompleter.Selection(input, input.value.length, input.value.length);
        };

        function receiveData(q, data) {
            if (data && data.length && hasFocus) {
                stopLoading();
                var doAutoFill = select.visible();
                select.display(data, q);
                // re-motion: 
                // 1) only auto-fill if dropdown list is already open
                // 2) pass display string instead of value
                if (doAutoFill)
                    autoFill(q, data[0].result);
                select.show();
            } else {
                hideResultsNow();
            }
        };

        function request(term, success, failure) {
            if (!options.matchCase)
                term = term.toLowerCase();
            var data = cache.load(term);
            // recieve the cached data
            if (data && data.length) {
                success(term, data);

                // re-motion: if a webservice url and a method name have been supplied, try loading the data now
            } else if ((typeof options.serviceUrl == "string") && (options.serviceUrl.length > 0)
                        && (typeof options.serviceMethod == "string") && (options.serviceMethod.length > 0)) {

                // re-motion: replaced jQuery AJAX call with .NET call because of the following problem:
                //           when extending the parameter list with the necessary arguments for the web service method call,
                //           the JSON object is serialized to "key=value;" format, but the service expects JSON format ("{ key: value, ... }")
                //           see http://encosia.com/2008/06/05/3-mistakes-to-avoid-when-using-jquery-with-aspnet-ajax/ 
                //           under "JSON, objects, and strings: oh my!" for details.
                var params = {
                    prefixText: (options.ignoreInput ? '' : lastWord(term)),
                    completionSetCount: options.max,
                    businessObjectClass: options.extraParams['businessObjectClass'],
                    businessObjectProperty: options.extraParams['businessObjectProperty'],
                    businessObjectID: options.extraParams['businessObjectID'],
                    args: options.extraParams['args']
                };
                Sys.Net.WebServiceProxy.invoke(options.serviceUrl, options.serviceMethod, false, params,
                                          function(result, context, methodName) {
                                              var parsed = options.parse && options.parse(result) || parse(result);
                                              cache.add(term, parsed);
                                              success(term, parsed);
                                          },
                                          function(err, context, methodName) { });
            } else {
                // if we have a failure, we need to empty the list -- this prevents the the [TAB] key from selecting the last successful match
                select.emptyList();
                failure(term);
            }
        };

        function parse(data) {
            var parsed = [];
            var rows = data.split("\n");
            for (var i = 0; i < rows.length; i++) {
                var row = $.trim(rows[i]);
                if (row) {
                    row = row.split("|");
                    parsed[parsed.length] = {
                        data: row,
                        value: row[0],
                        result: options.formatResult && options.formatResult(row, row[0]) || row[0]
                    };
                }
            }
            return parsed;
        };

        function stopLoading() {
            $input.removeClass(options.loadingClass);
        };

    };

    $.Autocompleter.defaults = {
        inputClass: "ac_input",
        resultsClass: "ac_results",
        loadingClass: "ac_loading",
        minChars: 1,
        displayListDelay: 400,
        autoFillDelay: 400,
        matchCase: false,
        matchSubset: true,
        matchContains: false,
        cacheLength: 10,
        max: 100,
        mustMatch: false,
        extraParams: {},
        // re-motion: changed selectFirst from boolean field to function
        selectFirst: function() { return true; },
        formatItem: function(row) { return row[0]; },
        formatMatch: null,
        autoFill: false,
        width: 0,
        multiple: false,
        multipleSeparator: ", ",
        highlight: function(value, term) {
            return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>");
        },
        scroll: true,
        scrollHeight: 180,
        repositionInterval: 200
    };

    $.Autocompleter.Cache = function(options) {

        var data = {};
        var length = 0;

        function matchSubset(s, sub) {
            if (!options.matchCase)
                s = s.toLowerCase();
            var i = s.indexOf(sub);
            if (options.matchContains == "word") {
                i = s.toLowerCase().search("\\b" + sub.toLowerCase());
            }
            if (i == -1) return false;
            return i == 0 || options.matchContains;
        };

        function add(q, value) {
            if (length > options.cacheLength) {
                flush();
            }
            if (!data[q]) {
                length++;
            }
            data[q] = value;
        }

        function populate() {
            if (!options.data) return false;
            // track the matches
            var stMatchSets = {},
			nullData = 0;

            // no url was specified, we need to adjust the cache length to make sure it fits the local data store
            if (!options.serviceUrl) options.cacheLength = 1;

            // track all options for minChars = 0
            stMatchSets[""] = [];

            // loop through the array and create a lookup structure
            for (var i = 0, ol = options.data.length; i < ol; i++) {
                var rawValue = options.data[i];
                // if rawValue is a string, make an array otherwise just reference the array
                rawValue = (typeof rawValue == "string") ? [rawValue] : rawValue;

                var value = options.formatMatch(rawValue, i + 1, options.data.length);
                if (value === false)
                    continue;

                var firstChar = value.charAt(0).toLowerCase();
                // if no lookup array for this character exists, look it up now
                if (!stMatchSets[firstChar])
                    stMatchSets[firstChar] = [];

                // if the match is a string
                var row = {
                    value: value,
                    data: rawValue,
                    result: options.formatResult && options.formatResult(rawValue) || value
                };

                // push the current match into the set list
                stMatchSets[firstChar].push(row);

                // keep track of minChars zero items
                if (nullData++ < options.max) {
                    stMatchSets[""].push(row);
                }
            };

            // add the data items to the cache
            $.each(stMatchSets, function(i, value) {
                // increase the cache size
                options.cacheLength++;
                // add to the cache
                add(i, value);
            });
        }

        // populate any existing data
        setTimeout(populate, 25);

        function flush() {
            data = {};
            length = 0;
        }

        return {
            flush: flush,
            add: add,
            populate: populate,
            load: function(q) {
                if (!options.cacheLength || !length)
                    return null;

                // if the exact item exists, use it
                if (data[q]) {
                    return data[q];
                } else
                    if (options.matchSubset) {
                    for (var i = q.length - 1; i >= options.minChars; i--) {
                        var c = data[q.substr(0, i)];
                        if (c) {
                            var csub = [];
                            $.each(c, function(i, x) {
                                if (matchSubset(x.value, q)) {
                                    csub[csub.length] = x;
                                }
                            });
                            return csub;
                        }
                    }
                }
                return null;
            }
        };
    };

    $.Autocompleter.Select = function(options, input, select, config) {
        var CLASSES = {
            ACTIVE: "ac_over"
        };

        var listItems,
		active = -1,
		data,
		term = "",
		needsInit = true,
		element,
		list;

        // Create results
        function init() {
            if (!needsInit)
                return;
            element = $("<div/>")
		.hide()
		.addClass(options.resultsClass)
		.css("position", "absolute")
		.appendTo(document.body);

            list = $("<ul/>").appendTo(element).mouseover(function(event) {
                if (target(event).nodeName && target(event).nodeName.toUpperCase() == 'LI') {
                    active = $("li", list).removeClass(CLASSES.ACTIVE).index(target(event));
                    $(target(event)).addClass(CLASSES.ACTIVE);
                }
            }).click(function(event) {
                $(target(event)).addClass(CLASSES.ACTIVE);
                select();
                // TODO provide option to avoid setting focus again after selection? useful for cleanup-on-focus
                input.focus();
                return false;
            }).mousedown(function() {
                config.mouseDownOnSelect = true;
            }).mouseup(function() {
                config.mouseDownOnSelect = false;
            });

            if (options.width > 0)
                element.css("width", options.width);

            needsInit = false;
        }

        function target(event) {
            var element = event.target;
            while (element && element.tagName != "LI")
                element = element.parentNode;
            // more fun with IE, sometimes event.target is empty, just ignore it then
            if (!element)
                return [];
            return element;
        }

        function moveSelect(step) {
            listItems.slice(active, active + 1).removeClass(CLASSES.ACTIVE);
            movePosition(step);
            var activeItem = listItems.slice(active, active + 1).addClass(CLASSES.ACTIVE);
            var result = $.data(activeItem[0], "ac_data").result;
            $(input).val(result);
            $.Autocompleter.Selection(input, 0, input.value.length);

            var resultsElement = $('.' + options.resultsClass);

            if (options.scroll) {
                var offset = 0;
                listItems.slice(0, active).each(function() {
                    offset += this.offsetHeight;
                });

                if ((offset + activeItem[0].offsetHeight - resultsElement.scrollTop()) > resultsElement[0].clientHeight) {
                    resultsElement.scrollTop(offset + activeItem[0].offsetHeight - resultsElement.innerHeight());
                } else if (offset < resultsElement.scrollTop()) {
                    resultsElement.scrollTop(offset);
                }

            }
        };

        function movePosition(step) {
            active += step;
            if (active < 0) {
                active = listItems.size() - 1;
            } else if (active >= listItems.size()) {
                active = 0;
            }
        }

        function limitNumberOfItems(available) {
            return options.max && options.max < available
			? options.max
			: available;
        }

        var repositionTimer = null;

        function applyPositionToDropDown() {

            var whereToPen, whereToPenPosition, elementWidth, newHeight;

            var offset = $(input).offset();
            // re-motion: calculate best position where to open dropdown list
            var position = $.Autocompleter.calculateSpaceAround(input);

            if (position.spaceVertical == 'T') {

                //element.css('bottom', position.bottom + input.offsetHeight);
                topPosition = 'auto';
                bottomPosition = position.bottom + input.offsetHeight;

                if (options.scrollHeight > position.bottom) {
                    var maxHeight = position.top;
                } else {
                    var maxHeight = options.scrollHeight;
                }

            } else {
                //element.css('top', offset.top + input.offsetHeight);
                bottomPosition = 'auto';
                topPosition = offset.top + input.offsetHeight;
                if (options.scrollHeight > position.bottom) {
                    var maxHeight = position.bottom;
                } else {
                    var maxHeight = options.scrollHeight;
                }

            }

            // re-motion: need to resize list to specified width in css not in plugin config
            var elementWidth;
            if (options.width > 0) {
                elementWidth = options.width;
            } else if (parseInt(element.css('width')) > 0) {
                elementWidth = element.css('width');
            } else {
                elementWidth = $(input).width() + $('#' + options.dropDownButtonId).width();
            }

            element.css({
                width: elementWidth,
                left: offset.left,
                height: maxHeight,
                top: topPosition,
                bottom: bottomPosition
            });

        }

        function fillList() {
            list.empty();
            var max = data.length;
            for (var i = 0; i < max; i++) {
                if (!data[i])
                    continue;
                var formatted = options.formatItem(data[i].data, i + 1, max, data[i].value, term);
                if (formatted === false)
                    continue;
                var li = $("<li/>").html(options.highlight(formatted, term)).addClass(i % 2 == 0 ? "ac_even" : "ac_odd").appendTo(list)[0];
                $.data(li, "ac_data", data[i]);
            }
            listItems = list.find("li");
            if (options.selectFirst()) {
                listItems.slice(0, 1).addClass(CLASSES.ACTIVE);
                active = 0;
            }
            // apply bgiframe if available
            if ($.fn.bgiframe)
                list.bgiframe();
        }

        return {
            display: function(d, q) {
                init();
                data = d;
                term = q;
                fillList();
            },
            next: function() {
                moveSelect(1);
            },
            prev: function() {
                moveSelect(-1);
            },
            pageUp: function() {
                if (active != 0 && active - 8 < 0) {
                    moveSelect(-active);
                } else {
                    moveSelect(-8);
                }
            },
            pageDown: function() {
                if (active != listItems.size() - 1 && active + 8 > listItems.size()) {
                    moveSelect(listItems.size() - 1 - active);
                } else {
                    moveSelect(8);
                }
            },
            hide: function() {
                element && element.hide();
                listItems && listItems.removeClass(CLASSES.ACTIVE);
                active = -1;
                if (repositionTimer) clearInterval(repositionTimer);
            },
            visible: function() {
                return element && element.is(":visible");
            },
            current: function() {
                return this.visible() && (listItems.filter("." + CLASSES.ACTIVE)[0] || options.selectFirst() && listItems[0]);
            },
            show: function() {

                // re-motion: reposition element 
                applyPositionToDropDown();
                element.show();
                // re-motion: start interval to reposition element 
                if (repositionTimer) clearInterval(repositionTimer);
                repositionTimer = setInterval(applyPositionToDropDown, options.repositionInterval);

                //re-motion: block blur bind as long we scroll dropDown list 
                var revertInputStausTimeout = null;
                function revertInputStaus() {
                    config.mouseDownOnSelect = false;
                    $(input).focus();
                }
                element.scroll(function() {
                    config.mouseDownOnSelect = true;
                    if (revertInputStausTimeout) clearTimeout(revertInputStausTimeout);
                    revertInputStausTimeout = setTimeout(revertInputStaus, 50);
                });

                //re-motion: scroll dropDown list to value from input
                listItems.each(function(i) {
                    if (this.outerText == $(input).val()) {
                        element.scrollTop(this.offsetTop);
                        $(this).addClass(CLASSES.ACTIVE); ;
                        active = i;
                        return;
                    }
                });


                if (options.scroll) {
                    list.scrollTop(0);

                    if ($.browser.msie && typeof document.body.style.maxHeight === "undefined") {
                        var listHeight = 0;
                        listItems.each(function() {
                            listHeight += this.offsetHeight;
                        });
                        var scrollbarsVisible = listHeight > options.scrollHeight;
                        list.css('height', scrollbarsVisible ? options.scrollHeight : listHeight);
                        if (!scrollbarsVisible) {
                            // IE doesn't recalculate width when scrollbar disappears
                            listItems.width(list.width() - parseInt(listItems.css("padding-left")) - parseInt(listItems.css("padding-right")));
                        }
                    }

                }

            },
            selected: function() {
                // re-motion: removing the CSS class does not provide any benefits, but prevents us from highlighting the currently selected value
                // on dropDownButton Click
                // Original: var selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
                var selected = listItems && listItems.filter("." + CLASSES.ACTIVE);
                return selected && selected.length && $.data(selected[0], "ac_data");
            },
            emptyList: function() {
                list && list.empty();
            },
            unbind: function() {
                element && element.remove();
            }
        };
    };

    $.Autocompleter.Selection = function(field, start, end) {
        if (field.value.length < 2)
            return;

        if (field.createTextRange) {
            var selRange = field.createTextRange();
            selRange.collapse(true);
            selRange.moveStart("character", start);
            selRange.moveEnd("character", end);
            selRange.select();
        } else if (field.setSelectionRange) {
            field.setSelectionRange(start, end);
        } else {
            if (field.selectionStart) {
                field.selectionStart = start;
                field.selectionEnd = end;
            }
        }
        field.focus();
    };

    $.Autocompleter.calculateSpaceAround = function(element) {
        // re-motion: make sure CSS values are allways numbers, IE can return 'auto'
        function number(num) {
            return parseInt(num) || 0;
        };

        var element = $(element);
        // re-motion: check position where to place the element
        var offsetParent = element.offsetParent();
        var pos = element.position();
        // re-motion: position and dimensions of the element
        var top = number(pos.top) + number(element.css('margin-top')); // IE can return 'auto' for margins
        var left = number(pos.left) + number(element.css('margin-left'));
        var width = element.outerWidth();
        var height = element.outerHeight();

        // re-motion: calculate space arrownd the element
        var scrollTop = number($(document).scrollTop());
        var scrollLeft = number($(document).scrollLeft());
        var documentWidth = number($(window).width());
        var documentHeight = number($(window).height());
        var windowRight = scrollLeft + documentWidth;
        var windowBottom = scrollTop + documentHeight;

        var space = new Object();
        space.top = element.offset().top - scrollTop;
        space.bottom = documentHeight - ((element.offset().top + height) - scrollTop);
        space.left = element.offset().left - scrollLeft;
        space.right = documentWidth - ((element.offset().left + width) - scrollLeft);

        (space.top > space.bottom) ? space.spaceVertical = 'T' : space.spaceVertical = 'B';
        (space.left > space.right) ? space.spaceHorizontal = 'L' : space.spaceHorizontal = 'R';
        space.space = space.spaceVertical + space.spaceHorizontal;

        return space;
    }

})(jQuery);