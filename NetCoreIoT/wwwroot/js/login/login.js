layui.use(['form', 'layer', 'request'], function () {
    var form = layui.form,
        request = layui.request,
        layer = layui.layer;

    debugger
    // 背景轮播图
    const bgImages = [
        '../../../images/login/login-bg1.jpg',
        '../../../images/login/login-bg2.jpg',
        '../../../images/login/login-bg3.jpg'
    ];
    let bgIndex = 0;
    const bgEl = document.getElementById('bg-slideshow');
    function updateBackground() {
        bgEl.style.backgroundImage = `url(${bgImages[bgIndex]})`;
        bgIndex = (bgIndex + 1) % bgImages.length;
    }
    updateBackground();
    setInterval(updateBackground, 8000);

    // 粒子初始化
    particlesJS('particles-js', {
        particles: {
            number: { value: 60 },
            color: { value: "#00c8ff" },
            shape: { type: "circle" },
            opacity: { value: 0.5 },
            size: { value: 3 },
            line_linked: {
                enable: true,
                distance: 120,
                color: "#00c8ff",
                opacity: 0.4,
                width: 1
            },
            move: { enable: true, speed: 2 }
        },
        interactivity: {
            detect_on: "canvas",
            events: { onhover: { enable: true, mode: "grab" } },
            modes: { grab: { distance: 140, line_linked: { opacity: 1 } } }
        },
        retina_detect: true
    });

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