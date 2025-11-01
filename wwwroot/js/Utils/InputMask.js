
/** REGEX **/
// La expresión /[a-zA-Z]/ solo permite caracteres alfabéticos (mayúsculas y minúsculas)
const ALPHABET_ONLY_REGEX = /[a-zA-Z]/;


// Ayuda de GEMINI. 
// Máscara para documentos paraguayos.
const pyDocumentMaskOptions = {
    mask: Number,  // Usa la máscara de número de IMask

    // Configuración de formatos
    thousandsSeparator: '.', // El separador de miles es el punto
    radix: ',',              // El separador decimal es la coma (aunque no lo usaremos, es buena práctica definirlo)

    // Límites de dígitos
    min: 1,    // Mínimo 6 dígitos (100.000)
    max: 999999999, // Máximo 9 dígitos (999.999.999) - puedes ajustar este límite

    // Opcional: Desactiva el sufijo o prefijo,
    scale: 0, // No permite decimales
    signed: false, // No permite el signo negativo

    // Opcional: Esto obliga a que la entrada se vea siempre con el formato de miles
    autofix: true,
};

//Máscara para letras mayúsculas del alfabeto
const upperAlphabetMaskOptions = {
    // Usamos el patrón simple 'a' como placeholder
    mask: 'a'.repeat(3), // Repetimos el patrón 'a' N veces (ej. hasta 3 caracteres)

    // Definición de bloques/caracteres personalizados
    definitions: {
        // La definición 'a' usará el Regex que solo permite letras
        'a': ALPHABET_ONLY_REGEX
    },

    // 3. Función de conversión a mayúsculas (Transformación)
    prepare: (appended, masked) => {
        // La función 'prepare' se ejecuta antes de insertar el carácter.
        // Convertimos el carácter que se está insertando a mayúsculas.
        return appended.toUpperCase();
    },

    // 4. Opcional: Esto ayuda si el campo ya tiene contenido
    autofix: true
};

const decimalNumberMaskOptions = (length = 2, decimals = 1) => {
    const minVal = calcularValorMinimo(decimals);
    const maxVal = calcularValorMaximo(length, decimals);

    return {
        //máscara de tipo numérico
        mask: Number,
        scale: decimals,
        thousandsSeparator: '.', //Separador de miles
        padFractionalZeros: false,
        normalizeZeros: true,
        radix: ',',
        signed: false,
        //Limites definidos con 'min' y 'max'
        min: minVal,
        max: maxVal
    };
}

const integerNumberMaskOptions = (length = 1, notZero = true) => {
    return {
        mask: Number,
        scale: 0,
        thousandsSeparator: '.',
        padFractionalZeros: false,
        normalizeZeros: true,
        signed: false,
        min: notZero ? 1 : 0,
        max: calcularValorMaximo(length, 0)
    }
};


function applyIMask(input, maskOptions) {
    const maskInstance = IMask(input, maskOptions);

    input.__imask = maskInstance;

    return maskInstance;
}

function removeIMask(input) {
    if (input.__imask)
        input.__imask.destroy();
}



/**
 * Calcula el valor máximo que puede almacenar un tipo numeric(length, decimals).
 * Asume que el tipo es unsigned (sin signo).
 * @param {number} length - Longitud total de dígitos (L).
 * @param {number} decimals - Cantidad de dígitos decimales (D).
 * @returns {number} - El valor máximo absoluto.
 */
function calcularValorMaximo(length, decimals) {
    const digitosEnteros = length - decimals;

    if (digitosEnteros <= 0) {
        return 0;
    }

    // 1. Crear la cadena de la Parte Entera (solo 9's)
    const parteEntera = '9'.repeat(digitosEnteros);

    // 2. Crear la cadena de la Parte Decimal (solo 9's)
    let parteDecimal = '';
    if (decimals > 0) {
        parteDecimal = '.' + '9'.repeat(decimals);
    }

    // 3. Combinar y convertir a número
    const valorMaximoString = parteEntera + parteDecimal;

    // Utilizamos parseFloat para manejar la conversión a número con precisión.
    return parseFloat(valorMaximoString);
}

/**
* Calcula el valor positivo mínimo (distinto de cero) 
* según la cantidad de decimales permitidos.
* * @param {number} decimals - Cantidad de dígitos decimales (D).
* @returns {number} - El valor positivo más pequeño (ej. 0.001).
*/
function calcularValorMinimo(decimals) {
    if (decimals <= 0) {
        // Si no se permiten decimales, el mínimo entero positivo es 1.
        return 1;
    }

    // Calcula 10 elevado a la potencia de los decimales (ej. 10^3 = 1000)
    const divisor = Math.pow(10, decimals);

    // El valor mínimo es 1 dividido por el divisor (ej. 1 / 1000 = 0.001)
    return 1 / divisor;
}

class InputMask {
    //Aplica la máscara al campo de entrada
    static documentMask(documentInput) {
        return applyIMask(documentInput, pyDocumentMaskOptions);
    }

    static alphabetUpperMask(alphabetUpperInput) {
        return applyIMask(alphabetUpperInput, upperAlphabetMaskOptions);
    }

    static decimalNumberMask(decimalInput, length, decimals) {
        return applyIMask(decimalInput, decimalNumberMaskOptions(length, decimals));
    }

    static onlyNumberMask(input, length, notZero) {
        return applyIMask(input, integerNumberMaskOptions(length, notZero));
    }

    static removeIMask(input) {
        removeIMask(input);
    }
}

export { InputMask }
