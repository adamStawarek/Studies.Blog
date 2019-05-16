class PostComment {

    public id: number;

    public content: string;

    public postId: number;

    public postTitle: string;

    public userId: string;

    public creationTime: Date;

    public lastEditTime: Date;

    public state: number;

    public author: string;
}

class User {

    public id: string;

    public name: string;
}


function formatDate(d: string): string {
    var date = new Date(d);
    return date.toLocaleString();
}