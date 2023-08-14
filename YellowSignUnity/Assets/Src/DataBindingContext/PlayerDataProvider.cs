using Slash.Unity.DataBind.Core.Data;
public class PlayerDataProvider : Context
{
    public Property<int> GoldAmount = new Property<int>();
    public Property<bool> TowerSpawnMode = new Property<bool>();
}
