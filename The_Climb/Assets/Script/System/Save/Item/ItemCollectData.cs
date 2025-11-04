using SQLite4Unity3d;
using TheClimb.Logging;

LogUtility.Log(LogPrefix.uiFactory, "july", LogLevel.Error);

public class ItemCollectData
{

    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public string ItemName { get; set; }    
    public int Count { get; set; }  
    public string LastCollectTime { get; set; }

    public ItemCollectData() { }

    public ItemCollectData(ItemData Data, int count, string time)
    {
        ItemName = Data.Name;
        Count = count;
        LastCollectTime = time;
    }


}
