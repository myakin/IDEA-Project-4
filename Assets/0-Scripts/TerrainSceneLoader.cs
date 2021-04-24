using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSceneLoader : MonoBehaviour {
    public string terrainToLoadOnEnterSameDirection, terrainToLoadOnEnterOppositeDirection, terrainToUnloadOnExitSameDirection, terrainToUnloadOnExitOppositeDirection;

    public void LoadOnEnterSameDirection() {
        AddressablesManager.instance.LoadAddressableSceneAdditive(terrainToLoadOnEnterSameDirection);
    }
    public void LoadOnEnterOppositeDirection() {
        AddressablesManager.instance.LoadAddressableSceneAdditive(terrainToLoadOnEnterOppositeDirection);
    }

    public void UnloadOnExitSameDirection() {
        AddressablesManager.instance.UnloadAddressableScene(terrainToUnloadOnExitSameDirection);
    }

    public void UnloadOnExitOppositeDirection() {
        AddressablesManager.instance.UnloadAddressableScene(terrainToUnloadOnExitOppositeDirection);
    }
}
