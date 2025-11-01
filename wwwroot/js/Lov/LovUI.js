import { LovState } from './LovState.js';
import { LovData } from './LovData.js';
import { RscNotFoundErr } from '../Errors/RscNotFoundErr.js'
import { DOM } from './DOM.js';
import { NoDataErr } from '../Errors/NoDataErr.js';
import { ErrorMessages } from '../Utils/ErrorMessages.js';

const {
	modalLovElem, table, tBody,
	tHead, btnNextPage, btnPrevPage,
	templatePagerInfo, lovPagerInfo, searchInput
} = DOM;

export class LovUI {
	constructor() {
		/**
		 * @type {LovState}
		 */
		this.lovState = new LovState();
		this.tableData = {};
		/**
		 * @type {LovData}
		 */
		this.lovData = new LovData(this.lovState);
    }

	/**
	 * 
	 * @param {{input , targetsFunc} }
	 * input: Input que abre el modal
	 * getTargetsFunc: Funcion que retorna un arreglo de referencias
	 * a los inputs que deben ser completados por el LOV.
	 * @returns
	 */
	showLovModal({ input, targetsFunc, lovData, nextFocusScope }) {
		this.resetFiltroLov();

		tHead.innerHTML = '';
		tBody.innerHTML = '';

		//agregamos la referencia del input al modal
		//de esa forma podremos trabajar siempre con esa referencia mas adelante
		modalLovElem.inputTarget = input;
		modalLovElem.inputTargetsFunc = targetsFunc;
		modalLovElem.nextFocusScope = nextFocusScope;

		//Los datos de lov los mostramos en el modal
		if (lovData) {
			//Si no existe la referencia del modal
			if (!this.modal) {
				this.modal = new bootstrap.Modal(modalLovElem);
				//se agrega una vez el manejador del evento
				table.addEventListener('click', (e) => this.handleTableElemClick(e));
				btnNextPage.addEventListener('click', e => this.onNextPageClick(e));
				btnPrevPage.addEventListener('click', e => this.onPrevPageClick(e));
				//El uso de debounce es para retrasar la ejecución de la búsqueda
				searchInput.addEventListener('input', debounce(e => this.onSearchInput(e), 500));
			}

			const params = {
				items: lovData.items,
				modal: modalLovElem
			};

			this.buildLovTable(params);

			//Datos de paginación
			this.setPagerInfo(lovData);

			this.modal.show();
		}
	}

	/**
	 * Maneja cuando se hace click en una fila del LOV.
	 * @param {any} tr
	 */
	handleRowClick(tr) {
		const inputTarget = modalLovElem.inputTarget;
		const inputTargetsFunc = modalLovElem.inputTargetsFunc;
		const nextFocusScope = modalLovElem.nextFocusScope;

		if (inputTarget) {
			//Obtenemos el item correspondiente a la fila del lov
			const rowItem = this.tableData[tr.dataset.rowIndex];

			this.modal.hide();

			//Completamos el input y sus targets
			this.completeLovInput({
				input: inputTarget,
				targetsFunc: inputTargetsFunc,
				lovItem: rowItem,
				nextFocusScope: nextFocusScope
			});
		}
	}

	handleHeaderClick(th) {

		//Identificamos que sea haya hecho click en una fila de la tabla
		const searchData = th.searchData;
		if (searchData) {
			this.configFiltroLov(searchData);
		}
	}

	handleTableElemClick(e) {
		const target = e.target;

		const tr = target.closest('tr');

		if (!tr) return;

		if (tr.closest('thead')) { //Identificamos que se haya hecho click en una cabecera de la tabla
			//obtenemos la cabecera (th)
			const th = target.closest('th');

			this.handleHeaderClick(th);
		}
		else if (tr.closest('tbody')) { //Identificamos que sea haya hecho click en una fila de la tabla
			//Llamamos al manejador de filas
			this.handleRowClick(tr);
		}
	}

