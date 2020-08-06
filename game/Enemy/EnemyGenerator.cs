using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    private Camera FirstPersonCamera;
    public static bool generateEnemyFlag {private set; get;} = false;   //是否生成怪物
    public float interval = 5f; //生成怪物間隔時間

    public GameObject[] enemyPrefab;
    public Text debugText;
    public Text scanText;
    // Start is called before the first frame update
    void Start()
    {
        FirstPersonCamera = Camera.main;
        generateEnemyFlag = true;
        StartCoroutine(generateHandler());
        //createEnemy(transform);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(new Vector3(0, 2, 5),FirstPersonCamera.transform.position - new Vector3(0, 2, 5), Color.red, 1f);
    }

    private IEnumerator generateHandler()
    {
        RaycastHit hit;
        
        Collider[] scanOverlap = new Collider[5];
        yield return new WaitForSeconds(5f);   //Delay

        while(generateEnemyFlag)
        {
            //刷新AR的虛擬環境碰撞體(深度感測)
            SingleObj<DepthMeshColliderCus>.instance.ScanDepthCollider();
            yield return new WaitForSeconds(0.3f);

            Ray ray = FirstPersonCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));        
            if(Physics.Raycast(ray, out hit, 100))  //距離3~20之間可以產生怪物
            {
                float hitDistance = Vector3.Distance(FirstPersonCamera.transform.position, hit.point);
                debugText.text = "距離 " + hitDistance.ToString();
                debugText.text += "\n點 " + hit.point;
                if(hitDistance >= 2 && hitDistance <= 20)
                {
                    Vector3 pos = scanAround(hit.point);
                    if(pos != Vector3.zero)
                        createEnemy(pos);
                }
                else
                {

                }

            }  
            yield return new WaitForSeconds(interval); 
        }
    }

    private Vector3 scanAround(Vector3 center)
    {
        Vector3 ans = Vector3.zero, tmp = center;
        float scanScale = 1f;
        RaycastHit hit;
        Collider[] scanOverlap = new Collider[3];
        Vector3[] scanPosDelta = new Vector3[5];
        scanPosDelta[0].Set(0, scanScale, 0);
        scanPosDelta[1].Set( scanScale,  0,  scanScale);
        scanPosDelta[2].Set( scanScale,  0, -scanScale);
        scanPosDelta[3].Set(-scanScale,  0,  scanScale);
        scanPosDelta[4].Set(-scanScale,  0, -scanScale);
        //---------------------------
        scanText.text = "collide " + FirstPersonCamera.transform.position + "\n";

        //---------------------------
        foreach(Vector3 delta in scanPosDelta)
        {
            tmp = center + delta;
            scanText.text += tmp + " : ";
            if(Physics.Raycast(tmp, FirstPersonCamera.transform.position - tmp, out hit, 100))
            {
                scanText.text += hit.transform.tag + "\n";  //test
                if(hit.transform.tag == "Player")
                {                    
                    scanText.text += Physics.OverlapSphereNonAlloc(tmp, 0.3f, scanOverlap) + "\n";  //test
                    if(Physics.OverlapSphereNonAlloc(tmp, 0.3f, scanOverlap) <= 1)
                    {
                        ans = tmp;
                        debugText.text += "\n位置 " + tmp;
                        break;
                    }
                }
            }
            scanText.text += "\n";
        }

        return ans;
    }

    private void createEnemy(Vector3 pos)
    {
        
        Instantiate(enemyPrefab[0], pos, Quaternion.identity);
    }
}
