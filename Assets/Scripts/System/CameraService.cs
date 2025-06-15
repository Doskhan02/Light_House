using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] List<Transform> transforms;
    private Camera mainCamera;
    [SerializeField]private Transform gameStartPos;
    public Vector3 startPos;
    public Vector3 startRot;
    public bool cutsceenActive = false;
    void Start()
    {
        mainCamera = Camera.main;
        GameManager.Instance.OnCutsceen += StartCutsceen;
    }

    void LateUpdate()
    {
        if (GameManager.Instance.IsGameActive && cutsceenActive == false)
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
                mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, gameStartPos.position, 0.01f);
                mainCamera.gameObject.transform.rotation = Quaternion.Lerp(mainCamera.gameObject.transform.rotation, Quaternion.Euler(gameStartPos.eulerAngles), 0.01f);
            }
        }
        else if (cutsceenActive == false && GameManager.Instance.IsGameActive == false)
        {
            mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, startPos, 0.01f);
            mainCamera.gameObject.transform.rotation = Quaternion.Lerp(mainCamera.gameObject.transform.rotation, Quaternion.Euler(startRot), 0.01f);
        }
    }

    public void StartCutsceen()
    {
        StartCoroutine(Cutsceen());
    }

    public IEnumerator Cutsceen()
    {
        cutsceenActive = true;


        foreach (Transform t in transforms)
        {
            Vector3 targetPosition = t.position;
            Quaternion targetRotation = t.rotation;

            // Move until we reach position AND rotation
            while (true)
            {
                // Move towards position
                mainCamera.transform.position = Vector3.Lerp(
                    mainCamera.transform.position,
                    targetPosition,
                    Time.deltaTime
                );

                // Rotate towards rotation
                mainCamera.transform.rotation = Quaternion.Slerp(
                    mainCamera.transform.rotation,
                    targetRotation,
                    Time.deltaTime
                );

                // Check if we've arrived
                if (Vector3.Distance(mainCamera.transform.position, targetPosition) < 0.01f &&
                    Quaternion.Angle(mainCamera.transform.rotation, targetRotation) < 0.5f)
                {
                    mainCamera.transform.position = targetPosition;
                    mainCamera.transform.rotation = targetRotation;
                    break;
                }

                yield return null;
            }

            // Optional: wait before moving to next point
            yield return new WaitForSeconds(1f);
        }

        cutsceenActive = false;
        GameManager.Instance.IsCutsceenActive = false;
        GameManager.Instance.WindowService.HideWindow<CutsceenWindow>(false);
        GameManager.Instance.WindowService.ShowWindow<GamePlayWindow>(false);
    }
}
