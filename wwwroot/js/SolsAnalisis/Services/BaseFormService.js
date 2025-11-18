import { LovUI } from '../../Lov/LovUI';

class BaseFormService {
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

	addNewDetRow = (templateFilaDet) => {
		let template = templateFilaDet.content.cloneNode(true);

		const tr = template.firstElementChild;

		tr.innerHTML = tr.innerHTML
			.replace(/__index__/g, this.indexDet)
			.replace(/__orderNum__/g, (this.indexDet + 1));

		applyTableInputMasks(tr);

		tablaDets.querySelector('tbody').appendChild(tr);
		this.indexDet++;

		//A la nueva fila agregada, hacemos focus
		moveFocusToNext(tr);
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

}