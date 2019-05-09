// rem布局，设计稿宽度1080px，写入尺寸 = 设计尺寸 / 100 rem；
let oHtml = document.querySelector('html');
let htmlWidth = oHtml.getBoundingClientRect().width;
let seeHeight = document.documentElement.clientHeight;
oHtml.style.fontSize = htmlWidth / 1080 * 100 + 'px';


// 页面文字提示
function Alert(txt){
    alertmsg.innerHTML = txt
    alertmsg.classList.remove("hide")
    setTimeout(() => {            
        alertmsg.classList.add("hide")
    }, 1000); 
}

// 禁止输入中文
function check(str){ 
    let temp="" 
    for(let i=0;i<str.length;i++) 
         if(str.charCodeAt(i)>0&&str.charCodeAt(i)<255) 
            temp+=str.charAt(i) 
    return temp 
}

// 清除遍历空对象  ------用不上了，直接可以childen代替
function delFf(elem){
    let elem_child = elem.childNodes;
    for(let n=0; n<elem_child.length;n++){
        if(elem_child[n].nodeName == "#text" && !/\s/.test(elem_child.nodeValue)){
            elem.removeChild(elem_child[n])
        }
    }
}


function changeFormat(obj){
    let data = ""
    for(let key in obj){
        data += `${key}=${obj[key]}&`
    }
    data = data.slice(0,data.length-1)
    return data
}



// 获取用户选择的图片并通过ajax传到后台
function selectImage(ipt_ele, img_ele) {
    if (!ipt_ele.files || !ipt_ele.files[0]) {
        return;
    }
    var reader = new FileReader();
    reader.onload = function (evt) {
        img_ele.src = "imgs/loading.gif"
        //选到图片后将base64字符串传到后台
        let base64 = encodeURIComponent(evt.target.result)
        let _data = {
            Content: base64,
            phoneNumber: getCookie("PhoneId")
        }
        let data = changeFormat(_data)
        let xhr = new XMLHttpRequest()
        xhr.onload = function(){
            if(xhr.readyState!==4) return
            if(200<=xhr.status&&xhr.status<300||xhr.status===304){
                let res = JSON.parse(xhr.responseText)
                if (res.Msg == "success") { // 返回的是一个json对象，如何Msg为success，那么将相应的img控件的src设为对应的url值
                    console.log(img_ele)
                    img_ele.src = res.url;                    
                }
                return true
            }else{
                Alert("网络连接失败！")
            }           
        }
        xhr.open("post","http://47.99.118.16:8004/api/Image/UploadImg",true)
        xhr.setRequestHeader("Content-type", "application/x-www-form-urlencoded")
        xhr.send(data)        
    }
    reader.readAsDataURL(ipt_ele.files[0]);
}

// 复制链接地址--------------由于需兼容苹果，所以用插件去了，弃用了此方法
function copyAddress(text) {
    if (navigator.userAgent.match(/(iPhone|iPod|iPad);?/i)) {    //是苹果
        // Alert("垃圾苹果复制你妈！")
    }else{                                                       //是安卓
        let iptAnd = document.getElementById("input")   // 获取对象
        iptAnd.value = text                             // 修改文本框的内容
        iptAnd.select();                                // 选择对象
        document.execCommand("Copy")                    // 执行浏览器复制命令
        document.activeElement.blur()                   // 禁止复制带来的键盘弹出
        Alert("复制成功！")
    }
}



//跳转登录
function goSign(){
    window.location.href = "sign.html";
};

//返回上一页
function goBack(){
    window.history.back(-1);
}

//返回主页
function goHome(){
    window.location.href = "index.html"
}

// 构建函数---弹窗
function GetAlert(opt){
    if(typeof opt.el !== "string"){
        throw Error("必须写上根节点的id")
    }
    this._el = document.getElementById( opt.el.slice(1) )
    this._h2 = this._el.getElementsByTagName("h2")[0]
    this._p = this._el.getElementsByTagName("p")[0]
    this.btn_left = this._el.getElementsByClassName("btn_left")[0]
    this.btn_right = this._el.getElementsByClassName("btn_right")[0]
    this._alertcontent = this._el.getElementsByClassName("alertcontent")[0]

    this._h2.innerText = opt.h2_txt || "提示"
    this._p.innerHTML = opt.p_txt || "这是默认的提示文本信息"
    this.btn_left.innerText = opt.btn_left || "取消"
    this.btn_right.innerText = opt.btn_right || "确定"

    this.close()
    this._el.onclick = function(){    
        this.close()
    }.bind(this)
    this._alertcontent.addEventListener("click",(e)=>{
        e.stopPropagation()  // 阻止事件冒泡 实现点击_el的子元素不会触发_el的点击事件
    })
}
GetAlert.prototype.open = function(){
    this._el.style.display = "block"
}
GetAlert.prototype.close = function(){
    this._el.style.display = "none"
}

