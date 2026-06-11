using SBabchuk.Runtime.Architecture;
using UnityEngine;
using Zenject;
using UnityEngine.Serialization;

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
    private bool _isSubscribed;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        Subscribe();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

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

    private void Subscribe()
    {
        if (_isSubscribed || _signalBus == null)
            return;

        _signalBus.Subscribe<UIPanelReplaceSignal>(Replace);
        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || _signalBus == null)
            return;
            
        _signalBus.Unsubscribe<UIPanelReplaceSignal>(Replace);
        _isSubscribed = false;
    }
}
