using SBabchuk.Runtime.Architecture;
using UnityEngine;
using Zenject;

public class UIPanelController : MonoBehaviour
{
    [Header("Панель не повинна зникати, вона є дочірна")]
    public UIPanelController parentPanel;

    [Header("Чи варто ховати батькa при відкритті дочерної панельки")]
    public bool hideIfShowChild;

    [Header("ID панельки")]
    private int _instanceID;
    public int InstanceID
    {
        get => _instanceID;
        set => _instanceID = value;
    }

    [Header("Тіло панельки")]
    public GameObject panel;

    private SignalBus _signalBus;
    private bool _isSubscribed;

    [Inject]
    private void Construct(SignalBus signalBus)
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

        panel.SetActive(true);
    }

    public void Hide(UIPanelController targetPanel)
    {
        if (targetPanel.InstanceID != InstanceID)
            return;

        panel.SetActive(false);
    }

    private void Replace(UIPanelReplaceSignal signal)
    {
        var targetPanel = signal.Panel;
        if (targetPanel.InstanceID == InstanceID)
            return;

        if (targetPanel.parentPanel && InstanceID == targetPanel.parentPanel.InstanceID && !targetPanel.parentPanel.hideIfShowChild)
            return;

        panel.SetActive(false);
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
