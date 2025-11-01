import { NoDataErr } from '../Errors/NoDataErr.js';
import { RscNotFoundErr } from '../Errors/RscNotFoundErr.js';
import { LovUI } from '../Lov/LovUI.js';
import { InputMask } from '../Utils/InputMask.js';

const lov = new LovUI();

let indexDet = 0;

const templateFilaDet = document.getElementById('templateFilaDet');
const tablaDets = document.getElementById('tablaDetalles');
const inputAlertClass = 'msg-alert-showing';
let hideInputAlertTimeout;

document.addEventListener("DOMContentLoaded", function () {
	const buscadorCliente = document.getElementById('docClienteInput');

	document.getElementById('btnAddDetail')
		.addEventListener('click', (e) => {
			e.preventDefault();
			addNewDetRow();
		});

	tablaDets.addEventListener('keydown', e => handleTableInputKeydown(e,
		(error) => {
			if (error instanceof RscNotFoundErr || error instanceof NoDataErr) {
				tableSearchErrorHandler(error);
			}
			else {
				throw error;
			}
		}));

	tablaDets.addEventListener('input', e => clearTableInputLovTargets(e.target));

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

	//Aplicamos las máscaras de caractéres a los inputs
	InputMask.documentMask(buscadorCliente);

	const tbody = tablaDets.querySelector('tbody')
	if (tbody) {
		tbody.querySelectorAll('tr').forEach(applyTableInputMasks);

		tbody.addEventListener('click', handleTableRowClick);
	}
});

const handleInputKeydown = async (e, handleErrorFunc) => {
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

		await executeLovFunc(lovParams, handleInputShowLov, handleErrorFunc);
	}
	else {
		moveFocusToNext(scope, input);
	}
};

const handleTableInputKeydown = async (e, handleErrorFunc) => {
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

		await executeLovFunc(lovParams, handleTableShowLov, handleErrorFunc);
	}
	else {
		moveFocusToNext(trScope, input);
	}
};

const hideInputAlert = (input) => {
	input.classList.remove(inputAlertClass);
};

const executeLovFunc = async (lovParams, lovHandlerFunc, errorCallback) => {
	try {
		await lovHandlerFunc(lovParams);
	}
	catch (error) {
		errorCallback(error);
	}
};

const handleInputShowLov = async (lovParams) => {
	//Agregamos la lista de targets al input
	lovParams.targetsFunc = () => lov.getLovInputTargets(lovParams.input);

	await lov.executeLovForInput(lovParams);
};


const handleTableShowLov = async (lovParams) => {
	const tr = lovParams.nextFocusScope;
	//Agregamos la lista de targets al input
	lovParams.targetsFunc = () => lov.getLovInputTargets(lovParams.input, tr);

	await lov.executeLovForInput(lovParams);
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
					handleRemoveTableRow(tr);
					break;
			}
		}
	}
}

const handleRemoveTableRow = (tr) => {
	//Eliminamos las máscaras innecesarias
	dropTableInputMasks(tr);

	const tbody = tr.closest('tbody');

	//Eliminamos la fila del DOM
	removeDetRow(tr); 

	//Reasignamos el orden a los detalles
	
	const rows = tbody?.querySelectorAll('tr') ?? [];
	
	rows.forEach((r, i) => {
		const label = r.querySelector('label.table-numeration');
		if (label) {
			label.innerText = i + 1;
		}
	});
}

const clearInputLovTargets = (input) => {
	if (!input.dataset.rscName) return;

	const lovTargets = lov.getLovInputTargets(input);

	if (lovTargets) {
		lovTargets.forEach(ipt => setInputValue(ipt, ''));
	}
};

const clearTableInputLovTargets = (input) => {
	const tr = getTableRowScope(input);

	if (!tr || !input.dataset.rscName) return;

	const lovTargets = lov.getLovInputTargets(input, tr);

	if (lovTargets) {
		lovTargets.forEach(ipt => setInputValue(ipt, ''));
	}
};

const addNewDetRow = () => {
	let template = templateFilaDet.content.cloneNode(true);

	const tr = template.firstElementChild;

	tr.innerHTML = tr.innerHTML
		.replace(/__index__/g, indexDet)
		.replace(/__orderNum__/g, (indexDet + 1));

	applyTableInputMasks(tr);

	tablaDets.querySelector('tbody').appendChild(tr);
	indexDet++;

	//A la nueva fila agregada, hacemos focus
	moveFocusToNext(tr);
};

const removeDetRow = (tr) => {
	tr.remove();
	indexDet--;
}

/* Manejadores de errores */
const inputSearchErrorHandler = (error) => {
	const inputAlert = document.getElementById('docClienteErr');

	inputAlert.innerText = error.message;

	showInputAlert(inputAlert, 1200);
};

/*
 *	MÁSCARAS DE CARACTÉRES PARA LOS INPUTS DE LA TABLA DE DETALLES 
 */
const applyTableInputMasks = (tr) => {
	tr.querySelectorAll('input.alphabet-upper')
		.forEach(x => InputMask.alphabetUpperMask(x)); //Máscara solo letras

	tr.querySelectorAll('input.garantia-val')
		.forEach(x => InputMask.decimalNumberMask(x, 7, 3));

	tr.querySelectorAll('input.solubl-val')
		.forEach(x => InputMask.onlyNumberMask(x, 3));
}

const dropTableInputMasks = (tr) => {
	tr.querySelectorAll('input.alphabet-upper')
		.forEach(InputMask.removeIMask);

	tr.querySelectorAll('input.garantia-val')
		.forEach(InputMask.removeIMask);

	tr.querySelectorAll('input.solubl-val')
		.forEach(InputMask.removeIMask);
}

const tableSearchErrorHandler = (error) => {
	const alertElem = document.getElementById('tableDetailsError');

	alertElem.innerText = error.message;

	showInputAlert(alertElem, 1500); // 1s y medio
};

const showInputAlert = (alert, time) => {
	alert.classList.add(inputAlertClass);

	clearTimeout(hideInputAlertTimeout); //Se limpia el timer

	hideInputAlertTimeout = setTimeout(() => { hideInputAlert(alert) }, time)
};

/* METODOS PARA USAR DESDE Create.cshtml */

export const showDetailsError = (errorMsgs) => {
	const alertElem = document.getElementById('tableDetailsError');
	alertElem.innerText = errorMsgs;
	showInputAlert(alertElem, 1500);
};

export const setDetRowIndex = (index) => {
	indexDet = index; 
};

window.showDetailsError = showDetailsError;
window.setDetRowIndex = setDetRowIndex;