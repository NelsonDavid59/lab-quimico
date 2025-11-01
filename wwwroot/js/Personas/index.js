const btnCrear = document.getElementById('btnNewPersona');
const modalCrear = document.getElementById('modalCrearPersona');
const modalEditar = document.getElementById('modalEditPersona');
const tablaElem = document.getElementById('tablaPersonas');

btnCrear.addEventListener('click', async () => {
    const urlCrear = "/Personas/CreateForm";

    await renderModalForm(modalCrear, urlCrear);
});

$(document).on("submit", "#formCrearPersona", async function (e) {
    e.preventDefault();

    let form = this;
    let formData = new FormData(form);

    await postModalFormData(modalCrear, form, formData, () => {
        window.location.href = "/Personas/Index";
    });
});

$(document).on("submit", "#formEditPersona", async function (e) {
    e.preventDefault();

    let form = this;
    let formData = new FormData(form);

    await postModalFormData(modalEditar, form, formData, () => {
        window.location.href = "/Personas/Index";
    });
});

tablaElem.addEventListener("click", async function (e) {
    //Verificamos que el evento provenga de un elemento editar
    const link = e.target.closest(".btn-edit");
    if (!link) return;

    e.preventDefault(); //Cancelamos la acción del link

    const url = link.href;

    //Se carga el formulario en el modal
    await renderModalForm(modalEditar, url);
});