using AgroLaboratorio.Data;
using AgroLaboratorio.Models;
using Microsoft.EntityFrameworkCore;
using AgroLaboratorio.Repository.Base;

namespace AgroLaboratorio.Repository;

public class ElemQuimicoRep : BaseRepository<ElemQuimico, string>, IElemQuimicoRep
{
    public ElemQuimicoRep(AppDbContext context) : base(context)
    {

    }

    public Task<List<ElemQuimico>> SearchByCod(string codElemento)
    {
        throw new NotImplementedException();
    }

    public Task<List<ElemQuimico>> SearchByDesc(string desc)
    {
        throw new NotImplementedException();
    }
}
