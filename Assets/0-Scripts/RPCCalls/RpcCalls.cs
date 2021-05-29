using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RpcCalls : MonoBehaviour
{
    private PhotonView photonView;
    private void Awake() {
        photonView = GetComponent<PhotonView>();
    }


    public void FireRPC() {
        photonView.RPC("FireOnClones", RpcTarget.Others);
    }

    public void StartedToUseWeaponOnClones() {
        photonView.RPC("StatedToUseWeapon", RpcTarget.OthersBuffered);
    }

    public void EndUsingWeaponOnClones() {
        photonView.RPC("EndedToUseWeapon", RpcTarget.OthersBuffered);
    }

    [PunRPC]
    private void FireOnClone() {
        GetComponent<PlayerController>().Fire();
    }

    [PunRPC]
    private void LoadWeaponOnClones() {
        GetComponent<WeaponLoader>().LoadLastWeapon();
    }

    [PunRPC]
    private void StatedToUseWeapon() {
        GetComponent<PlayerController>().PutWeponOnActivePosition();
    }

    [PunRPC]
    private void EndedToUseWeapon() {
        GetComponent<PlayerController>().PutWeaponInRestPosition();
    }
}
