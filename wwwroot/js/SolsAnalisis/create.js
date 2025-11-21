import { NoDataErr } from '../Errors/NoDataErr.js';
import { RscNotFoundErr } from '../Errors/RscNotFoundErr.js';
import { LovUI } from '../Lov/LovUI.js';
import { InputMask } from '../Utils/InputMask.js';
import { BaseFormInit } from './Initializers/BaseFormInit.js';

const lov = new LovUI();

let indexDet = 0;

const templateFilaDet = document.getElementById('templateFilaDet');
const tablaDets = document.getElementById('tablaDetalles');
const inputAlertClass = 'msg-alert-showing';
let hideInputAlertTimeout;

const formElem = document.getElementById('formCreate');


document.addEventListener("DOMContentLoaded", function () {
	/** @type {BaseFormInit} */
	const initializer = new BaseFormInit(formElem);

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