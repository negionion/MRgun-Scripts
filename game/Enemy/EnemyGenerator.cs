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
    
    public float distance = 2f;
    public float interval = 5f; //生成怪物間隔時間
    public float scanScale = 1f;    //Enemy size
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
                if(hitDistance >= distance && hitDistance <= distance * 10)
                {
                    Vector3 pos = scanAround(hit.point);
                    if(pos != Vector3.zero)
                        createEnemy(pos);
                }
                else
                {

                }

            }  
            yield return new WaitForSeconds(Random.Range(interval, interval * 2)); 
        }
    }

    private Vector3 scanAround(Vector3 center)
    {
        Vector3 ans = Vector3.zero, tmp = center;
        RaycastHit hit;
        Collider[] scanOverlap = new Collider[3];
        Vector3[] scanPosDelta = new Vector3[5];
        scanPosDelta[0].Set(0, scanScale, 0);
        scanPosDelta[1].Set( scanScale,  0,  scanScale);
        scanPosDelta[2].Set( scanScale,  0, -scanScale);
        scanPosDelta[3].Set(-scanScale,  0,  scanScale);
        scanPosDelta[4].Set(-scanScale,  0, -scanScale);
        //---------------------------
        //scanText.text = "collide " + FirstPersonCamera.transform.position + "\n";

        //---------------------------
        foreach(Vector3 delta in scanPosDelta)
        {
            tmp = center + delta;
            //scanText.text += tmp + " : ";
            if(Physics.Raycast(tmp, FirstPersonCamera.transform.position - tmp, out hit, 100))
            {
                //scanText.text += hit.transform.tag + "\n";  //test
                if(hit.transform.tag == "Player")
                {                    
                    //scanText.text += Physics.OverlapSphereNonAlloc(tmp, 0.35f, scanOverlap) + "\n";  //test
                    //檢測座標周圍是否直接卡到碰撞體內
                    if(Physics.OverlapSphereNonAlloc(tmp, scanScale * 0.35f, scanOverlap) == 0)
                    {
                        //檢測座標正下方有無地板，有地板才生成
                        if(Physics.OverlapBoxNonAlloc(tmp - new Vector3(0, scanScale, 0), new Vector3(scanScale * 0.35f, scanScale, scanScale * 0.35f), scanOverlap) > 0)
                        {
                            ans = tmp;
                            //debugText.text += "\n位置 " + tmp;
                            break;
                        }
                    }
                }
            }
            //scanText.text += "\n";
        }

        return ans;
    }

    private void createEnemy(Vector3 pos)
    {
        if(FindObjectsOfType<Enemy>().Length < 10)
            Instantiate(enemyPrefab[0], pos, Quaternion.identity);
    }
}
