using UnityEngine;
using UnityEngine.UI;
using Basic3D;

public class UpdateBar : MonoBehaviour
{
    #region SharingSettings
    [SerializeField] private bool isShared = false;
    private BaseState dataSource;                          //当前的数据源
    [SerializeField] private SharedType sharedType;        //从目标数据源取数据的凭证
    private float preCurrValue;                            //主动检测变化
    private CanvasGroup m_CanvasGroup;

    /// <summary>
    /// 更改数据源
    /// </summary>
    /// <param name="newSrc">新的数据源</param>
    public void ChangeDataSrc(BaseState newSrc, float newCurr, float newMax)
    {
        dataSource = newSrc;
        if (LinearTarget > -0.5f) LinearTarget = -1f;

        MaxSlashValue = newMax;
        currSlashValue = newCurr;
        preCurrValue = currSlashValue;
    }
    #endregion

    private float linearTarget = -1f;                                           //线性变化目标

    [SerializeField] private bool makeEffect = true;                           //使用特效

    /// <summary>
    /// 获取/设置--线性变化的终点
    /// </summary>
    public float LinearTarget {
        get {
            return linearTarget < -0.5f ? currSlashValue : linearTarget;
        }
        set {
            linearTarget = FormatSlashValue(value);
            if (makeEffect)
            {
                if (linearTarget > currSlashValue) effect.fillAmount = linearTarget / maxSlashValue;
                else fill.fillAmount = linearTarget / maxSlashValue;
                effect.gameObject.SetActive(true);
            }
        }
    }

    private float value = 1;
    /// <summary>
    /// （高级）设置百分比进度
    /// </summary>
    public float PercentValue {
        get { return value; }
        set {
            currSlashValue = value * maxSlashValue;
        }
    }

    public enum DisplayMode
    {
        Null,
        SingleValue,
        Percentage,
        Slash,
    }

    public DisplayMode displayMode = DisplayMode.Null;

    [SerializeField] private float maxSlashValue = 100;
    /// <summary>
    /// 获取/设置进度最大值
    /// </summary>
    public float MaxSlashValue {
        get { return maxSlashValue; }
        set {
            maxSlashValue = value;
            lineSpeed = lineSpeed_percentage * maxSlashValue;
        }
    }

    public float currSlashValue = 100;

    [SerializeField] private float lineSpeed_percentage = 1f;          //每秒线性变化的速度（百分比）
    private float lineSpeed;
    private float slashRatePerFrame;

    private Transform mask;
    private Image fill, effect;
    private Text valueText;
    private Vector2 effectPos;

    private void Awake()
    {
        mask = transform.Find("mask");
        fill = mask.Find("fill").GetComponent<Image>();
        effect = mask.Find("effect").GetComponent<Image>();
        effect.gameObject.SetActive(false);

        valueText = transform.Find("valueText").GetComponent<Text>();

        lineSpeed = lineSpeed_percentage * maxSlashValue;      //to slashValue
        if (isShared) m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (isShared)
        {
            if (dataSource)
            {
                m_CanvasGroup.alpha = 1;
                float _curr = dataSource.GetData(sharedType);
                if (_curr != preCurrValue) LinearTarget = _curr;
                preCurrValue = _curr;
            }
            else
            {
                m_CanvasGroup.alpha = 0;
            }
        }

        if (linearTarget > -0.5f)
        {
            slashRatePerFrame = Time.deltaTime * lineSpeed;
            if (Mathf.Abs(linearTarget - currSlashValue) < slashRatePerFrame)
            {
                currSlashValue = linearTarget;
                linearTarget = -1f;
                if (makeEffect) effect.gameObject.SetActive(false);
            }
            else
            {
                if (linearTarget > currSlashValue) currSlashValue += slashRatePerFrame;
                else currSlashValue -= slashRatePerFrame;
            }
        }
        SetBar();
    }

    private void SetBar()
    {
        currSlashValue = FormatSlashValue(currSlashValue);
        value = currSlashValue / maxSlashValue;

        switch (displayMode)
        {
            case DisplayMode.Null: break;
            case DisplayMode.Percentage: valueText.text = (value * 100).ToString("0") + "%"; break;
            case DisplayMode.Slash: valueText.text = currSlashValue.ToString("0") + "/" + maxSlashValue.ToString("0"); break;
            case DisplayMode.SingleValue: valueText.text = ((int)currSlashValue).ToString(); break;
        }

        if (linearTarget > -0.5f)
        {
            if (makeEffect)
            {
                if (linearTarget > currSlashValue) fill.fillAmount = value;
                else effect.fillAmount = value;
            }
            else fill.fillAmount = value;
        }
        else fill.fillAmount = value;
    }

    public void Init(Vector2 curr_max)
    {
        MaxSlashValue = curr_max.y;
        currSlashValue = curr_max.x;
    }


    /// <summary>
    /// 按百分比增加进度
    /// </summary>
    /// <param name="x"></param>
    public void AddPercentValue(float x)
    {
        currSlashValue += x * maxSlashValue;
    }

    /// <summary>
    /// 格式化给定的currSlashValue
    /// </summary>
    /// <param name="x"></param>
    public float FormatSlashValue(float x)
    {
        if (x < 0f) x = 0f;
        else if (x > maxSlashValue) x = maxSlashValue;
        return x;
    }

}

