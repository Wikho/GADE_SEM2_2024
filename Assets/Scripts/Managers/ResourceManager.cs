using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    #region Variables
    public static ResourceManager Instance;

    private int wood = 0;
    private int stone = 0;


    #endregion

    #region Unity Methods
    private void Awake()
    {
        Singleton();
    }

    #endregion

    #region Functions

    private bool CanPurchase(int woodAmount, int stoneAmount)
    {
        //True(Enought resources) false(GetGood/NotEnought)
        return (Wood >= woodAmount && Stone >= stoneAmount);
    }

    #endregion

    #region GetSet

    //Wood
    public int Wood
    {
        get { return wood; }
        private set { wood = value; }
    }
    public void AddWood(int amount) {wood += amount;}
    public void RemoveWood(int amount) { wood -= amount; }

    //Stone
    public int Stone
    {
        get { return stone; }
        private set { stone = value; }
    }
    public void AddStone(int amount) { stone += amount; }
    public void RemoveStone(int amount) { stone -= amount; }


    #endregion

    #region Singleton
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Another instance of ResourceManager was destroyed on creation!");
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
