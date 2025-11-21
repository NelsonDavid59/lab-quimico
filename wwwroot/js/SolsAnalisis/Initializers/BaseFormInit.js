import { InputMask } from '../../Utils/InputMask.js';
import { formHandlers } from '../Handlers/form-handlers.js';
import { BaseFormService } from '../Services/BaseFormService.js';

/**
 * @typedef {object} DOMElements
 * @property {HTMLFormElement} form Elemento form del Solicitud de Análisis
 * @property {HTMLInputElement} buscadorClienteInp Elemento input Buscador de clientes
 * @property {HTMLTableElement} tableElem Elemento table del formulario (Detalles de Análisis)
 * @property {HTMLTemplateElement} rowTemplateElem Elemento template - Fila para agregarse 
 */


/**
 * Initializer base para los formularios de Solicitudes de Análisis
 * @class
 */
export class BaseFormInit {

    /**
     * @constructor
     * @param {form} formElem - Elemento DOM del Formulario
     */
    constructor(formElem) {
        /**
         * @type {DOMElements}
         */
        this.elements = {
            form: formElem,
            buscadorClienteInp: formElem.querySelector('[id="docClienteInput"]'),
            tableElem: formElem.querySelector('[id="tablaDetalles"]'),
            rowTemplateElem: document.getElementById('templateFilaDet')
        };

        /** @type {BaseFormService} */
        this.service = new BaseFormService();
        this.handlers = this.#initHandlers();
    }

    #initHandlers() {
        return formHandlers(this.service, this.elements);
    }
    
    /**
     * Inicializa las máscaras en las filas de la tabla
     */
    initMaskToTableRows() {
        const tbody = this.elements.tableElem.querySelector('tbody');
        this.service.applyTableBodyInputMasks(tbody);
    }

    /**
     * Inicializa los manejadores de eventos del formulario
     */
    initEventListeners() {
        
        /* BUSCADOR DE CLIENTES */
        this.elements.buscadorClienteInp
            .addEventListener('keydown', this.handlers.handleInputKeydown);

        this.elements.buscadorClienteInp
            .addEventListener('input', this.handlers.handleInput);

        /* TABLA DE DETALLES DEL ANÁLISIS */
        this.elements.tableElem
            .addEventListener('keydown', this.handlers.handleInputKeydown);

        this.elements.tableElem
            .addEventListener('input', this.handlers.handleInput);

        document.getElementById('btnAddDetail')
            .addEventListener('click', this.handlers.handleAddRowClick);
    }

}