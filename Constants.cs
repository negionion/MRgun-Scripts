public static class Constants
{
    public const int uiWidth = 1280;
    public const int uiHeight = 720;
    public const string bleMicroBit = "MicroBit";
    public const string tagARCollider = "ARCoreCollider";
    public const string tagPlayer = "Player";
    public const string tagEnemy = "Enemy";
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
