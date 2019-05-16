var currentPage: number = 1;
var numOfComments: number;
var commentsPerPage:number = 5;

window.onload = () => {
    numOfComments = getCommentsCount();
    setUpComments(currentPage);
    createPaginationBox();    
    $("#pagination .btn").click(event => {
        var text = $(event.target).text();
        setUpComments(Number(text));
        $("#pagination .btn").removeClass("btn-primary");
        $("#pagination .btn").addClass("btn-default");
        $(event.target).addClass("btn-primary");
    });
}

function getComments(page: number) {
    var baseUrl = document.location.origin;
    return $.ajax({
        url: baseUrl + "/api/comments/page/"+page,
        type: "GET",
        async: false,
        dataType: "json",
        success(data) {
            currentPage = page;
            return data;           
        }
    }).responseJSON;
}

function getCommentsCount() {
    var baseUrl = document.location.origin;
    return $.ajax({
        url: baseUrl + "/api/comments/count",
        type: "GET",
        async: false,
        dataType: "json",
        success(data) {            
            return data;
        }
    }).responseJSON;
}

function setUpComments(page: number) {
    var json = this.getComments(page);
    var comments: PostComment[] = new Array();
    $.extend(comments, json);
    var html: string = '';
    comments.forEach((c) => {
        html += createCommentTemplateWithAcceptanceActions(c);
    });
    $("#commentSection").html(html);

    setUpTitle();

    $(".panel-heading a").click(event => {
        var text = $(event.target).text().split(' ', 2)[0];
        var baseUrl = document.location.origin;
        window.location.href = baseUrl + "/home/details/" + text;
    });
}

function createCommentTemplateWithAcceptanceActions(comment: PostComment) {
    var html = '<div class="row comment">' +
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
                    '<a class="text-muted">' + comment.postId +" ("+comment.postTitle+")"+ '</a>' +
                '</div>' +
                '<div class="panel-body">' + comment.content + '</div>' +
            '</div>' +
        '</div>' +
        '<di class="col-sm-3">' +
            '<button class="btn btn-primary" style="margin-right: 5px;">Approve</button>' +
            '<button class="btn btn-danger" style="margin-right: 5px;">Reject</button>' +
        '</div>'+
        '</div>';
    return html;
}

function createPaginationBox() {
    var html = '';
    for (var i = 0,j=1; i < numOfComments; i+=commentsPerPage,j++) {
        var btnStateClass = 'btn-default';
        if (j === currentPage)
            btnStateClass = 'btn-primary';
        html += '<button class="btn '+btnStateClass+'" style="margin-right: 5px;">' + j + '</button>';
    }
    $("#pagination").append(html);
}

function setUpTitle() {

    var title = "";
    if (numOfComments === 0) {
        title = "No comments";
    }
    else if (numOfComments === 1) {
        title = "1 comment";
    }
    else {
        title = numOfComments + " comments waiting for acceptance";
    }
    $("#commentsSectionTitle").text(title);
}