	async onSearchInput(keyDownEvent) {
		const input = keyDownEvent.target;
		const searchField = input.dataset.searchField;

		if (searchField) {
			this.lovState.resetLovPage();
			this.lovState.setLovSearchField(searchField);
			this.lovState.setLovSearchValue(getInputCleanValue(input));

			const lovResult = await this.lovData.execLovRequest();

			if (lovResult) {
				this.buildLovTableBody(lovResult.items);

				//Datos de paginación
				this.setPagerInfo(lovResult);
			}
		}
	}

	async onNextPageClick(e) {
		e.preventDefault();

		//Modificamos el estado de la pagina del objeto request
		this.lovState.incrementLovPage();

		this.renderLovRequest();
	}

	async onPrevPageClick (e) {
		e.preventDefault();

		//Modificamos el estado de la pagina del objeto request
		this.lovState.decrementLovPage();

		this.renderLovRequest();
	}

	async renderLovRequest() {
		const lovResult = await this.lovData.execLovRequest();

		if (lovResult) {
			this.buildLovTableBody(lovResult.items);

			//Datos de paginación
			this.setPagerInfo(lovResult);
		}
	}

	/**
	 * 
	 * @param {any} input
	 * @returns
	 */
	async executeLovForInput({ input, targetsFunc, nextFocusScope }) {
		//Reiniciamos el estado de paginación y filtro
		this.lovState.reset();

		const rscName = input.dataset.rscName; //nombre del recurso que quiero
		const searchValue = getInputCleanValue(input);
		const searchField = input.dataset.rscField;

		//Solo sigue si encuentran dichas propiedades
		if (!rscName) return;

		//Construimos la ruta correspondiente al lov
		//Se identifica con data-rsc-name del input
		this.lovState.setLovUrl(rscName);

		//Seteamos los valores de búsqueda por campo
		//Si no existen, el lov buscará todos
		if (searchField) this.lovState.setLovSearchField(searchField);
		if (searchValue) this.lovState.setLovSearchValue(searchValue);

		//Ejecutamos la llamada a los datos
		const lovResult = await this.lovData.execLovRequest();

		/*
			El lov puede completar directamente el campo si es que solo existe un
		resultado.
			En caso de que más de un ítem sea encontrado (De acuerdo al campo 
		de búsqueda), se mostrará el modal.
			Si existe un único resultado podrá completarse diréctamente el resultado.
			Si no hay resultados deberá lanzarse un error.
		*/
		if (lovResult.items.length > 0) {
			let lovParams = { input: input, nextFocusScope: nextFocusScope };
			lovParams['targetsFunc'] = targetsFunc;

			//Más de un resultado muestran el modal.
			if (lovResult.items.length > 1) {
				lovParams['lovData'] = lovResult;
				this.showLovModal(lovParams);
			}
			else { //Un sólo resultado completa directamente los targets.
				lovParams['lovItem'] = lovResult.items[0];
				this.completeLovInput(lovParams);
			}
		}
		else if (lovResult.isDataAvailable) {
			//Si existen datos disponibles se indica que no hay resultados.
			throw new RscNotFoundErr(ErrorMessages.SEARCH_NOT_FOUND);
		}
		else {
			throw new NoDataErr(ErrorMessages.RSC_NO_DATA);
		}
	}

	completeLovInputTargets({ targets, lovItem }) {
		targets.forEach(input => {
			const fieldName = input.dataset.rscField;
			if (fieldName) setInputValue(input, lovItem[fieldName].value ?? "");
		});
	}

	/**
	 * 
	 * @param {any} param0
	 */
	completeLovInput({ input, targetsFunc, lovItem, nextFocusScope }) {
		/*
			Los targets son un arreglo de inputs.
			En caso de no existir una funcion que retorne un arreglo solo generamos
		un arreglo vacío.
		*/
		let inputTargets = targetsFunc ? targetsFunc() : [];

		//Como mínimo siempre tendremos al propio input que llama al LOV para completarlo.
		inputTargets.push(input);

		this.completeLovInputTargets({ targets: inputTargets, lovItem: lovItem });

		//Focus
		moveFocusToNext(nextFocusScope, input);
	}

