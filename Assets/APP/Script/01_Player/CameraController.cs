using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//=======================================
//カメラ処理スクリプト(回転と上下の限界値)
//=======================================
public class CameraController : MonoBehaviour
{
    //変数定義
    public Vector3 offset = new Vector3(0f, 3f, -5f);
    public float sensitivity = 500f;
    public float pitchMin = -20f;
    public float pitchMax = 45f;

    private float yaw = 0f;
    private float pitch = 0f;

    public float CurrentYaw => yaw;

    public bool canRotate = true;
    [SerializeField] public Transform player;

    public GameObject[] prevRaycast;
    public List<GameObject> raycastHitsList_ = new List<GameObject>();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //カメラ回転処理
        if (!canRotate) return;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //カメラ上下限界固定
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.position = player.position + rotation * offset;
        transform.LookAt(player.position + Vector3.up * 1.0f);


        //=============================================
        //レイキャストで障害物を半透明化
        //=============================================
        //オブジェクト間のベクトル
        Vector3 _difference = (player.transform.position - this.transform.position);
        //正規化
        Vector3 _direction = _difference.normalized;
        // 距離
        float _distance = _difference.magnitude;

        // Ray(開始地点,進む方向)
        Ray _ray = new Ray(this.transform.position, _direction);
        
        // プレイヤーまでの距離分だけレイを飛ばす
        RaycastHit[] rayCastHits =  Physics.RaycastAll(_ray, _distance);

        Debug.Log($"Raycast hits: {rayCastHits.Length}, Distance: {_distance}");

        prevRaycast = raycastHitsList_.ToArray();
        raycastHitsList_.Clear();
        
        foreach (RaycastHit hit in rayCastHits)
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}, Tag: {hit.collider.tag}");
            
            if (hit.collider.CompareTag("transparent"))
            {
                SampleMaterial sampleMaterial = hit.collider.GetComponent<SampleMaterial>();
                
                if (sampleMaterial != null)
                {
                    Debug.Log($"Making transparent: {hit.collider.gameObject.name}");
                    sampleMaterial.ClearMaterialInvoke();
                    if (!raycastHitsList_.Contains(hit.collider.gameObject))
                    {
                        raycastHitsList_.Add(hit.collider.gameObject);//hitしたgameobjectを追加する
                    }
                }
                else
                {
                    Debug.LogWarning($"SampleMaterial component not found on: {hit.collider.gameObject.name}");
                }
            }
        }
        foreach (GameObject _gameObject in prevRaycast.Except<GameObject>(raycastHitsList_))
        {
            if (_gameObject != null)
            {
                SampleMaterial noSampleMaterial = _gameObject.GetComponent<SampleMaterial>();
                if (noSampleMaterial != null)
                {
                    Debug.Log($"Restoring opacity: {_gameObject.name}");
                    noSampleMaterial.NotClearMaterialInvoke();
                }
            }
        }
    }

}


