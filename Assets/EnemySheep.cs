using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySheep : MonoBehaviour
{
    public GameObject Player;
    public RoomCondition RoomConditionGO;

    public LayerMask layerMask;

    public GameObject DangerMarker;
    public GameObject EnemyBolt;

    public Transform BoltGenPosition;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("❌ Không tìm thấy Player! Kiểm tra lại tag Player trong Unity.");
        }

        // Tìm RoomCondition ở đối tượng cha gần nhất
        RoomConditionGO = GetComponentInParent<RoomCondition>();
        if (RoomConditionGO == null)
        {
            Debug.LogError("❌ Không tìm thấy RoomCondition trong các đối tượng cha!");
            return;
        }

        StartCoroutine(WaitPlayer());
    }

    IEnumerator WaitPlayer()
    {
        yield return null;

        while (!RoomConditionGO.playerInThisRoom)
        {
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(4f);
        transform.LookAt(Player.transform.position);
        DangerMarkerShoot();

        yield return new WaitForSeconds(2f);
        Shoot();

    }

    void DangerMarkerShoot()
    {
        Vector3 NewPosition = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        Physics.Raycast(NewPosition, transform.forward, out RaycastHit hit, 30f, layerMask);

        if (hit.transform.CompareTag("Wall"))
        {
            GameObject DangerMarkerClone = Instantiate(DangerMarker, NewPosition, transform.rotation);
            DangerMarkerClone.GetComponent<DangerLine>().EndPosition = hit.point;
        }
    }

    void Shoot()
    {
        Vector3 CurrentRotation = transform.eulerAngles + new Vector3(-90, 0, 0);
        Instantiate(EnemyBolt, BoltGenPosition.position, Quaternion.Euler(CurrentRotation));
    }
}