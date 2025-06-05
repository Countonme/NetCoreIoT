layui.define(["jquery"], function (exports) {
    var $ = layui.$;

    var requestUtil = {
        /**
         * 发送请求
         * @param options
         */
        sendRequest: function (options) {
            const { url, method = 'GET', data, done, error } = options;

            $.ajax({
                url: url,
                type: method,
                data: method === 'GET' ? data : JSON.stringify(data),
                contentType: 'application/json',
                headers: {
                    "token": localStorage.getItem("token"),
                    'Accept-Language': localStorage.getItem('site_language') || 'zh'
                },
                crossDomain: true,
                success: function (res) {
                    if (typeof done === 'function') {
                        done(res);
                    }
                },
                error: function (xhr, status, err) {
                    if (typeof error === 'function') {
                        error(xhr, status, err);
                    }
                }
            });
        }
    };

    exports("request", requestUtil);
});