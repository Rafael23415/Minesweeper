using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI textMesh;
    private SpriteRenderer sr;
    
    
    [Header("<size=15>Sprites</size>")]
    [SerializeField] private SpriteRenderer symbolSpriteRenderer;
    
    [Header("Cell")]
    [SerializeField] private Sprite unrevealedSprite; 
    [SerializeField] private Sprite revealedSprite; 
    
    [Header("Symbols")]
    [SerializeField] private Sprite bombSprite; 
    [SerializeField] private Sprite flagSprite; 
    
    private int bombCount;
    private bool isBomb;
    private bool isRevealed;
    private bool hasFlag;
    
    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (!textMesh)
        {
            Debug.LogWarning("No TextMeshPro component found!");
            return;
        }
        
        sr = GetComponentInChildren<SpriteRenderer>();
        if (!sr)
        {
            Debug.LogWarning("No SpriteRenderer component found!");
            return;
        }
        
        if(!symbolSpriteRenderer)
        {
            Debug.LogWarning("Symbol SpriteRenderer not set!");
            return;
        }

        textMesh.text = "";
        sr.sprite = unrevealedSprite;
        symbolSpriteRenderer.sprite = null;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Normal reveal
            if (!isRevealed)
            {
                GridCreator.Instance.StartGrid(this);
                Reveal();
            }
            
            // Reveal when with all flags
            if (isRevealed && !isBomb && HasAllFlags())
            {
                RevealAround();
            }
        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Flag only not revealed
                if(!isRevealed)
                {
                    hasFlag = !hasFlag;
                    if (hasFlag)
                    {
                        symbolSpriteRenderer.sprite = flagSprite;
                        GameManager.Instance.IncreaseFlagAmount();
                    }
                    else
                    {
                        symbolSpriteRenderer.sprite = null;
                        GameManager.Instance.DecreaseFlagAmount();
                    }
                }
            }
        }
    }
    
    private void Reveal()
    {
        if (isRevealed || hasFlag)
        {
            return;
        }
        
        isRevealed = true;
        
        if (isBomb)
        {
            sr.sprite = revealedSprite;
            symbolSpriteRenderer.sprite = bombSprite;
            GameManager.Instance.GameOver();
        }
        else
        {
            sr.sprite = revealedSprite;
            GameManager.Instance.IncreaseRevealedAmount();
            
            if(bombCount == 0)
            {
                RevealAround();
            }
            else
            {
                switch (bombCount)
                {
                    case 1:
                        textMesh.color = Color.blue;
                        break;
                    case 2:
                        textMesh.color = Color.green;
                        break;
                    case 3:
                        textMesh.color = Color.red;
                        break;
                    case 4:
                        textMesh.color = Color.navyBlue;
                        break;
                    case 5:
                        textMesh.color = Color.darkRed;
                        break;
                    case 6:
                        textMesh.color = Color.darkCyan;
                        break;
                    case 7:
                        textMesh.color = Color.black;
                        break;
                    case 8:
                        textMesh.color = Color.gray6;
                        break;
                }
                textMesh.text = bombCount.ToString();
            }
        }
    }
    
    private bool HasAllFlags()
    {
        if (isRevealed)
        {
            var cells = GridCreator.Instance.GetAroundCells(this);
            int flagCount = 0;

            foreach (var cell in cells)
            {
                if(cell.hasFlag)
                {
                    flagCount++;
                }
            }

            return flagCount >= bombCount;
        }

        return false;
    }
    
    private void RevealAround()
    {
        var cells = GridCreator.Instance.GetAroundCells(this);
        
        foreach (var cell in cells)
        {
            cell.Reveal();
        }
    }
    
    public void Restart()
    {
        bombCount = 0;
        isBomb = false;
        isRevealed = false;
        hasFlag = false;
        
        textMesh.text = "";
        
        sr.sprite = unrevealedSprite;
        symbolSpriteRenderer.sprite = null;
    }
    
    public void AddBombCount() => ++bombCount;
    
    public void SetAsBomb() => isBomb = true;
    public bool IsBomb => isBomb;
}
