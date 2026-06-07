[System.Serializable]
public enum StateUpgrade
{
    None,
    Completed,      //"Апгрейд виконаний вже раніше"
    Activated,      //"Апгрейд може бути виконаний зараз"
    Deactivated,    //"Апгрейд не можна здійснити, замало бабла"
    Disabled        //"Апгрейд не доступний, бо перед ним не апгрейднуті"
}