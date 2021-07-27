[System.Serializable]
public class Task
{
    public enum TaskType
    {
        Talk,
        Hunt,
        Item
    }


    public TaskType taskType;
    public string goalText;
    public int goalCount;
    public int currentCount;
    public int targetId;

}
