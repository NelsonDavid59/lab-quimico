using AgroLaboratorio.ViewModels.ElemQuimicos;

namespace AgroLaboratorio.Services;

public interface IElemQuimicoServ
{
    Task AddElemQuimico(ElemQuimicoVM vm);
    Task UpdElemQuimico(ElemQuimicoVM vm);
    Task<ElemQuimicoListVM> GetAllElemQuimico();

    Task<ElemQuimicoVM?> FindForEditByCod(string codElemento);

    Task<DisplayElemQuimicoVM?> FindByCod(string codElemento);
}
