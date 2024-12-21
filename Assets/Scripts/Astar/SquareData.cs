using TMPro;
using Unity.Mathematics;
using UnityEngine;

public enum GridType
{
    None,
    Target,
    Unit,
    path,
    Obs,
    Search,
}

public class SquareData : MonoBehaviour
{
    public int Index;

    public TMP_Text m_Text;

    private SpriteRenderer m_SpriteRenderer;

    public GridType _gridType;

    public int2 Point;

    public int Dis;

    public float G;

    public float H;

    public float F;

    private void Start()
    {
        m_SpriteRenderer =GetComponent<SpriteRenderer>();
    }

    public void ChangeGrid(GridType grid)
    {
        _gridType =grid;

        switch (grid)
        {
            case GridType.None:
                m_SpriteRenderer.color = Color.white;
                break;
            case GridType.Target:
                m_SpriteRenderer.color = Color.red;
                break;
            case GridType.Unit:
                m_SpriteRenderer.color = Color.yellow;
                break;
            case GridType.path:
                m_SpriteRenderer.color = Color.green;
                break;
            case GridType.Obs:
                m_SpriteRenderer.color = Color.blue;
                break;
            case GridType.Search:
                m_SpriteRenderer.color = Color.gray;
                break;
            default:
                break;
        }
    }

    public void SetText(Vector3 pos)
    {
        m_Text.text = $"{Dis} \n ({pos.x},{pos.y})";
    }

}
