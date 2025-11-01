
const btnCrear = document.getElementById('btnNewElemQuimico');
const modalCrear = document.getElementById('modalCrearElemQuimico');
const modalEditar = document.getElementById('modalEditarElemQuimico');
const tablaElem = document.getElementById('tablaElemQuimicos');

btnCrear.addEventListener('click', async () => {
    
    const urlCrear = "/ElemQuimicos/CreateForm";

    await renderModalForm(modalCrear, urlCrear);
});

$(document).on("submit", "#formNuevoElemQuimico", async function (e) {
    e.preventDefault();

    let form = this;
    let formData = new FormData(form);

    await postModalFormData(modalCrear, form, formData, () => {
        window.location.href = "/ElemQuimicos/Index";
    });
});

$(document).on("submit", "#formEditElemQuimico", async function (e) {
    e.preventDefault();

    let form = this;
    let formData = new FormData(form);

    await postModalFormData(modalEditar, form, formData, () => {
        window.location.href = "/ElemQuimicos/Index";
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
