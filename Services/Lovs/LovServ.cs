using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using AgroLaboratorio.Data;
using AgroLaboratorio.Models;
using AgroLaboratorio.Utils;
using AgroLaboratorio.ViewModels.Lovs;
using AgroLaboratorio.ViewModels.Lovs.Base;
using Microsoft.EntityFrameworkCore;
using AgroLaboratorio.Extensions;

namespace AgroLaboratorio.Services.Lovs;

public class LovServ 
{
    private readonly AppDbContext _context;

    public LovServ(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LovResultVM> GetLovResult(LovRequest request)
    {
        switch (request.RscName) 
        {
            case LOV.Solubilidad: // LOV/SOLUBILIDADES
                return await Solubilidades(request);
            case LOV.ElemQuimico:
                return await ElemQuimicos(request);
            case LOV.Persona:
                return await Personas(request);
            default:
                return new LovResultVM();
        }
    }

    private async Task<LovResultVM> Solubilidades(LovRequest rq)
    {
        var query = _context.Set<Solubilidad>().AsQueryable();

        //Antes de aplicar cualquier filtro analizamos si existen datos
        bool isDataAvaliable = await query.AnyAsync();

        if (rq.IsSearchable())
        {
            switch (rq.SearchField)
            {
                case nameof(LovSolublVM.Codigo):
                    int codigo = 0;

                    if(!int.TryParse(rq.SearchValue, out codigo))
                    {
                        query = query.Where(s => false);
                    }
                    else
                    {
                        query = ApplyFilter(query, s => s.CodSoluble == codigo);
                    }
                    break;

                case nameof(LovSolublVM.Descripcion):
                    var search = rq.SearchValue.ToUpper();
                    query = ApplyFilter(query, s => s.Descripcion.ToUpper().Contains(search));
                    break;
            }
        }
        
        var selectedQuery = query.Select(m => new LovSolublVM
                        {
                            Codigo = m.CodSoluble,
                            Descripcion = m.Descripcion
                        });

        rq.SortColumn = "Codigo";

        var lovResult = await ExecuteQuery(selectedQuery, rq);
        lovResult.IsDataAvailable = isDataAvaliable;

        return lovResult;
    }

    private async Task<LovResultVM> ElemQuimicos(LovRequest rq)
    {
        var query = _context.Set<ElemQuimico>().AsQueryable();

        //Antes de aplicar cualquier filtro analizamos si existen datos
        bool isDataAvaliable = await query.AnyAsync();

        if (rq.IsSearchable())
        {
            switch (rq.SearchField)
            {
                case nameof(LovElemQuimicoVM.Codigo):
                    query = ApplyFilter(query, e => e.CodElemento.ToUpper()
                                                        .Equals(rq.SearchValue.ToUpper()));
                    break;
            }
        }

        var selectedQuery = query
                    .Select(e => new LovElemQuimicoVM
                    {
                        Codigo = e.CodElemento,
                        Descripcion = e.Descripcion
                    });

        rq.SortColumn = "Codigo";

        var lovResult = await ExecuteQuery(selectedQuery, rq);
        lovResult.IsDataAvailable = isDataAvaliable;

        return lovResult;
    }

    private async Task<LovResultVM> Personas(LovRequest rq)
    {
        var query = _context.Set<Persona>().AsQueryable();

        //Antes de aplicar cualquier filtro analizamos si existen datos
        bool isDataAvaliable = await query.AnyAsync();

        if (rq.IsSearchable())
        {
            //Casos de filtrado
            switch (rq.SearchField)
            {
                case nameof(LovPersonaVM.Nombre):
                    var search = rq.SearchValue.ToUpper();
                    query = ApplyFilter(query, p => (p.Nombre + " " + p.Apellido).ToUpper()
                                                        .Contains(search));
                    break;
                case nameof(LovPersonaVM.Documento):
                    query = ApplyFilter(query, p => p.Documento.Contains(rq.SearchValue));
                    break;
                case nameof(LovPersonaVM.Codigo):
                    //Manejamos el tipo int
                    int codigo = 0;

                    if(!int.TryParse(rq.SearchValue, out codigo))
                    {
                        //Si falla la conversión del valor de entrada se retorna un resultado vacío
                        query = query.Where(p => false);
                    }
                    else
                    {
                        query = ApplyFilter(query, p => p.CodPersona == codigo);
                    }
                        
                    break;
            }
        }
        
        var selectedQuery = query.Select(p => new LovPersonaVM
                        {
                            Codigo = p.CodPersona,
                            Nombre = p.Nombre + " " + p.Apellido,
                            Documento = p.Documento
                        });

        rq.SortColumn = "Codigo";

        var lovResult = await ExecuteQuery(selectedQuery, rq);
        lovResult.IsDataAvailable = isDataAvaliable;

        return lovResult;
    } 

    private async Task<LovResultVM> ExecuteQuery<T>(IQueryable<T> query, LovRequest rq) where T : LovBaseVM
    {
        //Paginación
        var totalRows = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((decimal)totalRows / rq.PageSz);

        //Ajuste de paginas
        if (rq.Page < 1) rq.Page = 1;
        if (rq.Page > totalPages) rq.Page = totalPages;

        if(totalRows > 0)
        {
            query = query
            .Skip((rq.Page - 1) * rq.PageSz)
            .Take(rq.PageSz);
        }

        //Ordenamiento
        query = ApplyDynamicSort(query, rq.SortColumn, rq.SortDir);

        var items = await query.ToListAsync();

        var result = new LovResultVM
        {
            Items = items.SerializeLovItems(),
            TotalRows = totalRows,
            TotalPages = totalPages,
            HasNextPage = rq.Page < totalPages,
            HasPreviousPage = rq.Page > 1 && rq.Page <= totalPages
        };

        return result;
    }
    
    private IQueryable<T> ApplyFilter<T>(IQueryable<T> query, Expression<Func<T, bool>> filtro)
    {
        return query.Where(filtro);
    }

    //Helper que ayuda a ordenar de forma dinamica
    private IQueryable<T> ApplyDynamicSort<T>(IQueryable<T> query, string sortColumn, string sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortColumn)) return query;
    
        var parameter = Expression.Parameter(typeof(T), "x");           // x
        var property = Expression.Property(parameter, sortColumn);      // x.Propiedad
        var lambda = Expression.Lambda(property, parameter);            // x => x.Propiedad

        var methodName = sortDirection?.ToLower() == "DESC" ? "OrderByDescending" : "OrderBy";

        var resultExpression = Expression.Call(
            typeof(Queryable),                                          // Llama a Queryable.OrderBy / OrderByDescending
            methodName,
            new Type[] { typeof(T), property.Type },                    // Generics <T, TipoDePropiedad>
            query.Expression,                                           // La query original
            lambda);                                                    // x => x.Propiedad
        
        return query.Provider.CreateQuery<T>(resultExpression);         // Construye la nueva query ordenada
    }
}
