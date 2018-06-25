$(window).ready(function(){
    $('body').on('click','#LogOut',function(){
        sessionStorage.clear();
    });
});