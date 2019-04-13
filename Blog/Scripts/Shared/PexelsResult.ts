import Photos = require("./Photos");
import IPhotos = Photos.Photos;

export interface PexelsResult {
    page: number;
    per_page: number;
    total_results: number;
    url: string;
    next_page: string;
    photos?: (IPhotos)[] | null;
}