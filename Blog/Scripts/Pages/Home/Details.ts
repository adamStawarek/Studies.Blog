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

function createCommentTemplate(id: number) {
    var json = this.getPostComments(id);
    var comments:PostComment[]=new Array();
    $.extend(comments, json);
    console.log(comments);
    var html: string = '';
    comments.forEach((c) => {
        html += '<div class="row comment">' +
            '<div class="col-sm-1">' +
                '<div class="thumbnail">' +
                    '<img class="img-responsive" src="https://ssl.gstatic.com/accounts/ui/avatar_2x.png">'+
                '</div>'+
            '</div>'+
            '<div class="col-sm-8">' +
                '<div class="panel panel-default">' +
                    '<div class="panel-heading">' +
                        '<strong style="margin-right: 10px;">'+c.authorName+'</strong>' +
                        '<span class="text-muted">'+c.creationTime.toLocaleString()+'</span>'+
                    '</div>' +
                    '<div class="panel-body">'+c.content+'</div>'+
                '</div>'+
            '</div>'+
            '</div>';
    });
    $("#commentSection").append(html);
}