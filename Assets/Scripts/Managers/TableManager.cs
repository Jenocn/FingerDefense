using Game.Strings;
using Game.Tables;
using GCL.Serialization;

namespace Game.Managers {
    public class TableManager : ManagerBase<TableManager> {
        private TableContainer _container = new TableContainer();

        public override void OnInitManager() {
            _container.Push(TableMapdat.instance);
            _container.Load();
        }
        public override void OnDestroyManager() {
            _container.Clear();
        }
    };
}