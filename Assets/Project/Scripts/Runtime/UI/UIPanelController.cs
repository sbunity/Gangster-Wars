using SBabchuk.Runtime.Architecture;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

namespace SBabchuk.Runtime.UI
{
    public class UIPanelController : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("parentPanel")]
        private UIPanelController _parentPanel;
        public UIPanelController ParentPanel { get => _parentPanel; set => _parentPanel = value; }

        [SerializeField, FormerlySerializedAs("hideIfShowChild")]
        private bool _hideIfShowChild;
        public bool HideIfShowChild { get => _hideIfShowChild; set => _hideIfShowChild = value; }

        [SerializeField, FormerlySerializedAs("panel")]
        private GameObject _panel;

        private int _instanceID;
        public int InstanceID { get => _instanceID; set => _instanceID = value; }

        private SignalBus _signalBus;
        private SignalSubscriptions _signals;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signals = new SignalSubscriptions(signalBus)
                .Add<UIPanelReplaceSignal>(Replace);
            _signals.Enable();
        }

        private void OnEnable() => _signals?.Enable();

        private void OnDisable() => _signals?.Disable();

        private void Awake()
        {
            InstanceID = gameObject.GetInstanceID();
        }

        public void Show(UIPanelController targetPanel)
        {
            _signalBus.Fire(new UIPanelReplaceSignal(targetPanel));
            if (targetPanel.InstanceID != InstanceID)
                return;

            _panel.SetActive(true);
        }

        public void Hide(UIPanelController targetPanel)
        {
            if (targetPanel.InstanceID != InstanceID)
                return;

            _panel.SetActive(false);
        }

        private void Replace(UIPanelReplaceSignal signal)
        {
            var targetPanel = signal.Panel;
            if (targetPanel.InstanceID == InstanceID)
                return;

            if (targetPanel.ParentPanel && InstanceID == targetPanel.ParentPanel.InstanceID && !targetPanel.ParentPanel.HideIfShowChild)
                return;

            _panel.SetActive(false);
        }
    }
}
