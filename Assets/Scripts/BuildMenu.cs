using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : MonoBehaviour
{
    [Header("Les Plans de Salles (Prefabs)")]
    public Room PowerRoomPrefab;
    public Room FoodRoomPrefab;
    public Room WaterRoomPrefab;
    public Room LaboratoryRoomPrefab;

    public void OnClickBuildPower()
    {
        if (PowerRoomPrefab != null) RoomManager.Instance.StartPlacingRoom(PowerRoomPrefab);
    }

    public void OnClickBuildFood()
    {
        if (FoodRoomPrefab != null) RoomManager.Instance.StartPlacingRoom(FoodRoomPrefab);
    }

    public void OnClickBuildWater()
    {
        if (WaterRoomPrefab != null) RoomManager.Instance.StartPlacingRoom(WaterRoomPrefab);
    }

    public void OnClickBuildLaboratory()
    {
        if (LaboratoryRoomPrefab != null) RoomManager.Instance.StartPlacingRoom(LaboratoryRoomPrefab);
    }
}
