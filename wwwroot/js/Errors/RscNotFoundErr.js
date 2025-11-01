export class RscNotFoundErr extends Error {
    constructor(msg) {
        super(msg);
        this.name = "RscNotFoundErr"
    }
}