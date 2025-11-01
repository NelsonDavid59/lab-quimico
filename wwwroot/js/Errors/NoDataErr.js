/**
 * Info: Se utiliza para indicar que no existen datos para un tipo de 
 * recurso.
 */
export class NoDataErr extends Error {
    constructor(msg) {
        super(msg);
        this.name = "NoDataErr";
    }
}