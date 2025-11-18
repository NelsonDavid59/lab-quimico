
import { BaseFormService } from '../Services/BaseFormService.js';

const service = new BaseFormService();

export const handleInputKeydown = async (e) => {
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

export const handleTableInputKeydown = async (e, handleErrorFunc) => {
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


export const handleTableRowClick = (e) => {
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

