
/*add by Lee 20150712*/

var lee = {
    common: {}
};

lee.common = {
    getquerystring: function (name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.search);
        if (results == null)
            return "";
        else
            return decodeURIComponent(results[1].replace(/\+/g, " "));
    },
    compareDate: function (d1, d2) {  // 时间比较的方法，如果d1时间比d2时间大，则返回true   
        return Date.parse(d1.replace(/-/g, "/")) > Date.parse(d2.replace(/-/g, "/"))
    }
}


$(function () {

    $(".goback").click(function () { window.history.go(-1); });

})
