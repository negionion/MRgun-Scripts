public static class Constants
{
    public const int uiWidth = 1920;
    public const int uiHeight = 1080;
    public const string bleMicroBit = "MicroBit";
    public const string tagARCollider = "ARCoreCollider";
    public const string tagARPlane = "ARCorePlane";
    public const string layerMRcollider = "MRcollider";
    public const string tagPlayer = "Player";
    public const string tagEnemy = "Enemy";
    public const string tagNetPlayer = "NetPlayer";
    public const string nameLocalPlayer = "LocalPlayer";
    public const uint nullNetId = 999999;
    

}


public static class GeneralFunc
{
    public static void Swap<T>(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }
}
