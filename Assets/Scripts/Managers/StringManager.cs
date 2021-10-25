using Game.Strings;
using Game.Systems;
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

        public override void OnStartManager() {
            LocalizationSystem.message.AddListener(this, () => {
                _container.Reload();
            });
        }

        public override void OnDestroyManager() {
            LocalizationSystem.message.RemoveListener(this);
            _container.Clear();
        }
    };
}