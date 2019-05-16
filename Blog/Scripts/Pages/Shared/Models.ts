﻿class PostComment {

    public id: number;

    public content: string;

    public postId: number;

    public postTitle: string;

    public userId: string;

    public creationTime: Date;

    public lastEditTime: Date;

    public state: State;

    public author: string;
}

enum State {

    WaitingForApproval = 0,

    Approved = 1,

    Rejected = 2
}

class User {

    public id: string;

    public name: string;
}


function formatDate(d: string): string {
    var date = new Date(d);
    return date.toLocaleString();
}