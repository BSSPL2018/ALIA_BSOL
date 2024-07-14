Date.prototype.toDate = function () {
    this.setHours(0, 0, 0, 0);
    return this;
};
Date.prototype.isValid = function () {
    return !(this.getFullYear() < 1900);
};
$(document).ready(function () {
    $.ajaxSetup({ cache: false });// Disable caching of AJAX requests
    ConfigureGrid();
    OnPageLoad();
    imagePreview();
});
$(document).ajaxSuccess(function (event, request, options) {
    try {
        if (request.responseText == "SessionExpired();" || (request.responseJSON && request.responseJSON.Message == "SessionExpired"))
            SessionExpired();
    } catch (e) { }
});
$(document).ajaxError(function (event, xhr, ajaxOptions, thrownError) {
    $('button[data-ajax-action=1]').prop('disabled', false).removeAttr('data-ajax-action');
    if (xhr.status === 401) {
        location.href = "/Index?ReturnUrl=" + location.pathname + location.search;
    }
    else if (xhr.status === 500 || xhr.status === 403) {
        kendoAlert(xhr.getAllResponseHeaders().indexOf("content-type: text/html") >= 0 ? "Internal Server Error" : xhr.responseText, true, "", function () {
            location.reload();
        });
    }
    else if (xhr.status === 400 && xhr.responseJSON) {
        var errors = [];
        var data = xhr.responseJSON.errors ? xhr.responseJSON.errors : xhr.responseJSON;
        for (var prop in data) {
            errors = errors.concat(data[prop]);
        }
        kendoAlert(errors.join("<br>"), true);
    }
});
$.postData = function (url, data, callBack) {
    $('button:not(.btn-grid)').prop('disabled', true).attr('data-ajax-action', 1);
    $.post(url, data && data instanceof Object ? encodeData(cloneObject(data)) : data, function (res) {
        $('button:not(.btn-grid)').prop('disabled', false).removeAttr('data-ajax-action');
        if (!callBack) return;
        callBack(res && res instanceof Object ? decodeData(res) : res);
    });
};
$.getData = function (url, data, callBack) {
    $.post(url, data && data instanceof Object ? encodeData(cloneObject(data)) : data, function (res) {
        if (!callBack) return;
        callBack(res && res instanceof Object ? decodeData(res) : res);
    });
};
$.postJson = function (url, data, callBack) {
    $('button:not(.btn-grid)').prop('disabled', true).attr('data-ajax-action', 1);
    var jsonFormatter = function (key, value) {
        if (this[key] instanceof Date)
            return kendo.toString(this[key], 'yyyy-MM-ddTHH:mm');
        return value;
    };
    $.ajax({
        type: 'POST',
        url: url,
        data: data && data instanceof Object ? JSON.stringify(data, jsonFormatter) : data,
        contentType: "application/json",
        dataType: 'json',
        success: function (res) {
            $('button:not(.btn-grid)').prop('disabled', false).removeAttr('data-ajax-action');
            if (!callBack) return;
            callBack(res && res instanceof Object ? decodeData(res) : res);
        }
    });
};
// attach kendoAlert to the window
window.kendoAlert = (function () {
    var win = $("<div class='k-alert'>").kendoWindow({
        modal: true,
    }).getKendoWindow();
    win.wrapper.addClass('k-alert-window k-alert-wnd');

    return function (msg, isError, title, onClose) {
        if (!msg) {
            msg = "An error occured while process your request.<br/> Please Contact the Application Support Team.";
            isError = true;
            title = "BSOL - Error";
        }
        win._events.close = onClose ? [onClose] : [];

        win.content('<div class="w-content">' + (msg.replace(/\n/g, "<br/>").replace(/\\n/g, "<br/>")) + '</div><div class="w-btn"><input type="button" value="Ok" onclick="CloseAlert()" /></div>');

        if (isError && isError == true) {
            $('.k-alert-wnd .k-window-titlebar').removeClass('wnd-info').addClass('wnd-error');
        }
        else {
            $('.k-alert-wnd .k-window-titlebar').removeClass('wnd-error').addClass('wnd-info');
        }

        if (title && title != '')
            win.title(title);
        else
            title = "BSOL";

        win.title(title);
        win.center().open();
        setTimeout(function () { $('.k-alert-wnd input').focus(); }, 500);
    };

}());
window.kendoConfirm = (function () {

    var win = $("<div class='k-confirm'>").kendoWindow({
        modal: true,
    }).getKendoWindow();
    win.wrapper.addClass('k-alert-window k-confirm-wnd');

    return function (msg, onYesClick, onNoClick, title) {
        win.content('<div class="w-content">' + msg + '</div><div class="w-btn"><input type="button" value="Yes" name="Yes" />&nbsp;<input type="button" value="No" onclick="CloseConfirm()" /></div>');

        if (title && title != '')
            win.title(title);
        else
            title = "BSOL";

        win.title(title);
        win.center().open();
        $('.k-confirm [name=Yes]').click(function () {
            CloseConfirm();
            if (onYesClick)
                onYesClick();
        });
        $('.k-confirm [name=No]').click(function () {
            CloseConfirm();
            if (onNoClick)
                onNoClick();
        });
        setTimeout(function () { $('.k-confirm-wnd input:first').focus(); }, 500);
    };

}());
window.kendoPrompt = (function () {

    var win = $("<div class='k-prompt'>").kendoWindow({
        modal: true,
    }).getKendoWindow();
    win.wrapper.addClass('k-alert-window k-prompt-wnd');

    return function (msg, defaultValue, onYesClick, title, btnText, controlType) {
        if (!btnText || btnText.trim() == "")
            btnText = "Ok";

        if (!controlType || controlType.trim() == "")
            controlType = "textarea";

        var control;
        if (controlType == "text" || controlType == "email")
            control = '<input id="txtPrompt" type="' + controlType + '" value="' + (defaultValue ? defaultValue : "") + '" class="prompt-txt text-box" required />';
        else
            control = '<textarea id="txtPrompt" rows="1" class="prompt-txt text-box" required>' + (defaultValue ? defaultValue : "") + '</textarea>';

        win.content('<form onsubmit="return false"><div class="w-content"><label for="txtPrompt">' + msg + '</label>' + control + '</div><div class="w-btn"><input type="button" value="' + btnText + '" name="wndbtnOk" />&nbsp;<input type="button" value="Cancel" name="wndbtnCancel" onclick="ClosePrompt()" /></div></form>');

        if (title && title != '')
            win.title(title);
        else
            title = "BSOL";

        win.title(title);
        win.center().open();
        $('.k-prompt [name=wndbtnOk]').click(function () {
            var $form = $('.k-prompt form')[0];
            if ($form && $form.checkValidity && $form.reportValidity && !$form.checkValidity()) {
                $form.reportValidity();
                return;
            }

            if (!$('#txtPrompt').val().trim())
                return;
            ClosePrompt();
            if (onYesClick)
                onYesClick($('#txtPrompt').val().trim());
        });
        setTimeout(function () { $('.k-prompt-wnd input:first').focus(); }, 500);
    };

}());
function CloseAlert() {
    $('.k-alert').getKendoWindow().close();
}
function CloseConfirm() {
    $('.k-confirm').getKendoWindow().close();
}
function ClosePrompt() {
    $('.k-prompt').getKendoWindow().close();
}
function kendoWindow_Refresh() {
    this.center();
}
function ConfigureGrid() {
    $('[data-role=grid]').each(function () {
        var dsEvents = $(this).getKendoGrid().dataSource._events;
        if (dsEvents.hasOwnProperty("requestEnd"))
            dsEvents.requestEnd.push(Grid_RequestEnd);
        else
            dsEvents.requestEnd = [Grid_RequestEnd];

        if ($(this).hasClass("cb-grid")) {
            var gdEvents = $(this).getKendoGrid()._events;
            if (gdEvents.hasOwnProperty("edit"))
                gdEvents.edit.push(Grid_Edit);
            else
                gdEvents.edit = [Grid_Edit];
        }
    });
    $('.v-grid').each(function () {
        var $grid = $(this);
        if ($grid.length) {
            var $gridheader = $grid.find('.k-grid-header');
            if (!$gridheader.length)
                return;
            var $pager = $grid.find('.k-grid-pager');
            var pagerHeight = 0;
            if ($pager.length)
                pagerHeight = $pager.outerHeight();
            var $footer = $grid.find('.k-grid-footer');
            if ($footer.length)
                pagerHeight += $footer.outerHeight();
            var footerHeight = ($('.content-footer').outerHeight() ? $('.content-footer').outerHeight() : 0) + ($('.footer-abrv').outerHeight() ? $('.footer-abrv').outerHeight() : 0);
            var gridHeight = Math.min(Math.max($(window).height() - $gridheader.offset().top - $gridheader.outerHeight() - footerHeight - pagerHeight - 3, 250), $(window).height());
            gridHeight = $grid.attr("data-wnd") == 1 ? gridHeight - 200 : gridHeight;
            $grid.find('.k-grid-content').css('height', gridHeight + "px");

            if ($grid.find('.k-grid-content-locked').length && $grid.find('.k-grid-content').length) {
                var ele = $grid.find('.k-grid-content')[0];
                var scrollBarHeight = Math.max(ele.offsetHeight - ele.clientHeight, 0);
                $grid.find('.k-grid-content-locked').css('height', (gridHeight - scrollBarHeight) + "px");
            }
        }
    });

    $('.k-grid-filter').click(function () {
        setTimeout(function () { $('.k-filter-menu:visible .k-textbox').focus(); }, 300);
    });
}
function OnPageLoad() {
    $('#main-content').css('min-height', ($(window).height() - $('header').outerHeight() - $('.footer').outerHeight()) + 'px');
    //$('.g-title').css('top', $('.g-title').offset().top + 'px');
    //Cursor is blinking frequently
    //$('.k-input,.k-textbox').focus(function () {
    //    var $ele = $(this);
    //    setTimeout(function () { $ele.select(); }, 100);
    //});

    $('.toggle-btn').click(function (e) {
        e.preventDefault();

        var target = $(this).attr('data-target');
        $(target).toggle();
        var editMode = !$(target).is(':hidden');
        var dir = $(this).attr('data-orientation');
        var hideNav = false;
        if (editMode) {
            $('.toggle-btn.btn-open[data-target="' + target + '"]').hide();
            $('.toggle-btn.btn-minus[data-target="' + target + '"]').show();
            if (dir == "Horizontal")
                $('#displayContainer').removeClass('col-md-12').addClass('col-md-9');
            $(this).parents('.v-edit').addClass('tg-open');
            hideNav = !$('.app.header-default').hasClass('side-nav-folded');
        }
        else {
            $('.toggle-btn.btn-open[data-target="' + target + '"]').show();
            $('.toggle-btn.btn-minus[data-target="' + target + '"]').hide();
            if (dir == "Horizontal")
                $('#displayContainer').removeClass('col-md-9').addClass('col-md-12');
            $(this).parents('.v-edit').removeClass('tg-open');
            hideNav = $('.app.header-default').hasClass('side-nav-folded');
        }

        window[$(this).attr("data-click")](editMode, true);

        if ($(this).attr('data-orientation') == 'Horizontal' && !$('.app.header-default').hasClass('default-fold') && hideNav)
            $('.sidenav-fold-toggler').click();
    });

    jQuery.fn.extend({
        open: function () {
            var target = this.attr('data-target');
            var dir = $(this).attr('data-orientation');
            $('.toggle-btn.btn-open[data-target="' + target + '"]').hide();
            this.show();
            $(target).show();
            if (dir == 'Horizontal')
                $('#displayContainer').removeClass('col-md-12').addClass('col-md-9');
            $(this).parents('.v-edit').addClass('tg-open');
            window[$(this).attr("data-click")](true, false);
            if (dir == 'Horizontal' && !$('.app.header-default').hasClass('default-fold') && !$('.app.header-default').hasClass('side-nav-folded'))
                $('.sidenav-fold-toggler').click();

            $('body').animate({ scrollTop: $(".main-content").offset().top }, 10);
        },
        close: function () {
            var target = this.attr('data-target');
            var dir = $(this).attr('data-orientation');
            this.hide();
            $('.toggle-btn.btn-open[data-target="' + target + '"]').show();
            $(target).hide();
            if (dir == 'Horizontal')
                $('#displayContainer').removeClass('col-md-9').addClass('col-md-12');
            $(this).parents('.v-edit').removeClass('tg-open');
            window[$(this).attr("data-click")](false, false);
            if (dir == 'Horizontal' && !$('.app.header-default').hasClass('default-fold') && $('.app.header-default').hasClass('side-nav-folded'))
                $('.sidenav-fold-toggler').click();
        }
    });

    $('[data-role=upload]').each(function () {
        var $upload = $(this).getKendoUpload();
        var uploadEvents = $upload._events;
        if (uploadEvents.hasOwnProperty("select"))
            uploadEvents.select.unshift(onUploadSelect);
        else
            uploadEvents.select = [onUploadSelect];
    });
    $('[data-role=multiselect]').each(function () {
        var $multiSelect = $(this).getKendoMultiSelect();
        if ($multiSelect.popup && $multiSelect.popup.element)
            $multiSelect.popup.element.addClass("multi-select-popup");
    });
}
function encodeData(data) {
    if (data && data instanceof Object) {
        for (var prop in data) {
            if (data[prop] instanceof Date) {
                data[prop] = kendo.toString(data[prop], 'yyyy-MM-ddTHH:mm');
            }
            else if (data[prop] instanceof Object)
                data[prop] = encodeData(data[prop]);
            else if (typeof data[prop] === "string" && /<[a-z][\s\S]*>/i.test(data[prop]))
                data[prop] = encodeURIComponent(data[prop]);
        }
    }
    return data;
}
function decodeData(data) {
    if (data && data instanceof Object) {
        for (var prop in data) {
            if (typeof data[prop] === "string" && data[prop]) {
                if (data[prop].indexOf('/Date') === 0)
                    data[prop] = JDate(data[prop]);
                else if ((data[prop].length == 16 || data[prop].length == 19) && data[prop].indexOf('-') == 4 && data[prop].lastIndexOf('-') == 7 && data[prop].indexOf(':') == 13)
                    data[prop] = new Date(data[prop]);
            }
            else if (data[prop] instanceof Object) {
                var props = Object.keys(data[prop]);
                if (props.length === 11 && props[0] === "Ticks" && props[props.length - 1] === "TotalSeconds")
                    data[prop] = data[prop].Hours + ':' + data[prop].Minutes;
                else
                    data[prop] = decodeData(data[prop]);
            }
        }
    }
    return data;
}
function cloneObject(obj) {
    return $.extend(true, {}, obj);
}
function cloneAndDecode(obj) {
    var replacer = function (key, value) {
        if (this[key] instanceof Date)
            return "Date: " + kendo.toString(this[key], 'yyyy-MM-ddTHH:mm');
        return value;
    };
    var reviver = function (key, value) {
        if (this[key] && this[key].length == 22 && this[key].indexOf('Date: ') == 0)
            return new Date(this[key].replace("Date: ", ""));
        return value;
    };
    return decodeData(JSON.parse(JSON.stringify(obj, replacer), reviver));
}
function JDate(jDate) {
    return !jDate || jDate instanceof Date || jDate.indexOf('/') < 0 ? jDate : new Date(parseInt(jDate.substr(6)));
}
function Grid_RequestEnd(e) {
    if (e.response && typeof e.response == "string" && e.response == "Session Expired")
        SessionExpired();
    else if (e.response && typeof e.response == "object" && e.response.hasOwnProperty("HasError") && e.response.HasError == true)
        kendoAlert(e.response.Message, true);
}
function Grid_Edit(e) {
    var combobox = e.container.find("[data-role=combobox]").getKendoComboBox();
    if (combobox && combobox.value() === "0")
        combobox.value("");
}
function SessionExpired() {
    alert("Session expired. This will automatically redirect you to login page.");
    location.href = "/Authenticate/Index?ReturnUrl=" + location.pathname + location.search;
}
function setModel(e, obj, prop, callBack) {
    obj[prop] = e.sender.value();
    if (callBack)
        callBack(e);
}
function setTimeModel(e, obj, prop, callBack) {
    obj[prop] = kendo.toString(e.sender.value(), 'HH:mm');
    if (callBack)
        callBack(e);
}
function validateDSText(dsRef, val) {
    return Object.values(dsRef.kendoWidget().data()).includes(val);
}
function validateDSValue(dsRef, val, key) {
    key = key ? key : "ID";
    return $.grep(dsRef.kendoWidget().data(), function (x) { return x[key] == val }).length > 0;
}
function ToMonthStart(val) {
    return new Date(val.getFullYear(), val.getMonth(), 1);
}
function ToDate(val) {
    if (!val)
        return "";
    return kendo.toString(val instanceof Date ? val : new Date(val), DATE_FORMAT ? DATE_FORMAT : 'dd-MMM-yyyy');
}
function ToDateTime(val) {
    if (!val)
        return "";
    return kendo.toString(val instanceof Date ? val : new Date(val), DATETIME_FORMAT ? DATETIME_FORMAT : 'dd-MMM-yyyy hh:mm tt');
}
function formatTime(ts) {
    if (!ts)
        return "";
    return ts instanceof Date ? kendo.toString(ts, 'HH:mm')
        : ts instanceof Object ? kendo.toString(new Date(2000, 1, 1, ts.Hours, ts.Minutes, 0, 0), 'HH:mm')
            : (ts.length == 8 ? ts.substr(0, 5) : ts);
}
function StdDate(val) {
    return kendo.toString(val, 'yyyy-MM-dd');
}
function StdDateTime(val) {
    return kendo.toString(val, 'yyyy-MM-dd hh:mm tt');
}
function TimeSpanToTime(ts) {
    if (!ts)
        return "";
    return ts instanceof Date ? kendo.toString(ts, 'HH:mm')
        : ts instanceof Object ? kendo.toString(new Date(2000, 1, 1, ts.Hours, ts.Minutes, 0, 0), 'HH:mm')
            : (ts.length == 8 ? ts.substr(0, 5) : ts);
}
function TimeSpanToDate(ts) {
    return kendo.toString(new Date(2000, 1, 1, ts.Hours, ts.Minutes, 0, 0));
}
function minToTime(minutes) {
    minutes = parseInt(minutes);
    if (isNaN(minutes) || minutes <= 0)
        return "0:00";

    return Math.floor(minutes / 60) + ':' + ('0' + (minutes % 60)).slice(-2);
}
function timeToMin(time) {
    time = time instanceof Date ? kendo.toString(time, 'HH:mm') : time;
    if (!time || time.split(":").length < 2)
        return 0;
    return (parseInt(time.split(":")[0]) * 60) + parseInt(time.split(":")[1]);
}
var DateDiff = {

    inMins: function (d1, d2) {
        d1.setMilliseconds(0);
        d1.setSeconds(0);
        d2.setMilliseconds(0);
        d2.setSeconds(0);

        var diff = Math.abs(d2.getTime() - d1.getTime());
        var hours = diff / 1000 / 60 / 60;
        return hours * 60;
    },

    inDays: function (d1, d2) {
        const _MS_PER_DAY = 1000 * 60 * 60 * 24;
        const utc1 = Date.UTC(d1.getFullYear(), d1.getMonth(), d1.getDate());
        const utc2 = Date.UTC(d2.getFullYear(), d2.getMonth(), d2.getDate());

        return Math.floor((utc2 - utc1) / _MS_PER_DAY);
    },

    inWeeks: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();

        return parseInt((t2 - t1) / (24 * 3600 * 1000 * 7));
    },

    inMonths: function (d1, d2) {
        d1 = d1 instanceof Date ? d1 : new Date(d1);
        d2 = d2 instanceof Date ? d2 : new Date(d2);
        var d1Y = d1.getFullYear();
        var d2Y = d2.getFullYear();
        var d1M = d1.getMonth();
        var d2M = d2.getMonth();

        return (d2M + 12 * d2Y) - (d1M + 12 * d1Y);
    },

    inYears: function (d1, d2) {
        return d2.getFullYear() - d1.getFullYear();
    }
};
function toUpper(e) {
    e.value = e.value.toUpperCase();
}
function ExportToExcel(gridName) {
    $("#" + (gridName ? gridName : "grid")).data("kendoGrid").saveAsExcel();
}
function grid_ErrorHandler(e) {
    if (e.errors) {
        var message = "";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        kendoAlert(message, true);
    }
}
function grid_ParameterMap(options, type) {
    if (type !== "read")
        return kendo.stringify(options.models);
    else
        return options;
}
function showError(cntrl, msg, parent, Isrequired = true) {
    $((parent ? (parent + ' ') : '') + 'label[for="' + cntrl.replace(/[.\[\]]/g, '_') + '"]').after("<span class='field-Err'>* " + (msg ? msg : !Isrequired ? "" : "Required") + "</span>");
}
function removeError() {
    $('.field-Err').remove();
}
function validateEmail(emailField) {
    if (emailField && emailField.trim() != "") {
        var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
        if (!reg.test(emailField)) {
            return false;
        }
    }
    return true;
}

