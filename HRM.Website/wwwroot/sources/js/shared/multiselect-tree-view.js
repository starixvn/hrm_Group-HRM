var multiselectTreeViewId = 1;
(function ($) {
    // Method expose for user use
    var methods = {
        init: function (opts) {
            opts = $.extend({}, $.fn.multiSelectTreeView.defaults, opts);
            return this.each(function () {
                return new multiSelectTreeView(this, opts);
            });
        },
        getSelectedItems: function () {
            var $me = $(this);
            var items = $me.find('input[data-is-leaf=true]:checked'),
                totalItems = items.length,
                values = [];
            for (var i = 0; i < totalItems; i++) {
                values.push(items[i].value);
            }

            return values.join(',');
        }
    };

    // Plugin
    $.fn.multiSelectTreeView = function (options) {
        if (methods[options]) {
            return methods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof options === 'object' || !options) {
            return methods.init.apply(this, arguments);
        }
        else {
            throw 'Method does not supported!';
        }
    };

    // Default options
    $.fn.multiSelectTreeView.defaults = {
        singleSelection: false,
        noneSelectedText: 'Select options',
        selectedText: '# selected',
        selectedAllText: 'All selected',
        disabled: false,
        isSelectedAll: false,
        hasSearchBox: false,
        data: [],
        selectedItems: '',
        onSelect: function () { },
        onSelectAll: function () { }
    };

    var multiSelectTreeView = function (multiSelect, options) {
        var me = this;
        me.options = options;
        me.id = 'multiselect-treeview-' + (multiSelect.id ? multiSelect.id : multiselectTreeViewId++);
        me.$multiSelect = $(multiSelect);

        me.initialize();

        return me.$multiSelect;
    };

    multiSelectTreeView.prototype = {
        initialize: function () {
            var me = this;
            var html = me.render();

            //add global variable
            me.$multiSelect = me.$multiSelect.html(html);
            me.$dropdownbtn = me.$multiSelect.find('.multiselect-treeview-dropdown-button').first();
            me.$updownicon = me.$multiSelect.find('.up-down-icon').first();
            me.$selectedtext = me.$multiSelect.find('.selected-text').first();
            me.$dropdownlist = me.$multiSelect.find('.multiselect-treeview-dropdown-list').first();
            me.$header = me.$multiSelect.find('.multiselect-treeview-select-all');
            me.$selectAll = me.$multiSelect.find('#' + me.id + '-selectAll');
            me.$groupitems = me.$multiSelect.find('.multiselect-treeview-group-items');
            me.$checkboxes = me.$multiSelect.find('.multiselect-treeview-items input[type=checkbox]:not(:disabled)');

            if (me.options.hasSearchBox) {
                me.$searchInput = me.$multiSelect.find('.multiselect-treeview-search-box input');
                me.$clearButton = me.$multiSelect.find('.multiselect-treeview-clear-indicator');
            }

            me.registerEvents();
            me.updateSelectedItems();
        },
        render: function () {
            var me = this,
                options = me.options;
            return '<div id="' + me.id + '" class="multiselect-treeview">'
                + '<div class="multiselect-treeview-dropdown-button ' + (options.disabled ? 'disabled' : '') + '">'
                + '<span class="selected-text">' + options.noneSelectedText + '</span>'
                + '<span class="up-down-icon"><span class="down"></span></span>'
                + '</div>'
                + '<div class="multiselect-treeview-dropdown-list hidden">'
                + me.renderSearchBox()
                + me.renderCheckAll()
                + me.renderItems()
                + '</div>' +
                '</div>';
        },
        renderSearchBox: function () {
            var me = this,
                options = me.options,
                html = '';
            if (options.hasSearchBox) {
                html = '<div class="multiselect-treeview-search-box">'
                    + '<input type="text" placeholder="Search..." />'
                    + '<span class="multiselect-treeview-clear-indicator">'
                        + `<svg height="20" width="20" viewBox="0 0 20 20" focusable="false">
                            <path d="M14.348 14.849c-0.469 0.469-1.229 0.469-1.697 0l-2.651-3.030-2.651 3.029c-0.469 0.469-1.229 0.469-1.697 0-0.469-0.469-0.469-1.229
                                0-1.697l2.758-3.15-2.759-3.152c-0.469-0.469-0.469-1.228 0-1.697s1.228-0.469 1.697 0l2.652 3.031
                                2.651-3.031c0.469-0.469 1.228-0.469 1.697 0s0.469 1.229 0 1.697l-2.758 3.152 2.758 3.15c0.469 0.469 0.469 1.229 0 1.698z" /></svg>`
                    + '</span>'
                    + '</div>';
            }

            return html;
        },
        renderCheckAll: function () {
            var me = this,
                options = me.options,
                html = '';
            if (!options.singleSelection) {
                html = '<div class="multiselect-treeview-select-all">'
                    + '<div class="checkbox-wrapper">'
                    + '<input type="checkbox" id="' + me.id + '-selectAll" ' + (options.isSelectedAll ? 'checked' : '') + ' />'
                    + '<label for="' + me.id + '-selectAll" > Select all</label>'
                    + '</div>'
                    + '</div>';
            }

            return html;
        },
        renderItems: function () {
            var me = this,
                options = me.options,
                html = [],
                totalItems = options.data.length;
            html.push('<div class="multiselect-treeview-items">');
            for (var i = 0; i < totalItems; i++) {
                var data = options.data[i],
                    itemId = 'item-' + i,
                    disabled = options.disabled || (options.singleSelection && !data.isLeaf);

                html.push('<ul class="multiselect-treeview-group-items">'
                    + '<li class="multiselect-treeview-item">'
                    + '<div class="checkbox-wrapper">'
                    + '<input type="checkbox" id="' + me.id + '-' + itemId + '" value="' + data.id + '" data-is-leaf="' + data.isLeaf + '" '
                    + (data.checked || options.isSelectedAll ? 'checked' : '') + ' ' + (disabled ? 'disabled' : '') + ' />'
                    + '<label for="' + me.id + '-' + itemId + '">' + data.text + '</label>'
                    + '</div>');

                if (data.items && data.items.length > 0) {
                    html.push(me.renderChilds({
                        data: data.items,
                        parentId: itemId
                    }));
                }

                html.push('</li></ul>');
            }

            html.push('</div>');
            return html.join('');
        },
        renderChilds: function (item) {
            var me = this,
                options = me.options,
                html = [],
                totalItems = item.data.length;
            html.push('<ul class="multiselect-treeview-group-items">');
            for (var i = 0; i < totalItems; i++) {
                var data = item.data[i],
                    itemId = item.parentId + '-' + i,
                    disabled = options.disabled || (options.singleSelection && !data.isLeaf);
                html.push('<li class="multiselect-treeview-item is-child">'
                    + '<div class="checkbox-wrapper">'
                    + '<input type="checkbox" id="' + me.id + '-' + itemId + '" value="' + data.id + '" data-is-leaf="' + data.isLeaf + '" '
                    + ' data-parentid="' + me.id + '-' + item.parentId + '"'
                    + ' ' + (data.checked || options.isSelectedAll ? 'checked' : '') + ' ' + (disabled ? 'disabled' : '') + ' />'
                    + '<label for="' + me.id + '-' + itemId + '">' + data.text + '</label>'
                    + '</div>');

                if (data.items && data.items.length > 0) {
                    html.push(me.renderChilds({
                        data: data.items,
                        parentId: itemId
                    }));
                }

                html.push('</li>');
            }

            html.push('</ul>');
            return html.join('');
        },
        updateSelectedText: function () {
            var me = this,
                options = me.options,
                $checkboxes = me.$checkboxes.filter('[data-is-leaf=true]'),
                $checked = $checkboxes.filter(':checked');

            if ($checked.length === 0) {
                me.$selectedtext.text(options.noneSelectedText);
                me.$selectAll.prop('checked', false);
            } else if ($checkboxes.length === $checked.length) {
                me.$selectedtext.text(options.selectedAllText);
                me.$selectAll.prop('checked', true);
            } else {
                if (options.singleSelection) {
                    var text = $($checked[0]).closest('.checkbox-wrapper').find('label').html();
                    me.$selectedtext.text(text);
                } else {
                    me.$selectedtext.text(options.selectedText.replace('#', $checked.length));
                    me.$selectAll.prop('checked', false);
                }
            }
        },
        updateSelectedItems: function () {
            var me = this,
                options = me.options,
                checkboxes = me.$checkboxes.filter('[data-is-leaf=true]'),
                totalItems = checkboxes.length;

            if (options.selectedItems && options.selectedItems.length > 0) {
                var selectedItems = options.selectedItems.split(',');
                for (var i = 0; i < totalItems; i++) {
                    var $checkbox = $(checkboxes[i]);
                    var index = selectedItems.findIndex(x => x === $checkbox.val());
                    if (index >= 0) {
                        $checkbox.prop('checked', true).change();
                    }
                }
            } else {
                me.updateSelectedText();
            }

            if (totalItems === checkboxes.filter(':checked').length) {
                me.$checkboxes.prop('checked', true);
            }
        },
        registerEvents: function () {
            var me = this,
                options = me.options;

            var updateParent = function () {
                var $this = $(this),
                    parentId = $this.data('parentid');

                if (parentId) {
                    var checked = $this.prop('checked'),
                        $parent = me.$multiSelect.find('#' + parentId),
                        $siblings = me.$checkboxes.filter('[data-parentid=' + parentId + ']'),
                        $siblingschecked = $siblings.filter(':checked');
                    $parent.prop('checked', $siblings.length === $siblingschecked.length && checked);
                    updateParent.call($parent.get(0));
                }
            };

            me.$dropdownbtn.off('click').on('click', function () {
                if (me.$dropdownlist.hasClass('hidden')) {
                    me.$updownicon.find('span').removeClass('down').addClass('up');
                    me.$dropdownlist.removeClass('hidden');
                }
            });
            me.$selectAll.on('change', function () {
                var $this = $(this);
                if ($this.prop('checked')) {
                    me.$checkboxes.not('.hide').prop('checked', true);
                } else {
                    me.$checkboxes.not('.hide').prop('checked', false);
                }

                if (typeof options.onSelectAll === 'function') {
                    options.onSelectAll.call(this);
                }

                me.updateSelectedText();
            });
            me.$checkboxes.on('change', function () {
                var $this = $(this),
                    isChecked = $this.prop('checked');

                if (options.singleSelection) {
                    me.$checkboxes.not(this).prop('checked', false);
                } else {
                    me.$checkboxes.filter('[data-parentid=' + $this.attr('id') + ']').prop('checked', isChecked).change();
                    updateParent.call($this.get(0));
                }

                me.updateSelectedText();

                if ($this.attr('data-is-leaf') === 'true' && typeof options.onSelect === 'function') {
                    options.onSelect.call(this);
                }
            });

            if (me.options.hasSearchBox) {
                me.$searchInput.off('keyup').on('keyup', function () {
                    var $this = $(this);
                    var filterVal = $this.val().toLowerCase();
                    if (filterVal !== '') {
                        var matcher = new RegExp(filterVal, 'gi');

                        me.$groupitems.find('li.multiselect-treeview-item').removeClass('show')
                            .find('input').removeClass('show');
                        var showElements = me.$groupitems.find('li.multiselect-treeview-item').filter(function () {
                            if (matcher.test($(this).find('> .checkbox-wrapper label').text())) {
                                return $(this);
                            };
                        }).removeClass('hide').addClass('show');

                        if (showElements.length > 0) {
                            me.$header.show();

                            showElements.each(function () {
                                var showElement = $(this);
                                var prevParent = showElement.parent().prev();
                                var grandParent = showElement.parent().parent();

                                while (grandParent.hasClass('multiselect-treeview-item')) {
                                    prevParent.find('input').addClass('show');
                                    prevParent = grandParent.parent().prev();

                                    grandParent.removeClass('hide').addClass('show');
                                    grandParent = grandParent.parent().parent();
                                }

                                showElement.find('li.multiselect-treeview-item')
                                    .removeClass('hide').addClass('show')
                                    .find('input').addClass('show');
                            });
                        } else {
                            me.$header.hide();
                        }

                        me.$groupitems.find('li.multiselect-treeview-item').not('.show').addClass('hide')
                            .find('input').addClass('hide');
                    } else {
                        me.$header.show();
                        me.$groupitems.find('li.multiselect-treeview-item')
                            .removeClass('hide').addClass('show')
                            .find('input').removeClass('hide').addClass('show');
                    }

                    me.$checkboxes.not('.hide').filter('[data-is-leaf=true]').each(function () {
                        $(this).change();
                    });
                });

                me.$clearButton.off('click').on('click', function () {
                    if (me.$searchInput.val() !== '') {
                        me.$searchInput.val('');
                        me.$searchInput.trigger('keyup');
                    }
                });
            }

            // hide dropdown
            $(document).delegate('html', 'click', function (e) {
                var $wrapper = $('#' + me.id);
                if (!$wrapper.is(e.target) && $wrapper.has(e.target).length === 0 && $wrapper.is(':visible') && me.$updownicon.find('span').hasClass('up')) {
                    me.$updownicon.find('span').removeClass('up').addClass('down');
                    me.$dropdownlist.addClass('hidden');
                }
            });
        }
    };
})(jQuery);