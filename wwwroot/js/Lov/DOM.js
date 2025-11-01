/**
 * Referencias a los elementos HTML del modal para los LOV
 */
const modalLovElem = document.getElementById('mainLovModal');
const templatePagerInfo = document.getElementById('lovPagerInfo');
const table = modalLovElem.querySelector(".modal-body")
	.querySelector("table");

export const DOM = {
	modalLovElem: modalLovElem,
	templatePagerInfo: templatePagerInfo,
	table: table,
	tBody: table.querySelector("tbody"),
	tHead: table.querySelector("thead"),
	btnNextPage: modalLovElem.querySelector(".btn-next-page"),
	btnPrevPage: modalLovElem.querySelector(".btn-prev-page"),
	templatePagerInfo: document.getElementById('lovPagerInfo'),
	lovPagerInfo: modalLovElem.querySelector(".lov-pager-info"),
	searchInput: modalLovElem.querySelector("#filtroCampoInput")
};