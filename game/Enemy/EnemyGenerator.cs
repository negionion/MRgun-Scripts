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
    public int enemyMaxCount = 5;
    public float scanScale = 1f;    //Enemy size
    public GameObject enemyPrefab;
    [SerializeField]
    private Bounds enemyBounds;
    public Text debugText;
    public Text scanText;
    // Start is called before the first frame update
    void Start()
    {
        FirstPersonCamera = Camera.main;
        generateEnemyFlag = true;
        //enemyBounds = enemyCollider.bounds;
        float maxExtents = Mathf.Max(enemyBounds.extents.x, enemyBounds.extents.z);
        if(scanScale <= maxExtents)
        {
            scanScale = maxExtents * 2;
        }
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
            yield return new WaitForSeconds(interval); 
        }
    }

    private Vector3 scanAround(Vector3 center)
    {
        Vector3 ans = Vector3.zero, tmp = center;
        RaycastHit hit;
        Collider[] scanOverlap = new Collider[3];
        Vector3[] scanPosDelta = new Vector3[8];
        scanPosDelta[0].Set(0, 0, scanScale);
        scanPosDelta[1].Set(0, 0, -scanScale);
        scanPosDelta[2].Set(scanScale, 0, 0);
        scanPosDelta[3].Set(-scanScale, 0, 0);
        scanPosDelta[4].Set( scanScale,  0,  scanScale);
        scanPosDelta[5].Set( scanScale,  0, -scanScale);
        scanPosDelta[6].Set(-scanScale,  0,  scanScale);
        scanPosDelta[7].Set(-scanScale,  0, -scanScale);

        Vector3 scanCenterDelta = new Vector3(0, enemyBounds.size.y / 2, 0); //掃描下方有無地板的碰撞體中心
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
                if(hit.transform.tag == Constants.tagPlayer)
                {                    
                    //scanText.text += Physics.OverlapSphereNonAlloc(tmp, 0.3f, scanOverlap) + "\n";  //test
                    //檢測座標周圍是否直接卡到碰撞體內，提高掃描中心至敵人模型高度的25%位置，掃描半身範圍的圓球內有無碰撞
                    if(Physics.OverlapSphereNonAlloc(tmp + (scanCenterDelta / 2), (enemyBounds.size.y / 4), scanOverlap) == 0)
                    {
                        //檢測座標正下方有無地板，有地板才生成
                        //將座標的center移動到正下方(tmp - scanCenterDelta)，且不疊合生成的怪物碰撞體
                        if(Physics.OverlapBoxNonAlloc(tmp - scanCenterDelta, enemyBounds.extents * 0.6f, scanOverlap) == 1)
                        {
                            if(scanOverlap[0].tag == Constants.tagARCollider)
                            {
                                ans = tmp;                                    
                                return ans; //直接中斷送出目標位置點
                            }
                            /*foreach(Collider _collider in scanOverlap)   //檢查地板是否為ARcollider，而非其他敵人或玩家
                            {
                                if(!_collider)
                                    continue;
                                if(_collider.tag == Constants.tagARCollider)
                                {
                                    ans = tmp;                                    
                                    return ans; //直接中斷送出目標位置點
                                }
                            }*/
                        }
                    }
                }
            }
        }

        return ans;
    }

    private void createEnemy(Vector3 pos)
    {
        if(FindObjectsOfType<Enemy>().Length < enemyMaxCount)
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        else
        {
            Destroy(FindObjectsOfType<Enemy>()[enemyMaxCount - 1].gameObject);
            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }
}
