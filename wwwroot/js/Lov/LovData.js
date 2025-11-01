
export class LovData {

	constructor(lovState) {
		this.lovState = lovState;
	}

	/**
	 * 
	 * @returns
	 */
	async execLovRequest() {
		const reqBody = this.lovState.stringifyLovReq();

		const response = await fetch(this.lovState.url, {
			headers: { "content-type": "application/json" },
			method: 'POST',
			body: reqBody
		});

		if (!response.ok) {
			throw new Error("Error en la request.");
		}

		const data = await response.json();
		return data;
	}
}