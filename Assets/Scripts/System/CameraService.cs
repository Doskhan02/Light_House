using UnityEngine;

public class CameraService : MonoBehaviour
{
    [SerializeField] GameObject target;
    private Camera mainCamera;
    private Vector3 gameStartPos;
    private Vector3 gameStartRot;
    public Vector3 startPos;
    public Vector3 startRot;
    void Start()
    {
        mainCamera = Camera.main;
        gameStartPos = new Vector3 (0, 38, -14);
        gameStartRot = new Vector3(35, 0, 0);
    }

    void LateUpdate()
    {
        if (GameManager.Instance.IsGameActive)
        {
            Vector2 mousePosition = target.transform.position;
            if (mousePosition.x > Screen.width * 0.8f && mainCamera.gameObject.transform.position.x < 2)
            {
                Vector3 offsetPos = mainCamera.gameObject.transform.position + new Vector3(2,0,0);
                mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, offsetPos, 0.01f);
            }
            else if (mousePosition.x < Screen.width * 0.2f && mainCamera.gameObject.transform.position.x > -2)
            {
                Vector3 offsetPos = mainCamera.gameObject.transform.position + new Vector3(-2, 0, 0);
                mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, offsetPos, 0.01f);
            }
            else
            {
                mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, gameStartPos, 0.01f);
                mainCamera.gameObject.transform.rotation = Quaternion.Lerp(mainCamera.gameObject.transform.rotation, Quaternion.Euler(gameStartRot), 0.01f);
            }
        }
        else
        {
            mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, startPos, 0.01f);
            mainCamera.gameObject.transform.rotation = Quaternion.Lerp(mainCamera.gameObject.transform.rotation, Quaternion.Euler(startRot), 0.01f);
        }
    }
}
