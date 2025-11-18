import { InputMask } from '../../Utils/InputMask.js';

class BaseFormInit {

    constructor(formElem) {
        this.elements = {
            form = formElem,
            buscadorClienteInp = formElem.querySelector('[id="docClientInput"]'),
            tablaDets = formElem.querySelector('[id="tablaDetalles"]'),
            templateFilaDet = document.getElementById('templateFilaDet')
        };

        
    }

    /*
 *	MÁSCARAS DE CARACTÉRES PARA LOS INPUTS DE LA TABLA DE DETALLES 
 */
    applyTableInputMasks(tr) {
        tr.querySelectorAll('input.alphabet-upper')
            .forEach(x => InputMask.alphabetUpperMask(x)); //Máscara solo letras

        tr.querySelectorAll('input.garantia-val')
            .forEach(x => InputMask.decimalNumberMask(x, 7, 3));

        tr.querySelectorAll('input.solubl-val')
            .forEach(x => InputMask.onlyNumberMask(x, 3));
    }

    dropTableInputMasks(tr) {
        tr.querySelectorAll('input.alphabet-upper')
            .forEach(InputMask.removeIMask);

        tr.querySelectorAll('input.garantia-val')
            .forEach(InputMask.removeIMask);

        tr.querySelectorAll('input.solubl-val')
            .forEach(InputMask.removeIMask);
    }

    initEventListeners() {
        document.getElementById('btnAddDetail')
            .addEventListener('click', (e) => {
                e.preventDefault();
                addNewDetRow();
            });

        buscadorCliente.addEventListener('keydown', e => handleInputKeydown(e,
            (error) => {
                if (error instanceof RscNotFoundErr) {
                    inputSearchErrorHandler(error);
                }
                else {
                    throw error;
                }
            }));

        buscadorCliente.addEventListener('input', e => clearInputLovTargets(e.target));
    }
}