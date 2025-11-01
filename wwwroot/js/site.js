// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

const keyIsTab = (key) => key === 'Tab';
const keyIsEnter = (key) => key === 'Enter';

/* INDICAN TIPO DE AMBITO DE ÁMBITO PARA UN EVENTO. EJEMPLO: FORMULARIO, TABLA */
const FORM_SCOPE = 'input_scope';
const TABLE_ROW_SCOPE = 'table_row_scope';
const TABLE_HEAD_SCOPE = 'table_head_scope';

/**
 *  Función utilitaria:
 *  Ejecuta un GET a una url.
 *  El resultado lo renderiza en el cuerpo del modal pasado como parámetro.
 */
const renderModalForm = async (modalElem, url) => {
	const response = await fetch(url);

	if (response.ok) {
		const html = await response.text();
		modalElem.querySelector('.modal-body').innerHTML = html;
	}

	const modal = new bootstrap.Modal(modalElem);
	modal.show();
};

/**
 *  Función utilitaria:
 *  
 */
const postModalFormData = async (modalElem, form, formData, successAction) => {
	let response = await fetch(form.action, {
		method: "POST",
		body: formData
	});

	const contentType = response.headers.get('content-type');

	//Se verifica si el response es un json
	if (contentType && contentType.includes('application/json')) {
		const jsonResponse = await response.json();

		if (jsonResponse.success) {
			successAction();
		}
	}
	else {
		modalElem.querySelector('.modal-body').innerHTML = await response.text();
	}
};


const procesarSubmnitModal = (formId, modalELem, action) => {
	const form = `#${formId}`;

	$(document).on("submit", form, async function (e) {
		e.preventDefault();

		let form = this;
		let formData = new FormData(form);

		await postModalFormData(modalElem, form, formData, () => {
			action();
		});
	});
};

/**
 * https://www.freecodecamp.org/news/javascript-debounce-example/
 * Debounce function:
 * Para controlar la frecuencia con la que se ejecuta una función, 
 * útil cuando un evento se dispara repetidamente (por ejemplo, input, scroll, resize).
 */
function debounce(func, timeout = 300) {
	let timer;
	return (...args) => {
		clearTimeout(timer);
		timer = setTimeout(() => { func.apply(this, args); }, timeout);
	};
}

function InputHasValue(input) {
	return input.value.trim().length > 0;
}


function moveFocusToNext(scope, currentElem = null) {
	//Los posibles inputs son filtrados de acuerdo a si son focusables
	const inputs = Array.from(scope.querySelectorAll('input'))
		.filter(n => {
			if (n.disabled) return false;
			if (n.readOnly) return false;

			const tabindex = n.getAttribute('tabindex');
			if (tabindex === '-1') return false;

			const style = window.getComputedStyle(n);
			if (style.display === 'none' || style.visibility === 'hidden') {
				return false;
			}

			return true;
		});

	//buscamos si existe el elemento actual en el scope
	const elemIndex = !currentElem ? -1 : inputs.indexOf(currentElem);
	//Si el elemento no existe en la lista, saltamos al primero de la lista
	if (elemIndex === -1) {
		if (inputs.length > 0) {
			inputs[0].focus();
		}
	}
	else {
		//buscamos el siguiente
		const next = inputs[elemIndex + 1];
		if (next) {
			next.focus();
		}
	}
}

function getEventTargetScope(target, scopeType) {
	let scope = null;

	switch (scopeType) {
		case FORM_SCOPE:
			scope = target.form;
			break;
		case TABLE_ROW_SCOPE:
			scope = getTableRowScope(target);
			break;
	}

	return scope !== null ? scope : document;
}

function getTableRowScope(target) {
	const tr = target.closest('tr');

	if (!tr || !tr.closest('tbody')) return null;

	return tr;
}

/**
 * Obtiene el valor limpio de un input, ya sea que tenga o no una instancia de IMask.
 * @param {HTMLElement} inputElement - El elemento del DOM a inspeccionar.
 * @returns {string} El valor sin máscara.
 */
function getInputCleanValue(inputElement) {
	// 1. IMask almacena su instancia en el elemento del DOM bajo la propiedad '__imask'
	const imaskInstance = inputElement.__imask;

	if (imaskInstance) {
		// 2. Si existe la instancia, devuelve el valor limpio de IMask
		return imaskInstance.unmaskedValue;
	}

	// 3. Si NO tiene IMask (o la instancia no se encuentra), devuelve el valor normal
	return inputElement.value;
}

function setInputValue(inputElement, value) {
	const mask = inputElement.__imask;

	if (mask) {
		mask.value = value != null ? String(value) : '';
		
	}
	else {
		inputElement.value = value;
	}
}


