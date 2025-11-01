/**
 * @class
 */
export class LovState {
	constructor() {
		this.url = null;
		this.keys = null;
		this.page = 1;
		this.searchField = null;
		this.searchValue = null;
	}

	/**
	 * Setea la url para ejecutar la petición
	 * @param {any} rscName
	 */
	setLovUrl(rscName) {
		this.url = `/lov/${rscName}`;
	}

	/**
	 * 
	 */
	reset() {
		this.url = null;
		this.keys = null;
		this.page = 1;
		this.searchField = null;
		this.searchValue = null;
	}

	/**
	 * 
	 */
	resetLovPage() {
		this.page = 1;
	}

	/**
	 * 
	 * @param {any} keys
	 */
	setLovKeys(keys) {
		this.keys = keys;
	}

	setLovSearchField (searchField) {
		this.searchField = searchField;
	}

	/**
	 * 
	 * @param {any} searchValue
	 */
	setLovSearchValue(searchValue) {
		this.searchValue = searchValue;
	}

	/**
	 * 
	 */
	incrementLovPage() {
		this.page += 1;
	}
	/**
	 * 
	 */
	decrementLovPage() {
		if (this.page > 1) this.page -= 1;
	}
	/**
	 * 
	 * @returns
	 */
	stringifyLovReq() {
		let result = JSON.stringify(this,
			['page', 'searchField', 'searchValue']);

		return result;
	}
}