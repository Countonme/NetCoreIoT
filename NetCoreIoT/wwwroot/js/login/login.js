layui.use(['form', 'layer', 'request'], function () {
    var form = layui.form,
        request = layui.request,
        layer = layui.layer;

    form.on('submit(submitLogin)', function (data) {
        var field = data.field;
        //请求登入接口
        request.sendRequest({
            url: 'login'
            , method: 'post'
            , data: field
            , crossDomain: true  //允许跨域
            , contentType: 'application/json' // 设置请求的 Content-Type
            , done: function (res) {
                debugger
                if (res.code == 0) {
                    if (res.data.token == null) {
                        layer.alert(res.message, {
                            icon: 2,
                            skin: 'class-layer-skin-neon',
                            title: "Error"
                        });
                        return;
                    }
                    //登入成功的提示与跳转
                    layer.msg('login_success', {
                        offset: '15px',
                        icon: 1,
                        time: 1000
                    }, function () {
                       
                        localStorage.setItem("token", res.data.token);
                        localStorage.setItem("username", res.data.username);
                        document.cookie = "token=" + res.data.token + ";path=/";
                        window.location.href = "../../../home";
                    });
                } else {
                    layer.alert(res.message, {
                        icon: 2, skin: 'class-layer-skin-neon', title: "Error"
                    });
                    return
                }
            }
            , error: function (res) {
                layer.alert("服务器错误", {
                    icon: 2,
                    skin: 'layer-ext-demo',
                    title: "Error"
                });
                return
            }
        });
        return false;
    });
});