using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    #region Variables
    public static ResourceManager Instance;

    [SerializeField] private int wood = 0;
    [SerializeField] private int stone = 0;
    
    #endregion

    #region Unity Methods
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        UiManager.Instance.UpdateUiWood();
        UiManager.Instance.UpdateUiStone();
    }
    #endregion

    #region Functions

    public bool CanPurchase(int woodAmount, int stoneAmount)
    {

        //True(Enought resources) false(GetGood/NotEnought)
        if (Wood >= woodAmount && Stone >= stoneAmount)
        {
            RemoveWood(woodAmount);
            RemoveStone(stoneAmount);

            UiManager.Instance.UpdateUiWood();
            UiManager.Instance.UpdateUiStone();
            return true;
        }
        else
        {
            return false; 
        }

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
}
