var currentPage: number = 1;
var numOfComments: number;
var commentsPerPage:number = 5;
var option: string = "waiting for approval";
var optionDict = {
    "waiting for approval": 0,
    "approved": 1,
    "rejected": 2
}

window.onload = () => {
    numOfComments = getCommentsCount();
    setUpComments(currentPage);
    createPaginationBox(); 

    $('#dropdown').change(() => {
        option = $("#dropdown option:selected").text();
        numOfComments = getCommentsCount();
        setUpComments(1);        
    });
}

function getComments(page: number) {
    var baseUrl = document.location.origin;
    return $.ajax({
        url: baseUrl + "/api/comments/page/" +optionDict[option]+"/"+page,
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
        url: baseUrl + "/api/comments/count/"+optionDict[option],
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

    $(".comment .reject").click(event => {
        var parentDivId = $(event.target).closest(".comment").attr("id");
        var id = parentDivId.split('_')[1];
        var comment = new PostComment();
        var result = rejectComment(Number(id));
        $.extend(comment, result);

        numOfComments = getCommentsCount();
        setUpComments(1);
        createPaginationBox(); 
    });

    $(".comment .approve").click(event => {
        var parentDivId = $(event.target).closest(".comment").attr("id");
        var id = parentDivId.split('_')[1];
        var comment = new PostComment();
        var result = approveComment(Number(id));
        $.extend(comment, result);

        numOfComments = getCommentsCount();
        setUpComments(1);
        createPaginationBox();         
    });

    createPaginationBox(); 
}

interface IConWithColor {
    icon: string;
    color: string;
}

function createCommentTemplateWithAcceptanceActions(comment: PostComment) {
    var icon: IConWithColor = {icon:'',color:''};
    if (comment.state == State.Approved) {
        icon.icon = 'glyphicon-ok';
        icon.color = 'green';
    } else if (comment.state == State.Rejected) {
        icon.icon = 'glyphicon-remove';
        icon.color = 'red';
    } else {
        icon.icon = 'glyphicon-minus';
        icon.color = 'blue';
    }

    var html = '<div class="row comment" id="comment_'+comment.id+'">' +
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
        '<div class="col-sm-3">' +
            '<div style="margin-right: auto; margin-left: auto; width: 160px;">'+
                '<button class="btn btn-primary approve" style="margin-right: 5px;">Approve</button>' +
                '<button class="btn btn-danger reject" style="margin-right: 5px;">Reject</button>' +
            '</div>' +
            '<div style="text-align:center">' +
                '<span style="font-size: 40px;color: '+icon.color+';" class="glyphicon '+icon.icon+'" aria-hidden="true"></span>'+
            '</div>'+
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
    $("#pagination").html(html);

    $("#pagination .btn").click(event => {
        var text = $(event.target).text();
        setUpComments(Number(text));
        $(this).addClass("btn-primary");
    });
}

function setUpTitle() {
    var title = '';
    if (numOfComments === 0) {
        title = "No comments ";
    }
    else if (numOfComments === 1) {
        title = "1 comment ";
    }
    else {
        
        title = numOfComments + " comments ";
    }
    $("#commentsSectionTitle").text(title+option);
}