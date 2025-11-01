using AgroLaboratorio.Models;
using AgroLaboratorio.Repository.Base;

namespace AgroLaboratorio.Repository;

public interface IElemQuimicoRep : IRepository<ElemQuimico, string>
{
    Task<List<ElemQuimico>> SearchByCod(string codElemento);

    Task<List<ElemQuimico>> SearchByDesc(string desc);
}