// 获取cookie中 某属性的数据
const getCookie = attr =>{
    const arr = document.cookie.match(new RegExp('\\b' + attr + '=([^;]+)(;|$)'))
    return arr? arr[1]: ""
}
// 设置cookie键值与存在时间(天为单位)并保存
const setCookie = (obj, time = 0) =>{
    const timer = new Date(Date.now() + time *1000*60*60*24).toUTCString()
    for(var key in obj){
        document.cookie = `${key}=${obj[key]};expires=${timer}`
    }
}
// 删除cookie中 某属性的数据
const delCookie = attr =>{
    let obj = {}
    obj[attr] = ""
    setCookie(obj, -1)
}

//获取url中的参数值方法
function getQueryString(parameterName) {
    var reg = new RegExp('(^|&)' + parameterName + '=([^&]*)(&|$)', 'i');
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return null;
}

//-----------------------------------增删改查动态相关函数--------------------------------------------

// 编辑页生成 动态 头部信息
function dynHeader(type,titleMsg,imgMsg){
    tittxtbox.innerHTML = ""
    switch (type){
        case "01":
            tittxtbox.innerHTML =
            `<div class="header_text edit_header">
                <div class="header_h3 textarea" contenteditable="true" >${titleMsg}</div>          
            </div>
            <div class="header_bnr edit_header">
                <div class="top_banner">
                    <img src=${imgMsg} alt="" class="response_img center_img changeImg">
                    <input type="file" class="hide file_ipt" >
                </div>
                <p>点击更换图片，最佳适应尺寸：1020*600px</p>
            </div>`
            break
            
        case "02":
            tittxtbox.innerHTML =
            `<div class="header_text edit_header">
                <div class="header_h3 textarea" contenteditable="true" >${titleMsg}</div>          
            </div>`
            break
        // no default
    }
}

function dynContent(contentarr){
    let len = contentarr.length
    for(let i=0; i<len; i++){
        let {ContentType,Text} = contentarr[i]
        switch (ContentType){
            case 0:
                tittxtbox.innerHTML +=
                `<div class="change_img change">
                    <div class="change_imgbox">
                        <img src=${Text} alt="" class="response_img center_img changeImg">
                        <input type="file" class="hide file_ipt" >
                    </div> 
                    <div class="del"><img src="imgs/del.png" alt="" class="response_img"></div>  
                    <div class="append">+</div>          
                </div>`
                break
            case 1:
                tittxtbox.innerHTML +=
                `<div class="change_tit change">
                    <div class="textarea_h3 textarea" contenteditable="true">${Text}</div>
                    <div class="del "><img src="imgs/del.png" alt="" class="response_img"></div>
                    <div class="append">+</div>
                </div>`
                break
            case 2:
                tittxtbox.innerHTML +=
                `<div class="change_txt change">                
                    <div class="textarea_p textarea" contenteditable="true">${Text}</div>
                    <div class="del"><img src="imgs/del.png" alt="" class="response_img"></div> 
                    <div class="append">+</div>           
                </div> `
                break
            // no default                        
        }
    }
}

// 获取编辑页需存储的 模板头部 固定信息
function getHeaderMsg(){
    let thistype = window.location.href.split("edit")[1].split(".html")[0]
    let editHeader = document.getElementsByClassName("edit_header")
    let content = {}
    switch(thistype){
        case "01": 
            content = {
                editType: thistype,
                //title: editHeader[0].children[0].innerHTML.replace(/<div>/g,"<br/>").replace(/<\/div>/g,""),
                title: editHeader[0].children[0].innerHTML.replace(/<[^>]+>/g,(str)=>{
                    if(str==="<div>"){
                        return "<br>"
                    }else{
                        return ""
                    }
                }),
                msg: editHeader[1].children[0].children[0].src
            }
            break
        case "02": 
            content = {
                editType: thistype,
                //title: editHeader[0].children[0].innerHTML.replace(/<div>/g,"<br/>").replace(/<\/div>/g,""),
                title: editHeader[0].children[0].innerHTML.replace(/<[^>]+>/g,(str)=>{
                    if(str==="<div>"){
                        return "<br>"
                    }else{
                        return ""
                    }
                }),
                msg: ""
            }
            break
        // no default 
    }
    return content
}

// 获取编辑页需存储的 内容 数组
function getContentArr(){
    let allchangeDiv = document.getElementsByClassName("change")
    let content = []            
    for(let i=0,len=allchangeDiv.length; i<len; i++){
        let thisType = allchangeDiv[i].classList[0]
        let contentList
        switch(thisType){
            case "change_img":
                contentList = {
                    type: 0,
                    msg: allchangeDiv[i].children[0].children[0].src
                }                        
                break;
            case "change_tit":
                contentList = {
                    type: 1,
                    //msg: allchangeDiv[i].children[0].innerHTML.replace(/<div>/g,"<br/>").replace(/<\/div>/g,"")
                    msg: allchangeDiv[i].children[0].innerHTML.replace(/<[^>]+>/g,(str)=>{
                        if(str==="<div>"){
                            return "<br>"
                        }else{
                            return ""
                        }
                    })
                }
                console.log(contentList.msg)
                break;
            case "change_txt":
                contentList = {
                    type: 2,
                    //msg: allchangeDiv[i].children[0].innerHTML.replace(/<div>/g,"<br/>").replace(/<\/div>/g,"")
                    // msg: allchangeDiv[i].children[0].innerText
                    msg: allchangeDiv[i].children[0].innerHTML.replace(/<[^>]+>/g,(str)=>{
                        if(str==="<div>"){
                            return "<br>"
                        }else{
                            return ""
                        }
                    })   // 删除粘贴文本的自带样式并保留换行
                }
                console.log(contentList.msg)
                break;
            // no default                    
        }
        content.push(contentList)
        
    }
    return content
}

