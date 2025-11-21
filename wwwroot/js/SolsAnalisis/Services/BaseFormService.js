import { LovUI } from '../../Lov/LovUI.js';

/**
 * Clase encargada de la lógica relacionada a la interacción UI 
 * del formulario para Solicitudes de Análisis.
 * @class
 */
export class BaseFormService {
	/**
	 * Crea una instancia del service
	 * @constructor
	 */
    constructor() {
		this.lov = new LovUI();
		this.indexDet = 0;
		this.inputAlertClass = 'msg-alert-showing';
		this.hideInputAlertTimeout;
    }

	clearInputLovTargets(input) {
		if (!input.dataset.rscName) return;

		const lovTargets = this.lov.getLovInputTargets(input);

		if (lovTargets) {
			lovTargets.forEach(ipt => setInputValue(ipt, ''));
		}
	}

	clearTableInputLovTargets(input) {
		const tr = getTableRowScope(input);

		if (!tr || !input.dataset.rscName) return;

		const lovTargets = this.lov.getLovInputTargets(input, tr);

		if (lovTargets) {
			lovTargets.forEach(ipt => setInputValue(ipt, ''));
		}
	}
	/**
	 * Agrega una fila nueva a la tabla especificada
	 * @param {any} rowTemplateElem - Template que debe representar la nueva fila.
	 * @param {any} tableElem -  Elemento <table> que debe agregar el nuevo <tr>
	 * @returns
	 */
	addNewDetRow = ({ rowTemplateElem, tableElem }) => {
		const tr = this.#buildDetRow(rowTemplateElem);

		tableElem.querySelector('tbody').appendChild(tr);

		this.indexDet++;

		this.#applyTableRowInputMasks(tr);

		//A la nueva fila agregada, hacemos focus
		moveFocusToNext(tr);

		//Se devuelve la fila creada
		return tr;
	}

	removeDetRow(tr) {
		tr.remove();
		this.indexDet--;
	}

	hideInputAlert(input) {
		input.classList.remove(this.inputAlertClass);
	}

	showInputAlert(alert, time) {
		alert.classList.add(this.inputAlertClass);

		clearTimeout(this.hideInputAlertTimeout); //Se limpia el timer

		this.hideInputAlertTimeout = setTimeout(() => { this.hideInputAlert(alert) }, time)
	}

	setDetRowIndex(index) {
		indexDet = index;
	}

	showLovFromTable = async (lovParams) => {
		const tr = lovParams.nextFocusScope;
		//Agregamos la lista de targets al input
		lovParams.targetsFunc = () => lov.getLovInputTargets(lovParams.input, tr);

		await lov.executeLovForInput(lovParams);
	};

	showLovFromInput = async (lovParams) => {
		//Agregamos la lista de targets al input
		lovParams.targetsFunc = () => lov.getLovInputTargets(lovParams.input);

		await lov.executeLovForInput(lovParams);
	};

	removeTableRow = (tr) => {
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

	applyTableBodyInputMasks(tbody) {
		if (!tbody) return;

		tbody.querySelectorAll('tr').forEach(this.#applyTableRowInputMasks);
	}

	/*
	 *	MÁSCARAS DE CARACTÉRES PARA LOS INPUTS DE LA TABLA DE DETALLES 
	 */
	#applyTableRowInputMasks(tr) {
		tr.querySelectorAll('input.alphabet-upper')
			.forEach(x => InputMask.alphabetUpperMask(x)); //Máscara solo letras

		tr.querySelectorAll('input.garantia-val')
			.forEach(x => InputMask.decimalNumberMask(x, 7, 3));

		tr.querySelectorAll('input.solubl-val')
			.forEach(x => InputMask.onlyNumberMask(x, 3));
	}

	#dropTableRowInputMasks(tr) {
		tr.querySelectorAll('input.alphabet-upper')
			.forEach(InputMask.removeIMask);

		tr.querySelectorAll('input.garantia-val')
			.forEach(InputMask.removeIMask);

		tr.querySelectorAll('input.solubl-val')
			.forEach(InputMask.removeIMask);
	}

	#buildDetRow(rowTemplate) {
		let content = rowTemplate.content.cloneNode(true);

		const tr = content.firstElementChild;

		tr.innerHTML = tr.innerHTML
			.replace(/__index__/g, this.indexDet)
			.replace(/__orderNum__/g, (this.indexDet + 1));

		return tr;
	}
}