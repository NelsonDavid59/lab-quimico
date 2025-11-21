
export const moveFocusToNext = (scope, currentElem = null) => {
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