// 生成外发只读静态 html头部
function htmlHeader(type, title, msg){
    let str = ''            
    switch (type){
        case "01":
            str +=
            `<div class="header_text edit_header">
                <h3 class="change_tit_h3">${title}</h3>        
            </div>
            <div class="header_bnr edit_header">
                <div class="top_banner">
                    <img src=${msg} alt="" class="response_img center_img">
                </div>
            </div>`                    
            break
            
        case "02":
            str +=
            `<div class="header_text edit_header">
                <h3 class="change_tit_h3">${title}</h3>    
            </div>`
            break
        // no default
    }
    return str
}


// 生成外发只读静态 html内容
function htmlContent(contentarr){
    let con_str = '' 
    let len = contentarr.length
    for(let i=0; i<len; i++){
        let {type,msg} = contentarr[i]
        switch (type){
            case 0:
                con_str +=
                `<div class="change_img change">
                    <img src="http://47.99.118.16:8005/ActivityPublic/imgs/lazy.gif" alt="" class="response_img lazy_img" data-src=${msg}>
                </div>`
                break
            case 1:
                con_str +=
                `<div class="change_tit change">
                    <h3 class="change_tit_h3">${msg}</h3>
                </div>`
                break
            case 2:
                con_str +=
                `<div class="change_txt change">
                    <p class="change_txt_p">${msg}</p>
                </div>`
                break
            // no default
        }
    }
    return con_str
}

// 确认发布，保存数据
function saveDyn(){
    let hdyp = getHeaderMsg().editType
    let hdtitle = getHeaderMsg().title
    let hdmsg = getHeaderMsg().msg
    let cotarr = getContentArr()
    let htmlStr = htmlHeader(hdyp, hdtitle, hdmsg) + htmlContent(cotarr)
    // console.log(htmlStr)
    const data = {
        phoneNumber: getCookie("PhoneId"),
        webId: getQueryString("webId"),
        modUrl: window.location.href,
        content: {
            Header: getHeaderMsg(),
            Content: getContentArr(),
            Html: htmlStr,
            IsPublic: false
        }              
    }
    console.log(data)
    fetch('http://47.99.118.16:8004/api/Main/SaveDyn',{
        method: 'post',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(result => result.json())
    .then(res =>{
        console.log(res)
        if (res["Msg"] == 'success') {
            window.location.href = "success.html?successUrl=" + res["Content"];
        }
    })
    .catch(err => {
        console.log(err)
    })

}

// 退出编辑，保存草稿
function saveDraft(savetime){
    const data = {
        phoneNumber: getCookie("PhoneId"),
        webId: getQueryString("webId"),
        modUrl: window.location.href,
        content: {
            Header: getHeaderMsg(),
            Content: getContentArr(),
            Html: tittxtbox.innerHTML,
            IsPublic: false
        }              
    }
    console.log(data)
    fetch('http://47.99.118.16:8004/api/Main/SaveDraft',{
        method: 'post',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(result => result.json())
    .then(res =>{
        console.log(res)
        if (res["Msg"] == 'success' && !savetime) {
            goHome()
        }
    })
    .catch(err => {
        console.log(err)
    })

}

// 删除用户数据 包括已发布动态 和保存的草稿
function delUserdata(web_id, callback){
    const data = {
        webId: web_id,              
    }
    console.log(data)
    // if(!data.webId) return

    fetch('http://47.99.118.16:8004/api/Main/DeleteUserWeb',{
        method: 'post',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(result => result.json())
    .then(res =>{
        console.log(res)
        if (res.Msg == 'success') {            
            console.log("删除成功")

            callback()  // 删除成功后执行回调
        }
    })
    .catch(err => {
        console.log(err)
    })
}

//-----------------发送验证码---------------------------------------------------
function sendCode(phonenum,sendtype){
    const data = {
        phoneNumber: phonenum,
        type: sendtype,              
    }
    console.log(data)
    
    fetch('http://47.99.118.16:8004/api/Login/GetMsgCode',{
        method: 'post',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    })
    .then(result => result.json())
    .then(res =>{
        console.log(res)
        if (res.Msg == 'success') {
            Alert("验证码发送成功！")
        }
    })
    .catch(err => {
        console.log(err)
    })
}

// 确认是否为登录状态，如果不是，直接跳转到登录页
function ifSign(){
    let islogin = getCookie("IsLogin")
    console.log(islogin)
    if(islogin==="true") return
    goSign()
}