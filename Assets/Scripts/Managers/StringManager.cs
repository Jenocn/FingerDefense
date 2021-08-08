using Game.Strings;
using GCL.Serialization;

namespace Game.Managers {
    public class StringManager : ManagerBase<StringManager> {
        private TableContainer _container = new TableContainer();

        public override void OnInitManager() {
            _container.Push(StringUi.instance);
            _container.Push(StringTalk.instance);
            _container.Push(StringStory.instance);
            _container.Load();
        }
        public override void OnDestroyManager() {
            _container.Clear();
        }
    };
}