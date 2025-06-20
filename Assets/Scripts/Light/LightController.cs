using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private LightData data;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject lightGeo;
    [SerializeField] private Effect poisonEffect;

    public LightData LightData => data;

    [SerializeField] private LayerMask mask;

    private UpgradeManager upgradeManager;
    private EffectsManager effectsManager;
    private Material material;
    private Ray ray;
    private Camera mainCamera;
    public RaycastHit hit;
    private Vector3 offset;
    Color baseColor = new Color32(251, 238, 127, 20);
    Color DOTColor = new Color32(253, 110, 110, 20);

    void Start()
    {
        upgradeManager = GameManager.Instance.UpgradeManager;
        effectsManager = EffectsManager.Instance;
        lightGeo.SetActive(false);
        mainCamera = Camera.main;
        ray = new Ray(transform.position, transform.forward);
        offset = lightGeo.transform.position - mainCamera.transform.position;
        Physics.Raycast(ray, out hit, Mathf.Infinity);
    }

    public void Initialize()
    {
        lightGeo.SetActive(true);
        material = lightGeo.GetComponentInChildren<MeshRenderer>().material;
        
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive && !GameManager.Instance.IsCutsceenActive)
        {
            if (effectsManager.ActiveEffectTypes.Contains(poisonEffect))
            {
                material.SetColor("Color_686BCB55", DOTColor);
            }
            else
            {
                material.SetColor("Color_686BCB55", baseColor);
            }
            ray = mainCamera.ScreenPointToRay(target.transform.position);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
            {
                offset = lightGeo.transform.position - mainCamera.transform.position;
                Debug.DrawLine(ray.origin, hit.point);
                float distance = hit.distance - offset.magnitude;

                lightGeo.transform.rotation = Quaternion.LookRotation(hit.point - lightGeo.transform.position);
                lightGeo.transform.localScale = new Vector3(upgradeManager.Radius, upgradeManager.Radius, distance * 0.6f);

            }

            Vector3 direction = hit.point - transform.position;

        }
        else if(!GameManager.Instance.IsGameActive)
        {
            lightGeo.SetActive(false);
        }
    }
}

