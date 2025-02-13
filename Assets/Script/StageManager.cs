using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public GameObject Player;

    [System.Serializable]
    public class StartPositionArray
    {
        public List<Transform> StartPosition = new List<Transform>();
    }

    public StartPositionArray[] startPositionArrays; // Danh sách vị trí cho các phòng bình thường
    public List<Transform> StartPositionAngel = new List<Transform>(); // Danh sách vị trí phòng Angel
    public List<Transform> StartPositionBoss = new List<Transform>(); // Danh sách vị trí phòng Boss
    public Transform StartPositionLastBoss; // Vị trí Boss cuối

    public int currentStage = 0;  // Stage hiện tại
    private int LastStage = 20;    // Stage cuối cùng

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Thử di chuyển Player");
            Player.transform.position += new Vector3(5, 0, 0); // Dịch Player 5 đơn vị theo trục X
        }
    }

    public void NextStage()
    {
        if (Player == null)
        {
            Debug.LogError("Player chưa được gán!");
            return;
        }

        currentStage++;
        Debug.Log("Chuyển sang Stage: " + currentStage);

        if (currentStage > LastStage)
        {
            Debug.Log("Đã đạt đến màn cuối!");
            return;
        }

        Vector3 newPosition = Player.transform.position; // Vị trí mặc định nếu không có stage mới

        if (currentStage % 5 != 0) // Stage bình thường (không phải Boss / Angel)
        {
            int arrayIndex = currentStage / 10;
            if (arrayIndex < startPositionArrays.Length && startPositionArrays[arrayIndex].StartPosition.Count > 0)
            {
                int randomIndex = Random.Range(0, startPositionArrays[arrayIndex].StartPosition.Count);
                newPosition = startPositionArrays[arrayIndex].StartPosition[randomIndex].position;
                startPositionArrays[arrayIndex].StartPosition.RemoveAt(randomIndex); // Xóa vị trí đã dùng
            }
            else
            {
                Debug.LogWarning("Không có vị trí nào trong startPositionArrays!");
            }
        }
        else
        {
            if (currentStage == LastStage) // Boss cuối
            {
                newPosition = StartPositionLastBoss.position;
            }
            else if (currentStage % 10 == 5) // Angel Room
            {
                if (StartPositionAngel.Count > 0)
                {
                    int randomIndex = Random.Range(0, StartPositionAngel.Count);
                    newPosition = StartPositionAngel[randomIndex].position;
                }
                else
                {
                    Debug.LogWarning("Không có vị trí Angel Room!");
                }
            }
            else // Mid Boss Room
            {
                if (StartPositionBoss.Count > 0)
                {
                    int randomIndex = Random.Range(0, StartPositionBoss.Count);
                    newPosition = StartPositionBoss[randomIndex].position;
                    StartPositionBoss.RemoveAt(randomIndex);
                }
                else
                {
                    Debug.LogWarning("Không có vị trí Boss Room!");
                }
            }
        }

        CharacterController controller = Player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;  // Tắt CharacterController trước khi thay đổi vị trí
            Player.transform.position = newPosition;
            //controller.enabled = true;   // Bật lại để tiếp tục sử dụng CharacterController
        }
        else
        {
            Player.transform.position = newPosition;
        }
    }
}
