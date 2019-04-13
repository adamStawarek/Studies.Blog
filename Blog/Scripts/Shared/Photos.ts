import Src = require("./Src");
import ISrc = Src.Src;

export interface Photos {
    width: number;
    height: number;
    url: string;
    photographer: string;
    src: ISrc;
}