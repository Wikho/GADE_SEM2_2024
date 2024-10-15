using System.Collections;
using UnityEngine;
using TMPro;

public class ResourceTower : MonoBehaviour
{
    #region Variables

    [Header("Wood Generation Settings")]
    [SerializeField] private bool generateWood = true;
    [SerializeField] private float woodGenerationInterval = 8f; //Time interval for wood generation
    [SerializeField] private int minWoodAmount = 2; //Minimum wood amount generated
    [SerializeField] private int maxWoodAmount = 5; //Maximum wood amount generated

    [Header("Stone Generation Settings")]
    [SerializeField] private bool generateStone = true;
    [SerializeField] private float stoneGenerationInterval = 10f; //Time interval for stone generation
    [SerializeField] private int minStoneAmount = 1; //Minimum stone amount generated
    [SerializeField] private int maxStoneAmount = 4; //Maximum stone amount generated

    [Header("UI Settings")]
    [SerializeField] private GameObject woodUI;
    [SerializeField] private GameObject stoneUI;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text stoneText;

    private Coroutine woodCoroutine;
    private Coroutine stoneCoroutine;

    #endregion

    #region Unity Methods

    private void Start()
    {
        if (generateWood)
            woodCoroutine = StartCoroutine(GenerateWood());

        if (generateStone)
            stoneCoroutine = StartCoroutine(GenerateStone());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Functions

    private IEnumerator GenerateWood()
    {
        while (generateWood)
        {
            yield return new WaitForSeconds(woodGenerationInterval);

            if (EnemySpawnSettings.Instance.waveHaveStarted)
            {
                int woodAmount = Random.Range(minWoodAmount, maxWoodAmount + 1);
                ResourceManager.Instance.AddWood(woodAmount);
                UpdateUI(woodUI, woodText, woodAmount);
                UiManager.Instance.UpdateUiWood();
            }
        }
    }

    private IEnumerator GenerateStone()
    {
        while (generateStone)
        {
            yield return new WaitForSeconds(stoneGenerationInterval);

            if (EnemySpawnSettings.Instance.waveHaveStarted)
            {
                int stoneAmount = Random.Range(minStoneAmount, maxStoneAmount + 1);
                ResourceManager.Instance.AddStone(stoneAmount);
                UpdateUI(stoneUI, stoneText, stoneAmount);
                UiManager.Instance.UpdateUiStone();
            }
        }
    }

    private void UpdateUI(GameObject uiObject, TMP_Text uiText, int amount)
    {
        uiObject.SetActive(true);
        uiText.text = $"+ {amount}";
        StartCoroutine(HideUIAfterDelay(uiObject, 2f));
    }

    private IEnumerator HideUIAfterDelay(GameObject uiObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        uiObject.SetActive(false);
    }

    public void ToggleWoodGeneration(bool status)
    {
        generateWood = status;
        if (generateWood && woodCoroutine == null)
            woodCoroutine = StartCoroutine(GenerateWood());
        else if (!generateWood && woodCoroutine != null)
        {
            StopCoroutine(woodCoroutine);
            woodCoroutine = null;
        }
    }

    public void ToggleStoneGeneration(bool status)
    {
        generateStone = status;
        if (generateStone && stoneCoroutine == null)
            stoneCoroutine = StartCoroutine(GenerateStone());
        else if (!generateStone && stoneCoroutine != null)
        {
            StopCoroutine(stoneCoroutine);
            stoneCoroutine = null;
        }
    }

    #endregion
}