	setPagerInfo(lovResult) {
		let templateHtml = templatePagerInfo.innerHTML;

		//Se reemplazan los datos
		templateHtml = templateHtml
			.replace(/__currentPage__/g, this.lovState.page)
			.replace(/__pageCount__/g, lovResult.totalPages);

		lovPagerInfo.innerHTML = templateHtml;

		btnNextPage.disabled = !lovResult.hasNextPage;
		btnPrevPage.disabled = !lovResult.hasPreviousPage;
	}

	buildLovTable({ items }) {
		tHead.innerHTML = '';
		if (!items || items.length === 0) return; //nada que mostrar

		//claves del primer objeto (Igual para todos)
		const fields = Object.entries(items[0]);
		const keys = Object.keys(items[0]);

		//Buscamos el primer campo searchable
		const searchField = Object.entries(items[0])
			.find(([k, v]) => v.searchable);

		//Asignamos el campo al buscador del LOV
		if (searchField) {
			const [field, fieldValue] = searchField;

			this.configFiltroLov({ title: fieldValue.title, searchField: field });
		}

		this.lovState.setLovKeys(keys);

		//Cabeceras de la tabla
		tHead.appendChild(this.addTHeads(fields));

		this.buildLovTableBody(items);
	}

	buildLovTableBody(items) {
		tBody.innerHTML = '';

		if (!items || items.length === 0) return; //nada que mostrar

		this.tableData = {}; //Limpia los datos almacenados

		items.forEach((item, index) => {
			this.addTBodyRows(item, this.lovState.keys, index);
		});
	}

	addTHeads(fields) {
		const tr = document.createElement('tr');

		fields.forEach(f => {
			const th = document.createElement('th');
			const [fieldKey, fieldVal] = f;
			th.textContent = fieldVal.title;

			if (fieldVal.searchable) {
				th.classList.add("searchable-th");
				th.searchData = { title: fieldVal.title, searchField: fieldKey };
			}

			tr.appendChild(th);
		});

		return tr;
	}

	addTBodyRows(item, colNames, index) {
		const tr = document.createElement('tr');

		colNames.forEach(k => {
			const td = document.createElement('td');
			td.textContent = item[k]["value"] ?? "";

			tr.appendChild(td);
		});

		//Guardamos el item en el indice de la fila
		this.tableData[index] = item;
		//Guardamos el indice de la fila en la propia fila
		tr.dataset.rowIndex = index;

		tBody.appendChild(tr);
	}

	/**
	 * 
	 * @param {input} - Input a partir del cual se quiere especificar el tipo de recurso
	 *					   obtenido en el LOV.
	 * @param {scope} - Ámbito para buscar los inputs que queremos completar una vez 
	 *					   seleccionada una fila del LOV.
							Por defecto DOCUMENT. Otro ejemplo: tr
	 * @returns
	 */
	getLovInputTargets(input, scope = document) {
		const rscName = input.dataset.rscName;
		/*
			rscName: Indica el nombre del recurso que deberá ser buscado con el LOV.
	
			El objetivo aquí es buscar todos los inputs que tengan un data-lov-target 
			que coincida con el rscName del input que abre el LOV.
			El objetivo final es completar todos esos inputs con su campo correspondiente
			indicado por data-rsc-field.
		*/
		if (rscName) {
			const targets = scope.querySelectorAll(`input[data-lov-target=${rscName}]`);
			return Array.from(targets);
		}

		//Arreglo vacío
		return [];
	}

	configFiltroLov({ title, searchField }) {
		if (title && searchField) {
			const label = modalLovElem.querySelector('label[for="filtroCampoInput"]');

			label.textContent = title;

			searchInput.dataset.searchField = searchField;

			setInputValue(searchInput, '');

			searchInput.disabled = false;
		}
	}

	resetFiltroLov() {
		const label = modalLovElem.querySelector('label[for="filtroCampoInput"]');

		label.textContent = "Campo de Filtro";
		delete searchInput.dataset.searchField;
		setInputValue(searchInput, '');
		searchInput.disabled = true;
	}
}

export default LovUI;