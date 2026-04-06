
import { moveFocusToNext } from '../../helpers/form.helpers.js';

/** @typedef { import("../types/form.types.js").DOMElements } */

/**
 * Factory que crea los handlers para el formulario de Solicitud de Análisis
 * @param {import("../Services/BaseFormService.js").BaseFormService} service - Service del formulario
 * @param {DOMElements} formElems
 */
export const formHandlers = (service, formElems) => {
	const handleInputKeydown = async (e) => {
		if (!keyIsTab(e.key) && !keyIsEnter(e.key)) return;

		e.preventDefault();
		const input = e.target;

		const scope = getEventTargetScope(input, FORM_SCOPE);

		//Se verifica si el input tiene un rscName para abrir el lov
		if (keyIsTab(e.key) && input.dataset.rscName) {
			let lovParams = {
				input: input,
				nextFocusScope: scope
			};

			try {
				await service.showLovFromTable(lovParams);
			}
			catch (error) {
				throw error;
			}
		}
		else {
			moveFocusToNext(scope, input);
		}
	}

	const handleInput = async (e) => {
		service.clearInputLovTargets(e.target);
	}

	const handleTableInputKeydown = async (e) => {
		if (!keyIsTab(e.key) && !keyIsEnter(e.key)) return;

		const input = e.target;
		const trScope = getTableRowScope(input);

		if (!trScope) return;

		e.preventDefault();

		if (keyIsTab(e.key) && input.dataset.rscName) {
			let lovParams = {
				input: input,
				nextFocusScope: trScope
			};

			try {
				await service.showLovFromTable(lovParams);
			}
			catch (error) {
				throw error;
			}
		}
		else {
			moveFocusToNext(trScope, input);
		}
	};

	const handleTableRowClick = (e) => {
		//Verificamos si el elemento es un botón
		const button = e.target.closest('button');
		if (button) {
			const tr = getTableRowScope(button);

			if (tr) {
				const action = button.dataset.action;

				if (!action) return;

				switch (action) {
					case 'delete':
						service.removeTableRow(tr);
						break;
				}
			}
		}
	};

	const handleAddRowClick = (e) => {
		e.preventDefault();
		service.addNewDetRow(formElems);
	};

	const handleSubmit = (e) => {
		e.preventDefault();

		
	}

	return {
		handleAddRowClick,
		handleTableRowClick,
		handleTableInputKeydown,
		handleInput,
		handleAddRowClick,
		handleInputKeydown,
		handleSubmit
	};
}


