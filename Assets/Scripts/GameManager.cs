using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI flagsLeftText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onWin;
    [SerializeField] private UnityEvent onGameOver;
    
    private int timePassed;
    private float secondsPassed;
    
    private int bombAmount;
    private int flagAmount;
    private int revealedAmount;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        onGameStart.Invoke();
        
        bombAmount = GridCreator.Instance.BombAmount;
        timeText.text = timePassed.ToString();
        SetFlagsLeftText();
    }
    
    private void Update()
    {
        secondsPassed += Time.deltaTime;
        
        while (secondsPassed >= 1)
        {
            --secondsPassed;
            ++timePassed;
            timeText.text = timePassed.ToString();
        }
    }

    public void IncreaseFlagAmount()
    {
        ++flagAmount;
        SetFlagsLeftText();
    }
    
    public void DecreaseFlagAmount()
    {
        --flagAmount;
        SetFlagsLeftText();
    }
    
    private void SetFlagsLeftText() => flagsLeftText.text = (bombAmount - flagAmount).ToString();
    
    public void IncreaseRevealedAmount()
    {
        ++revealedAmount;

        int nonBombCells = GridCreator.Instance.CellsAmount - bombAmount;
        
        if (revealedAmount == nonBombCells)
        {
            onWin.Invoke();
        }
    }

    public void ResetValues()
    {
        timePassed = 0;
        flagAmount = 0;
        revealedAmount = 0;
        
        SetFlagsLeftText();
        timeText.text = timePassed.ToString();
    }
    
    public void GameOver()
    {
        onGameOver.Invoke();
    }
}
