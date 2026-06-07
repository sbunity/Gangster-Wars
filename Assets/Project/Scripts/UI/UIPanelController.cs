using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelController : MonoBehaviour
{
    public delegate void Initialize(int _id, string _name);
    public static event Initialize OnInitialize;

    public delegate void ReplaceEvent(UIPanelController _panel);
    public static event ReplaceEvent OnReplace;

    [Header("Панель не повинна зникати, вона є дочірна")]
    public UIPanelController parentPanel;

    [Header("Чи варто ховати батькa при відкритті дочерної панельки")]
    public bool hideIfShowChild;

    [Header("ID панельки")]
    private int instanceID;
    public int InstanceID
    {
        get { return instanceID; } 
        set { instanceID = value; }

    }

    [Header("Тіло панельки")]
    public GameObject panel;

    /// <summary>
    /// Підписки
    /// </summary>
    private void OnEnable()
    {
       // UIPanelsManager.OnShow += Show;
       // UIPanelsManager.OnHide += Hide;
        UIPanelController.OnReplace += Replace;
    }

    /// <summary>
    /// ВІдписки
    /// </summary>
    private void OnDisable()
    {
       // UIPanelsManager.OnShow -= Show;
       // UIPanelsManager.OnHide -= Hide;
        UIPanelController.OnReplace -= Replace;
    }

    /// <summary>
    /// Предстартова ініціалізація
    /// </summary>
    private void Awake()
    {
        InstanceID = gameObject.GetInstanceID();
    }

    /// <summary>
    /// Стартова ініціалізація
    /// </summary>
    private void Start()
    {
        //OnInitialize(InstanceID, gameObject.name);
    }

    /// <summary>
    /// Показує панель
    /// </summary>
    /// <param name="_name"></param>
    public void Show(UIPanelController _panel)
    {
        OnReplace(_panel);

        if (_panel.instanceID != InstanceID)
            return;

        panel.SetActive(true);
    }

    /// <summary>
    /// Скриває панель
    /// </summary>
    /// <param name="_name"></param>
    public void Hide(UIPanelController _panel)
    {
        if (_panel.instanceID != InstanceID)
            return;

        panel.SetActive(false);
    }

    /// <summary>
    /// Закриваєм ті панелі які є відкриті
    /// </summary>
    /// <param name="_name"></param>
    private void Replace(UIPanelController _panel)
    {
        if (_panel.instanceID == InstanceID)
            return;

        if (_panel.parentPanel)
            if (InstanceID == _panel.parentPanel.instanceID)
                if (!_panel.parentPanel.hideIfShowChild)
                return;

        panel.SetActive(false);
    }
}