function validateMobileNo(mobileNo) {
    //var mobileNolst = [];
    //mobileNolst = mobileNo.split('/');
    //var reg = /^[0-9]*$/;

    //var invalidMobileNo = $.grep(mobileNolst, function (el) {
    //    return (!reg.test(el) || el.length < 7 || el.length > 7);
    //});
    //if (invalidMobileNo.length) {
    //    return false;
    //}
    //return true;

    var mobileNolst = [];
    mobileNolst = mobileNo.split('/'); 

    var reg = /^(\+\d{10}|\d{7})$/; // To match 10 digits with + symbol or 7 digits without +

    var invalidMobileNo = $.grep(mobileNolst, function (el) {
        el = el.trim(); 
        return !reg.test(el);
    });

    if (invalidMobileNo.length) {
        return false;
    }

    return true;


}

function validateNicNo(nicNo) {
    var reg = /^[a-zA-Z0-9\'/-]*$/;
    if (!reg.test(nicNo)) {
        return false;
    }
    return true;
}

function validateNumeric(mobileNo) {
    var reg = /^[0-9]*$/;
    if (!reg.test(mobileNo)) {
        return false;
    }
    return true;
}
function validateNumeric(mobileNo) {
    var reg = /^[0-9]*$/;
    if (!reg.test(mobileNo)) {
        return false;
    }
    return true;
}
function validateWebsiteURL(websiteURL) {
    if (websiteURL != null) {
        var reg = /^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/;
        if (!reg.test(websiteURL)) {
            return false;
        }
    }
    return true;
}
function validatePhoneNo(phoneNum) {
    if (phoneNum && phoneNum.trim() != "" && phoneNum.length < 7) {
        return false;
    }
    return true;
}
//function validateMobileNo(mobileNum) {
//    if (mobileNum && mobileNum.trim() != "" && mobileNum.length < 7) {
//        return false;
//    }
//    return true;
//}
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function imagePreview() {
    $('.img-preview').on("click", ".img-cnt", function () {
        var windowHeight = window.innerHeight || $(window).height(),
            windowWidth = window.innerWidth || $(window).width();

        // Create the overlay, append it to body and make it visible.
        $('<div id="overlay"></div>')
            .css('opacity', '0')
            .animate({
                'opacity': '0.5'
            }, 'slow')
            .appendTo('body');


        // Create the lightbox container which shall contain the image
        $('<div id="lightbox"><span class="close">X</span></div>')
            .hide()
            .appendTo('body');

        // Create img-element and add to #lightbox when loaded.
        $('<img>')
            .attr('src', $(this).prop("tagName") == "img" ? $(this).attr('src') : $(this).css('background-image').replace('url(', '').replace(')', '').replace(/\"/gi, ""))
            .css({
                'max-height': windowHeight,
                'max-width': windowWidth
            }).on("load", function () {

                $('#lightbox')
                    .css({
                        'top': (windowHeight - $('#lightbox').height()) / 2,
                        'left': (windowWidth - $('#lightbox').width()) / 2
                    })
                    .fadeIn();
            })
            .appendTo('#lightbox');

        // Remove lightbox on click
        $('#overlay, #lightbox').click(function () {
            $('#overlay, #lightbox').remove();
        });
    });
}
function ShowProgress($btn) {
    $btn.prop('disabled', true);
    var $icon = $btn.find("span");
    var cls = $icon.attr('class');
    $icon.attr('class', 'fa fa-spinner fa-spin');
}
function showIconProgress($icon) {
    $icon.after("<i class='fa fa-spinner fa-spin' title='Please wait...'></i>");
    $icon.remove();
}
function disableEditor() {
    return false;
}
function disableVerified(e) {
    return !e.VerifiedOn;
}
function copyToClipBoard(text) {
    var txt = document.createTextNode(text);
    document.body.appendChild(txt);
    if (document.body.createTextRange) {
        var d = document.body.createTextRange();
        d.moveToElementText(txt);
        d.select();
        document.execCommand('copy');
    } else {
        var d = document.createRange();
        d.selectNodeContents(txt);
        window.getSelection().removeAllRanges();
        window.getSelection().addRange(d);
        document.execCommand('copy');
        window.getSelection().removeAllRanges();
    }
    txt.remove();
}
function toCurrency(amount) {
    return kendo.toString(amount, "n2");
}
function formatFileSize(size) {
    var i = Math.floor(Math.log(size) / Math.log(1024));
    return (size / Math.pow(1024, i)).toFixed(2) * 1 + ' ' + ['bytes', 'kb', 'mb', 'GB', 'TB'][i];
}
function onUploadSelect(e) {
    var fileSize = e.files[0].size;
    var FILE_UPLOAD_SIZE = 10485760;    /* 10 MB */
    if (fileSize > FILE_UPLOAD_SIZE) {
        kendoAlert("File size (" + formatFileSize(fileSize) + ") cannot exceed the maximum limit " + formatFileSize(FILE_UPLOAD_SIZE), true);
        e.files[0].name = "";
        e.files[0].rawFile = null;
        setTimeout(function () { e.sender.removeAllFiles(); }, 100);
    }
}
function getUniqueArray(array) {
    return $.grep(array, function (el, index) {
        return index === $.inArray(el, array);
    });
}
function getClassByFile(fileName) {
    var ext = fileName.split('.').pop().toLowerCase();
    switch (ext) {
        case "png":
        case "jpeg":
        case "jpg":
        case "bmp":
        case "gif":
            return "fa-picture-o";
        case "rar":
        case "zip":
            return "fa-file-archive-o";
        case "doc":
        case "docx":
            return "fa-file-word-o";
        case "xls":
        case "xlsx":
            return "fa-file-excel-o";
        case "pdf":
            return "fa-file-pdf-o";
        default:
            return "fa-file-text-o";
    }
}
function removeRowError(grid) {
    $('#' + grid ? grid : 'grid').find(".rw-err").removeClass("rw-err");
}
function setRowError(uid, grid) {
    $('#' + grid ? grid : 'grid').find("[data-uid=" + uid + "]:not('.rw-err')").addClass("rw-err");
}
function compareObj(obj1, obj2) {
    obj1 = obj1 ? obj1 : "";
    obj2 = obj2 ? obj2 : "";
    return obj1.toLowerCase().indexOf(obj2.toLowerCase()) >= 0;
}
function toCurrency(amount) {
    return kendo.toString(amount, "n2");
}
function convertCurrency(amount, fromCurrency, toCurrency, fromToUSDConversionRate, usdtoToConversionRate) {
    return parseFloat((amount == 0 || fromCurrency == toCurrency ? amount :
        (amount / fromToUSDConversionRate) * usdtoToConversionRate).toFixed(2)
    );
}
function currencyConversion(amount, toCurrency, usdtoToConversionRate) {
    return convertCurrency(amount, BASE_CURRENCY, toCurrency, CONVERSION_RATE, usdtoToConversionRate);
}
function convertToBaseCurrency(amount, fromCurrency, fromToUSDConversionRate) {
    return convertCurrency(amount, fromCurrency, BASE_CURRENCY, fromToUSDConversionRate, CONVERSION_RATE);
}
function removeDoubleLables(value) {
    var text = value;

    if (value % 1 !== 0) {
        text = "";
    }

    return text;
}
function exportChart(chartName) {
    var chart = $("#" + chartName).getKendoChart();
    chart.exportPDF({ paperSize: "auto", margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" } }).done(function (data) {
        //window.open(data);
        var iframe = "<iframe width='100%' height='100%' src='" + data + "'></iframe>"
        var x = window.open();
        x.document.open();
        x.document.write(iframe);
        x.document.close();
    });
}
function grid_Search(gridID, txtID) {
    try {
        var grid = $('#' + (gridID ? gridID : 'grid')).getKendoGrid();
        var dataSource = grid.dataSource;
        if (!dataSource) return;
        var text = $('#' + (txtID ? txtID : 'txtSearch')).val();
        var columns = grid.options.columns;
        var arr = [];
        if (!text || text == '') {
            grid.dataSource.filter({});
            return;
        }
        $(columns).each(function () {
            if (this.field == "")
                return true;
            var type = dataSource.options.schema.model.fields[this.field].type;
            if (this.field == "" || type != "string")
                return true;
            arr.push({ field: this.field, operator: "contains", value: text });
        });
        dataSource.filter({ logic: "or", filters: arr });
    } catch (e) {
        console.log(e);
    }
}
function oneClickPrint(url, data, callBack) {
    data.Ver = BSOL_PRINTER_VERSION;
    $.ajax({
        url: url,
        method: "post",
        data: data,
        success: function (res) {
            if (res == "ERROR") {
                location.reload();
                return;
            }
            callBack(res);
        },
        error: function (xhr, error) {
            if (callBack)
                callBack();
            kendoAlert("Print server is not running."
                + "\nDownload and Install Print Server if it is not installed."
                + "\nClick <a href='/downloads/BSOLPrinter.msi' class='c-lnk' target='_blank'>here</a> to download Printer Server"
                + "\nClick <a href='/downloads/CRRuntime_32bit_13_0_12.msi' class='c-lnk' target='_blank'>here</a> to download Dependencies", true);
        }
    });
}
function validatePassword(password) {
    var policy = { LowerCase: 1, UpperCase: 1, SpecialChars: 1, Numbers: 1, MinimumLength: 6 };
    var rules = [];

    if (policy.LowerCase > 0)
        rules.push({ Regex: '(?=[a-z]{' + policy.LowerCase + '})', Message: 'Password must contains minimum ' + policy.LowerCase + ' lower case alphabet(s)' });

    if (policy.UpperCase > 0)
        rules.push({ Regex: '(?=[A-Z]{' + policy.UpperCase + '})', Message: 'Password must contains minimum ' + policy.UpperCase + ' upper case alphabet(s)' });

    if (policy.Numbers > 0)
        rules.push({ Regex: '(?=[0-9]{' + policy.Numbers + '})', Message: 'Password must contains minimum ' + policy.Numbers + ' number(s)' });

    if (policy.SpecialChars > 0)
        rules.push({ Regex: '(?=[!,%,&,@,#,$,^,*,?,_,~]{' + policy.SpecialChars + '})', Message: 'Password must contains minimum ' + policy.SpecialChars + ' special character(s) !%&@#$^*?_~,' });

    if (policy.MinimumLength > 0)
        rules.push({ Regex: '.{' + policy.MinimumLength + ',}', Message: 'Password length must be ' + policy.MinimumLength });

    var isValid = true;
    var html = "<ul class='pwd-rules'>"
    $.each(rules, function (i, rule) {
        var isPassed = new RegExp(rule.Regex, 'g').test(password);
        html += "<li class='" + (isPassed ? "passed" : "failed") + "'><i class='fa " + (isPassed ? "fa-check" : "fa-times") + "'></i> " + rule.Message + "</li>";
        if (!isPassed)
            isValid = false;
    });
    html += "</ul>";
    return { HasError: !isValid, Html: html };
}
function getDocIcon(ext) {
    ext = ext ? ext.toLowerCase() : "";
    if (ext.endsWith("docx") || ext.endsWith("doc"))
        return "fa fa-file-word-o";
    else if (ext.endsWith("pdf"))
        return "fa fa-file-pdf-o";
    else if (ext.endsWith("xls") || ext.endsWith("xlsx"))
        return "fa fa-file-excel-o";
    else if (ext.endsWith("jpg") || ext.endsWith("jpeg") || ext.endsWith("png") || ext.endsWith("bmp") || ext.endsWith("gif"))
        return "fa fa-file-image-o";
    else if (ext.endsWith("zip") || ext.endsWith("rar"))
        return "fa fa-file-archive-o";
    else
        return "fa fa-file-o";
}


//NEW DESIGN

//SELECT ALL REQUIRED ELEMENTS
const USER_CLOSE = false;
const menuSearchInput = document.querySelector('.menu-search');



const scrollbarVisible = (element) => {
    return element.scrollHeight > element.clientHeight;
};

const toggleDropDown = (dropDown, dropDownIcon, state) => {
    if (state == 'hide') {
        dropDown.style.height = `0px`;
        dropDown.classList.remove('active-dropdown');

        dropDownIcon.style.rotate = '0deg';
        dropDownIcon.classList.remove('active-dropdown-icon');

    } else {
        dropDown.classList.add('active-dropdown');
        dropDown.style.height = `${dropDown.scrollHeight}px`;
        dropDownIcon.style.rotate = '180deg';
        dropDownIcon.classList.add('active-dropdown-icon');
    }
};

const slide = (elem) => {
    const dropDown = elem;
    const dropDownActive = dropDown.classList.contains('active-dropdown');
    const dropDownIcon = document.querySelector(".top-header-profile-drop");

    if (dropDownActive == false) {
        toggleDropDown(dropDown, dropDownIcon, 'show');

        document.addEventListener('click', (e) => {
            if (!dropDown.contains(e.target)) {
                toggleDropDown(dropDown, dropDownIcon, 'hide');
            }
        });

    } else {
        toggleDropDown(dropDown, dropDownIcon, 'hide');
    }

};


document.querySelector(".top-header-profile-drop").addEventListener("click", (e) => {
    e.stopPropagation();
    slide(document.querySelector(".profile-dropdown"));
});




const toggleAppsDropdown = (dropDown, dropDownIcon) => {
    const btn = document.querySelector(".apps-btn");



    document.addEventListener('click', (e) => {
        if (!dropDown.contains(e.target)) {
            dropDown.style.display = 'none';
            btn.classList.remove('sidebar-apps-btn-active');
            menuSearchInput.value = '';
            searchForMenu();
        }
    });

    if (btn.classList.contains('sidebar-apps-btn-active')) {
        dropDown.style.display = 'none';
        btn.classList.remove('sidebar-apps-btn-active');

    } else {
        setTimeout(() => menuSearchInput.focus(), 0);
        dropDown.style.display = 'block';
        btn.classList.add('sidebar-apps-btn-active');

    }

};




document.querySelector(".apps-btn").addEventListener("click", (e) => {
    e.stopPropagation();
    toggleAppsDropdown(document.querySelector(".apps-dropdown"), document.querySelector(".apps-btn"));
});

document.querySelector(".sidebar-logo-holder").addEventListener("click", (e) => {
    e.stopPropagation();
    toggleAppsDropdown(document.querySelector(".apps-dropdown"), document.querySelector(".apps-btn"));
});



/*SIDE BAR CODE */


const sidebar = document.querySelector(".sidebar");
const closeBtn = document.querySelector(".toggle-sidebar");
const searchBtn = document.querySelector(".bx-search");
const navLogoName = document.querySelector(".logo_name");
const navLogoHolder = document.querySelector('.sidebar-logo-holder');

closeBtn.addEventListener("click", () => {
    sidebar.classList.toggle("open");
    menuBtnChange();//calling the function(optional)
    adjustTopHeaderWidth();
});



// following are the code to change sidebar button(optional)
const menuBtnChange = () => {
    if (sidebar.classList.contains("open")) {
        closeBtn.classList.remove('closed', 'active-sidebar-btn');
        navLogoName.style.display = '';
        navLogoHolder.style.display = '';
        $('body').addClass('sm-open');
    } else {
        //Closed sidebar
        closeBtn.classList.add('closed', 'active-sidebar-btn');
        navLogoName.style.display = 'none';
        navLogoHolder.style.display = 'none';
        $('body').removeClass('sm-open');
    }
}






//TOOLTIPS WHEN SIDEBAR IS CLOSED HALF
document.querySelectorAll('.sidebar .nav-list li').forEach((e) => {
    e.addEventListener('mouseover', (event) => {
        const isOpen = sidebar.classList.contains('open');
        if (isOpen) return;
        const li = e.getBoundingClientRect();
        const toolTip = e.querySelector('.tooltip-holder .tooltip');
        toolTip.style.position = 'fixed';
        toolTip.style.top = `${li.top + (li.height / 2)}px`;
    });
});





/*CHECKING FOR SIDEBAR SCROLL AND APPLY PADDING*/
//NOT USING THE CODE RIGHT NOW
/*
const nav = document.querySelector('.nav-list');
let isScrollVisible = scrollbarVisible(nav);

if (isScrollVisible) {
    const sidebar = document.querySelector('.sidebar');
    //sidebar.classList.add('scroll');
}*/


/*SEARCH APPS FUNCTION*/

const searchForMenu = (e) => {
    const filter = menuSearchInput.value.toLowerCase();

    if (filter.trim().length > 0) {

        document.querySelector('.apps-dropdown-nav').style.display = 'none';
        document.querySelector('.apps-dropdown-searchul-wrapper').style.display = 'block';

        document.querySelectorAll(".apps-dropdown-searchul-wrapper li").forEach(function (li) {
            if (filter == "") {
                li.style["display"] = "list-item";
            } else if (!li.textContent.toLowerCase().match(filter)) {
                li.style["display"] = "none";
            } else {
                li.style["display"] = "list-item";
            }
        });
    } else {
        document.querySelector('.apps-dropdown-nav').style.display = 'grid';
        document.querySelector('.apps-dropdown-searchul-wrapper').style.display = 'none';
    }
};


menuSearchInput.addEventListener('keyup', searchForMenu);

/*END SEARCH APPS FUNCTION*/




/*SIDE BAR ADJUSTMENTS*/

const sidebarBackClose = (sidebarToggleBtn, sidebar, e) => {
    if (window.innerWidth <= 768) {
        if (sidebarToggleBtn.contains(e.target) == false && sidebar.contains(e.target) == false) {
            if (sidebar.classList.contains('open')) {
                sidebar.classList.remove("open");
                menuBtnChange();
            }
        }
    }
};

//FUNCTION TO OPEN AND CLOSE SIDEBAR DEPENDING ON THE SIZE
const adjustSideBar = (userClose = false) => {
    //TO DO
    //1 - IF USER HIDES SIDEBAR WE DO NOT NEED TO OPEN IT ON RESIZE

    const sidebar = document.querySelector('.sidebar');
    const sidebarToggleBtn = closeBtn;


    if (window.innerWidth > 1000) {
        sidebar.classList.add("open");


    } else {
        sidebar.classList.remove("open");

        //For closing the apps dropdown on sidebar close -- make function
        const btn = document.querySelector(".apps-btn");
        const dropDown = document.querySelector(".apps-dropdown")
        dropDown.style.display = 'none';
        btn.classList.remove('sidebar-apps-btn-active');
    }

    if (window.innerWidth <= 768) {

        document.addEventListener('click', sidebarBackClose.bind(event, sidebarToggleBtn, sidebar));
    } else {
        document.removeEventListener('click', sidebarBackClose);
    }

    adjustTopHeaderWidth();
    menuBtnChange();

};

const adjustTopHeaderWidth = () => {
    const isOpen = sidebar.classList.contains('open');
    const topHeader = document.querySelector('.top-header');

    if (window.innerWidth > 1000) {
        if (isOpen) {
            topHeader.style.left = '250px';
            topHeader.style.width = 'calc(100% - 250px)';
        } else {
            topHeader.style.left = '50px';
            topHeader.style.width = 'calc(100% - 50px)';
        }

    } else if (window.innerWidth < 1000 && window.innerWidth > 768) {
        if (!isOpen) {
            topHeader.style.left = '50px';
            topHeader.style.width = 'calc(100% - 50px)';
        } else {
            topHeader.style.left = '250px';
            topHeader.style.width = 'calc(100% - 250px)';
        }
    } else if (window.innerWidth <= 768) {
        topHeader.style.left = '0px';
        topHeader.style.width = '100%';
    }

    adjustTopHeader();
};



const adjustTopHeader = () => {

    //LITTLE BUGGY -- 
    const header = document.querySelector('.top-header');
    const headerWidth = header.offsetWidth;
    const root = document.documentElement;
    const headerRight = document.querySelector('.top-header-right').offsetWidth;
    const appsHolderBtn = document.querySelector('.top-header-apps-holder').offsetWidth;
    const diff = (window.innerWidth > 500) ? 40 : 10;

    const maxWidth = headerWidth - headerRight - appsHolderBtn - diff;
    root.style.setProperty('--topHeaderWidth', `${maxWidth}px`);


};





//FUNCTION TO REMOVE ALL PRELOAD TRANSITIONS
const removePreloadTransitions = () => {
    const allPreloads = document.querySelectorAll('.preload');

    allPreloads.forEach((element) => {
        setTimeout(() => {
            element.classList.remove('preload');
        }, 0);
    });
};





//TOP HEADER SLIDE
const options = document.querySelector(".header-apps-wrapper"),
    allOption = options.querySelectorAll(".header-app-item"),
    arrowIcons = document.querySelectorAll(".header-apps-icon span");

let isDragging = false, prevTouch, prevScroll;

const disabledKeys = ["u", "I"];

const handleIcons = (scrollVal) => {
    if (window.innerWidth <= 500) return;
    let maxScrollableWidth = options.scrollWidth - options.clientWidth;
    arrowIcons[0].parentElement.style.display = scrollVal <= 0 ? "none" : "flex";
    arrowIcons[1].parentElement.style.display = maxScrollableWidth > scrollVal ? "flex" : "none";
}



allOption.forEach(option => {
    option.addEventListener("click", () => {
        options.querySelector(".active").classList.remove("active");
        option.classList.add("active");
    });
});

const dragStart = (e) => {
    isDragging = true
    prevTouch = e.pageX || e.touches[0].pageX;
    prevScroll = options.scrollLeft;
};

const dragging = e => {
    if (!isDragging) return;
    options.classList.add("dragging");
    options.scrollLeft = prevScroll - ((e.pageX || e.touches[0].pageX) - prevTouch);

}

const stopDragging = () => {
    isDragging = false;
    options.classList.remove("dragging");
}




options.addEventListener("mousedown", dragStart);
options.addEventListener("touchstart", dragStart);
options.addEventListener("mousemove", dragging);
options.addEventListener("touchmove", dragging);
document.addEventListener("mouseup", stopDragging);
options.addEventListener("touchend", stopDragging);








document.addEventListener("DOMContentLoaded", () => {
    //WHEN THE DOCUMENT IS LOADED CALL THE PRELOAD FUNCTION TO REMOVE THE PRELOAD CLASS
    removePreloadTransitions();
    //ADJUST THE TOP HEADER WIDTH ON PAGE LOAD
    adjustTopHeader();

    //WHEN PAGE LOAD ADJUST SIDEBAR
    adjustSideBar();

    //SET THE SLIDE ICON ON PAGE LOAD
    menuBtnChange();

    //ADJUST SIDEBAR ON WINDOW RESIZE
    window.addEventListener('resize', adjustSideBar);
    //ADJUST TOP HEADER WIDTH ON RESIZE
    document.querySelector('.top-header').addEventListener('transitionend', adjustTopHeader);
});

function showModal(id) {
    /*$('#' + (id ? id : 'editor')).modal('show');*/
    const myModal = new bootstrap.Modal('#' + (id ? id : 'editor'), {
        keyboard: true,
        backdrop: 'static',
        focus: false
    });
    myModal.show();
}
function hideModal(id) {
    $('#' + (id ? id : 'editor')).modal('hide');
}
function fixedGridColumn(status) {
    if (status != "Verified")
        return;

    var grid = $("#grid").data("kendoGrid");

    var headerCell = grid.thead.find("[data-field='PaymentRequest']");

    var columnIndex = headerCell.index();

    var contentCells = grid.tbody.find("tr").find("td:eq(" + columnIndex + ")");

    headerCell.find(".k-link").css("color", "gray");

    contentCells.css({
        "position": "sticky",
        "right": "0",
        "z-index": "2",
        "background-color": "#1b264f",
        "color": "white",
        "cursor": "pointer",

    });

    headerCell.css({
        "position": "sticky",
        "right": "0",
        "z-index": "2",
        "background-color": "white",
    });

    grid.tbody.find(".k-grid-cell").css("border-right", "none");
}
function onWindowClose(e) {
    $('.header-default').css('opacity', '1');
    $('body').css('background-color', '');
}
function onWindowOpen() {
    $('.header-default').css('opacity', '0.3');
    $('body').css('background-color', '#424759');  //f5f2ff//#eff3ff
}


function copyToObject(copyFrom, copyTo, keysToIgnore) {
    for (var key in copyFrom) {
        if (keysToIgnore && $.grep(keysToIgnore, function (x) { return x == key; }).length)
            continue;
        if (copyTo.hasOwnProperty(key)) {
            if (copyFrom[key] && copyFrom[key] instanceof Object && copyTo[key])
                copyToObject(copyFrom[key], copyTo[key], keysToIgnore);
            else
                copyTo[key] = copyFrom[key];
        }
    }
}






