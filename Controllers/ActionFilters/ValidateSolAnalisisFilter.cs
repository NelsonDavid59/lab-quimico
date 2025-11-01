using AgroLaboratorio.Data;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.SolsAnalisis;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AgroLaboratorio.Controllers.ActionFilters
{
    public class ValidateSolAnalisisFilter : IAsyncActionFilter
    {
        //Contexto de base de datos
        private readonly AppDbContext _ctx;

        public ValidateSolAnalisisFilter(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var model = context.ActionArguments.Values
                                .OfType<SolAnalisisVM>()
                                .FirstOrDefault();

            var modelState = context.ModelState;

            if (model == null)
            {
                await next();
                return;
            }

            //Validación de Detalles inexistentes
            if(model.Detalles == null || !model.Detalles.Any())
            {
                modelState.AddModelError("Detalles", "Deben existir detalles para el análisis.");
                await next();
                return;
            }

            foreach(var item in model.Detalles.Select((d, i) => new { Detalle = d, Index = i }))
            {
                //Clave para el error relacionado con el indice de la lista
                var errorPrefix = $"Detalles[{item.Index}]";

                //Validación de Elemento Químico
                if (!_ctx.ElemQuimicos.Any(e => e.CodElemento == item.Detalle.CodElemento))
                {
                    ValidationHelper.AddModelErrorWithDisplayName<SolAnalisisDetVM>
                    (
                        modelState,
                        errorPrefix,
                        nameof(item.Detalle.CodElemento),
                        ErrorMessages.InvalidId
                    );
                }

                //Valor de Garantía
                if (item.Detalle.GarantiaVal == null)
                {
                    ValidationHelper.AddModelErrorWithDisplayName<SolAnalisisDetVM>
                        (
                            modelState,
                            errorPrefix,
                            nameof(item.Detalle.GarantiaVal),
                            ErrorMessages.RequiredField
                        );
                }

                //Validación de Solubilidad
                if(!_ctx.Solubilidads.Any(s => s.CodSoluble == item.Detalle.CodSoluble))
                {
                    ValidationHelper.AddModelErrorWithDisplayName<SolAnalisisDetVM>
                    (
                        modelState,
                        errorPrefix,
                        nameof(item.Detalle.CodSoluble),
                        ErrorMessages.InvalidId
                    );
                }
            }

            await next();
        }
    }
}
