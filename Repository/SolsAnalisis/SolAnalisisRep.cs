
using AgroLaboratorio.Data;
using AgroLaboratorio.Exceptions;
using AgroLaboratorio.Models;
using AgroLaboratorio.Repository.Base;
using AgroLaboratorio.Utils;
using Microsoft.EntityFrameworkCore;

namespace AgroLaboratorio.Repository.SolsAnalisis;
public class SolAnalisisRep : BaseRepository<SolAnalisisCab, int>, ISolAnalisisRep
{

    public SolAnalisisRep(AppDbContext context) : base(context)
    {
    }

    public DbSet<SolAnalisisDet> DetsSet => (Context as AppDbContext).SolAnalisisDets;

    //Sobreescribimos el Update del padre por si es llamado
    public override void Update(SolAnalisisCab model)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(SolAnalisisCab model)
    {
        await UpdateCabAsync(model);
        await UpdateDetsAsync(model.CodAnalisis, [.. model.SolAnalisisDets]);
    }

    public async Task<SolAnalisisCab?> FindByCodWithDetails(int codigo)
    {
        var model = await Set.Include(m => m.SolAnalisisDets)
                                .ThenInclude(d => d.CodSolubleNavigation)
                             .FirstOrDefaultAsync(m => m.CodAnalisis == codigo);

        return model;
    }

    /*
         * Referencia para actualización de entidades tipo Master-Details
            https://medium.com/@hamidmusayev/synchronizing-entity-framework-core-child-collections-a-clean-and-reusable-approach-2ebd8e853f4d
         */
    private async Task UpdateCabAsync(SolAnalisisCab solAnalisis)
    {
        //Buscamos la solicitud existente
        var existing = await Set.Include(m => m.SolAnalisisDets)
                                .FirstOrDefaultAsync(m => m.CodAnalisis == solAnalisis.CodAnalisis)
                                ?? throw new RscNotFoundException(ErrorMessages.ResourceNotFound);

        //Rastreamos la entidad a ser actualizada (La existente)
        var entry = Set.Entry(existing);

        //Reemplazamos los valores actuales por los nuevos
        entry.CurrentValues.SetValues(solAnalisis);

        //Marcamos los valores que no requieren actualizarse para que EF Core no los modifique
        entry.Member(nameof(existing.FechaAlta)).IsModified = false;
        entry.Member(nameof(existing.CodUserAlta)).IsModified = false;
        entry.Member(nameof(existing.Fecha)).IsModified = false;

        //Aseguramos de que el estado de actualizacion sea false para los detalles
        entry.Collection(m => m.SolAnalisisDets).IsModified = false;
    }

    

    private async Task UpdateDetsAsync(int codAnalisis, List<SolAnalisisDet> incomingDets)
    {
        //Traemos a memoria los detalles existentes para la cabecera
        var existingDets = await DetsSet.Where(d => d.CodAnalisis == codAnalisis).ToListAsync();

        //Calculamos el número de línea
        int currentNroLinea = existingDets.Count != 0 ? existingDets.Max(d => d.NroLinea) : 1;

        //Recorremos cada detalle para realizar las actualizaciones
        foreach (var modified in incomingDets)
        {
            //Obtenemos el detalle existente para modificarlos
            var existingDet = existingDets.FirstOrDefault(m => m.CodAnalisis == codAnalisis 
                                                                    && m.NroLinea == modified.NroLinea
                                                                    && m.CodElemento == modified.CodElemento);

            if(existingDet != null)
            {
                //Asignamos la PK existente al objeto entrante
                modified.NroLinea = existingDet.NroLinea;
                modified.CodAnalisis = existingDet.CodAnalisis;

                DetsSet.Entry(existingDet).CurrentValues.SetValues(modified);
            }
            else
            {
                //Asignamos número de línea.
                modified.NroLinea = ++currentNroLinea; //Sumamos 1 antes de asignar
                modified.CodAnalisis = codAnalisis;

                DetsSet.Add(modified);
            }
        }

        //Eliminamos los que no existan
        var incomingDetIds = incomingDets
            .Where(x => x.NroLinea != 0) //Solo los que tengan numero de linea se consideran existentes
            .Select(x => (x.NroLinea, x.CodElemento));

        //Lo que exista en la DB y no exista en la lista entrante debe ser eliminado
        var detsToRemove = existingDets
            .Where(d => !incomingDetIds.Contains((d.NroLinea, d.CodElemento)))
            .ToList();

        DetsSet.RemoveRange(detsToRemove);
    }
}
