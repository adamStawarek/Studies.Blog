var selectedTags = [];

window.onload = () => {

    $.each($('.activeTag'), function () {
        selectedTags.push($(this).attr("id"));
    });

    $(".badge").click(function () {
        if ($(this).hasClass("activeTag")) {
            const index = selectedTags.indexOf($(this).attr("id"), 0);
            if (index > -1) {
                selectedTags.splice(index, 1);
            }
            $(this).removeClass("activeTag");
        } else {
            selectedTags.push($(this).attr("id"));
            $(this).addClass("activeTag");
        }
    });   
}


function beforeSubmitTag() {
    
    var url = "Home/SearchByTags?";
    selectedTags.forEach((el) => {
        url+="ids="+el.toString()+"&";
    });
    url = url.substring(0, url.length - 1);
     
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        contentType: "application/json",       
        success: function(data) {
            if (data.ok)
                window.location = data.newurl;
            else
                window.alert(data.message);
        }
    });
}