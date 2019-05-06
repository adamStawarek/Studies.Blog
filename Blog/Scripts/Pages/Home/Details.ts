class PostComment {

    public id: number;

    public content: string;

    public postId: number;

    public authorName: string;

    public authorId: number;

    public creationTime: Date;

    public lastEditTime: Date;

    public state: number;
}

window.onload = () => {
    var baseUrl = document.location.origin; 

    $("#commentForm").submit(e => {
        e.preventDefault();
        var $form = $("#commentForm");
        var formData = getFormData($form);
        console.log(formData);
        $.ajax({
            type: 'post',
            url:baseUrl+'/home/addComment',
            data: formData,          
            dataType: 'json',
            success(data) {
                var comment = new PostComment();
                $.extend(comment, data);
                console.log(data);
                var html = createCommentTemplate(comment);
                $("#commentSection").append(html);
                setUpCommentsSectionTitle();
            },
            error(data) {
                console.log(data);
            }
        });
        return false;
    });

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

function formatDate(d: string): string {
    var date = new Date(d);
    return date.toLocaleString();
}

function setUpPostComments(id: number) {
    var json = this.getPostComments(id);
    var comments:PostComment[]=new Array();
    $.extend(comments, json);
    console.log(comments);
    var html: string = '';
    comments.forEach((c) => {
        html += createCommentTemplate(c);
    });
    $("#commentSection").append(html);
    setUpCommentsSectionTitle();
}

function createCommentTemplate(comment: PostComment) {
    var html = '<div class="row comment">' +
        '<div class="col-sm-1">' +
        '<div class="thumbnail">' +
        '<img class="img-responsive" src="https://ssl.gstatic.com/accounts/ui/avatar_2x.png">' +
        '</div>' +
        '</div>' +
        '<div class="col-sm-8">' +
        '<div class="panel panel-default">' +
        '<div class="panel-heading">' +
        '<strong style="margin-right: 10px;">' + comment.authorName + '</strong>' +
        '<span class="text-muted">' + formatDate(comment.creationTime.toString()) + '</span>' +
        '</div>' +
        '<div class="panel-body">' + comment.content + '</div>' +
        '</div>' +
        '</div>' +
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