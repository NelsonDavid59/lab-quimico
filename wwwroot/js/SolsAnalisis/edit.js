import { BaseFormInit } from './Initializers/BaseFormInit.js';

const formElem = document.getElementById('formEditSolAnalisis');

/**
 * Inicialización de la página
 */
document.addEventListener("DOMContentLoaded", function () {
	/** @type {BaseFormInit} */
	const initializer = new BaseFormInit(formElem, window.formConfig ?? {});

	initForm(initializer);
});

const initForm = (initializer) => {
	initializer.initEventListeners(formElem);
	initializer.initMaskToTableRows();
}

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