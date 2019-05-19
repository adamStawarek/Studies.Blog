window.onload = () => {
    var baseUrl = document.location.origin;
    var user = new User();
    var dbUser = getUserName();
    $.extend(user,dbUser);
    $("#commentForm").submit(e => {
        e.preventDefault();
        var $form = $("#commentForm");
        var formData = getFormData($form);       
        $.ajax({
            type: 'post',
            url:baseUrl+'/home/addComment',
            data: formData,          
            dataType: 'json',
            success(data) {
                $("#commentAfterPostMessage").text("Your message will be visible after review by admin");
            },
            error(data) {
                console.log(data);
            }
        });
        return false;
    });

}

function getUserName() {
    var baseUrl = document.location.origin;
    return $.ajax({
        url: baseUrl + "/api/users/current",
        type: "GET",
        async: false,
        dataType: "json",
        success(data) {
            console.log(data);
            return data;
        }
    }).responseJSON;
}

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        indexed_array[n['name']] = n['value'];
    });

    return indexed_array;
}

function getPostComments(id: number) {
    var baseUrl = document.location.origin;    
    return $.ajax({
        url: baseUrl+"/api/comments/post/" + id,
        type: "GET",
        async: false,
        dataType: "json",
        success(data) {
            return data;
        }
    }).responseJSON;
}

function setUpPostComments(id: number) {
    var postId = id;
    var json = this.getPostComments(id);
    console.log(json);
    var comments:PostComment[]=new Array();
    $.extend(comments, json);
    console.log(comments);
    var html: string = '';
    comments.forEach((c) => {
        html += createCommentTemplate(c);
    });
    $("#commentSection").html(html);

    $(".comment .reject").click(event => {
        var parentDivId = $(event.target).closest(".comment").attr("id");
        var id = parentDivId.split('_')[1];
        var comment = new PostComment();
        var result = rejectComment(Number(id));
        $.extend(comment, result);
        setUpPostComments(postId);
    });

    setUpCommentsSectionTitle();
}

function createCommentTemplate(comment: PostComment) {
    var html = '<div class="row comment" id="comment_' + comment.id +'">' +
        '<div class="col-sm-1">' +
            '<div class="thumbnail">' +
                '<img class="img-responsive" src="https://ssl.gstatic.com/accounts/ui/avatar_2x.png">' +
            '</div>' +
        '</div>' +
        '<div class="col-sm-8">' +
            '<div class="panel panel-default">' +
                '<div class="panel-heading">' +
                    '<strong style="margin-right: 10px;">' + comment.author + '</strong>' +
                    '<span class="text-muted">' + formatDate(comment.creationTime.toString()) + '</span>' +
                '</div>' +
                '<div class="panel-body">' + comment.content + '</div>' +
            '</div>' +
        '</div>' +
        '<button class="btn btn-danger reject" style="margin-right: 5px;">Reject</button>' +
        '</div>';
    return html;
}

function setUpCommentsSectionTitle() {
    var numOfComments = $('.comment').length;
    var title = "";
    if (numOfComments === 0) {
        title = "No comments";
    }
    else if (numOfComments === 1) {
        title = "1 comment";
    }
    else {
        title = numOfComments + " comments";
    }
    $("#commentsSectionTitle").text(title);
}