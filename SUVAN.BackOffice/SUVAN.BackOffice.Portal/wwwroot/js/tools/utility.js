
/**
 * Ejecución de notificaciones
 * @version 1.0.0
 * @author jacms-c8
 */
var notificacion = new function () {
    "use strict";
    var self = this;

    function _isIE() {
        var isIE11 = navigator.userAgent.indexOf(".NET CLR") > -1;
        var isIE11orLess = isIE11 || navigator.appVersion.indexOf("MSIE") != -1;
        return isIE11orLess;
    }

    function _init(type, title, message) {
        var timeOut = _isIE() ? 12500 : 6500;

        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toastr-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": 300,
            "hideDuration": 1000,
            "timeOut": timeOut,
            "extendedTimeOut": timeOut,
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        toastr[type](message, title);
        //toastr.info(message, title, { timeot: 30000 });

        $(".change-control").on("change", function () {
            if (($(this).val() != "") && ($(this).val() != null)) {
                var padre = $(this).parent();
                $(padre).find(".errorSpan").remove();
                $(padre).find(".text-danger").hide();
                //$(padre).find(".invalid").removeClass("invalid");
                $("#" + $(this)[0].id + "-error").hide();
            }
        });
    }

    self.success = function (message, title) {
        _init("success", title, message);
    }

    self.warning = function (message, title) {
        _init("warning", title, message);
    }

    self.info = function (message, title) {
        _init("info", title, message);
    }

    self.error = function (message, title) {
        _init("error", title, message);
    }

    self.show = function (type, message, title) {
        _init(type, title, message);
    }
}

    (function ($) {
        "use strict"
        $.fn.InitDataTable = function (params) {
            let $element = $(this);
            let _defautls = {
                "info": false,
                'order': [],
                "pageLength": 10,
                "lengthChange": false,
                'columnDefs': null
            };
            let _settings = $.extend(false, _defautls, params);
            let dataTable = $element.DataTable(_settings);
            return dataTable;
        };

    }(jQuery));