using AgroLaboratorio.ViewModels.SolsAnalisis;

namespace AgroLaboratorio.Services.SolsAnalisis
{
    public interface ISolAnalisisServ
    {
        public Task AddSolicitud(SolAnalisisVM vm);
        public Task UpdSolicitud(SolAnalisisVM vm);

        public Task<SolAnalisisListVM> GetAllSols();

        public Task<DisplaySolAnalisisVM?> FindByCod(int id);

        public Task<SolAnalisisVM?> FindForEditByCod(int id);
    }
}
