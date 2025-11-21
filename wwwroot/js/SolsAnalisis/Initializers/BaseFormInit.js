
import { formHandlers } from '../Handlers/form-handlers.js';
import { BaseFormService } from '../Services/BaseFormService.js';

/** @typedef { import("../types/form.types.js").DOMElements } */

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

        this.elements.tableElem
            .addEventListener('click', this.handlers.handleTableRowClick);

        document.getElementById('btnAddDetail')
            .addEventListener('click', this.handlers.handleAddRowClick);
    }

}