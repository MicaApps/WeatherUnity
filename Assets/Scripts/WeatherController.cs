/* by nnsdldy (nnsdldy@sina.com) 2025-01-04 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine;
//using UnityEngine.Windows;
#if ENABLE_WINMD_SUPPORT
using  Windows.Storage;
#endif

public class WeatherController : MonoBehaviour
{
    //����ģʽ
    /// <summary>
    /// ����������ʵ��
    /// </summary>
    public static WeatherController instance;
    private void Awake()
    {
        if (instance == true) Destroy(gameObject);
        instance = this;
    }

    private string _txtPath = @"AppData\Local\Packages\50c23970-b046-42f3-96a1-414427a9fd8f_bc58ztcjv32he\LocalState\StreamingAssets\WeatherData.txt";
    private string _fullPath;
    private float _timer = 0;
    private Dictionary<int, string> _weatherNumber_weatherAnim_dict = new Dictionary<int, string>();
    private Animator _weatherAnimator;
    private int _currentWeatherNumber = -1;
    //MoonContr�����WeatherController���������������������
    private MoonController _mc;

    private void Start()
    {
        _weatherAnimator = this.GetComponent<Animator>();
        _mc = GetComponentInChildren<MoonController>(true);

        /*//����·��
        _fullPath = Path.Combine(Application.streamingAssetsPath, _txtPath);
        Debug.Log(_fullPath);
        Debug.Log(File.ReadAllText(_fullPath));*/

        //����·��
        string[] strs = Application.persistentDataPath.Split('/');
        if (strs.Length >= 3)
            _fullPath = $"{strs[0]}\\{strs[1]}\\{strs[2]}\\{_txtPath}";
        else
            _fullPath = "UserPath Error";
        
        ReadConfig();
    }
    private void FixedUpdate()
    {
        _timer += Time.fixedDeltaTime;
        if (_timer >= 1)
        {
            _timer = 0;
            int weatherNumber, moonNumber;
            char dayParam;
            ReadWeatherAndMoonNumber(out weatherNumber, out moonNumber, out dayParam);
            SetWeatherAnim(weatherNumber);
            SetMoon(moonNumber);
            SetDay(dayParam);
        }
    }
    /// <summary>
    /// ��ȡ��ǰ��������
    /// </summary>
    public string GetCurrentAnimation()
    {
        if (_weatherAnimator)
        {
            AnimatorClipInfo[] clipInfo = _weatherAnimator.GetCurrentAnimatorClipInfo(0);
            return clipInfo[0].clip.name;
        }
        else
        {
            return null;
        }

    }
    /// <summary>
    /// �ж�ĳ�����Ƿ����ڹ�����
    /// </summary>
    /// <param name="anmationName"></param>
    /// <returns></returns>
    public bool IsCurrentAnimatorState(string anmationName)
    {
        if (_weatherAnimator)
        {
            return _weatherAnimator.GetAnimatorTransitionInfo(0).IsName(anmationName);
        }
        else
        {
            return false;
        }

    }
    //��ȡ���������ļ�
    public void ReadConfig()
    {
        string configPath = $"{Application.streamingAssetsPath}\\config.txt";
        try
        {
            string str = System.IO.File.ReadAllText(configPath);
            string[] strArr = str.Split(",");
            int number;
            for (int i = 0; i < strArr.Length / 2; i++)
            {
                if (strArr.Length <= i * 2 + 1)
                    break;
                if (int.TryParse(strArr[i * 2], out number))
                {
                    _weatherNumber_weatherAnim_dict.Add(number, strArr[i * 2 + 1]);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"There is a problem when reading config.txt: {e}");
        }
    }
    //��ȡ������ϢTXT
    public void ReadWeatherAndMoonNumber(out int weatherNumber, out int moonNumber, out char DN)
    {
        int num0 = 44;
        int num1 = 1;
        char num2 = '0';
        
        try
        {
            var str = "31,3,N";
            //_fullPath = UnityEngine.Windows.Directory.localFolder+ "\\WeatherData.txt";
            //#if ENABLE_WINMD_SUPPORT
            //            _fullPath = ApplicationData.Current.LocalFolder + "\\WeatherData.txt";
            //#endif
            _fullPath = Application.persistentDataPath + "/StreamingAssets" +"/WeatherData.txt";//���������������û�У�������unity������Ƕ����ƣ�û��д�ģ����ǵģ�unity������ⲿ��txtһ����,�����text������ȥ����,
            //Ӧ��û�����ˣ����´��
            if (System.IO.File.Exists(_fullPath))
            {
                str = System.IO.File.ReadAllText(_fullPath);
                //�б���û�����ǣ�����������Ժ󣬲��÷���assets�ļ��У�ֱ�ӷŵ���exeͬ��Ŀ¼����
            }
            //��������������TXT�ö��ŷָ���������������
            string[] strArr = str.Split(',');
            //����Ҫ�ж������Ƿ�λ����ȷ��Χ�����ǵĻ�������Ϊ44����Ӧȱʡͼ�꣩
            if (strArr.Length >= 1)
            {
                if (!int.TryParse(strArr[0], out num0))
                    num0 = 44;
                if (num0 < 0 || num0 > 47)
                    num0 = 44;
            }
            //���಻�ж������Ƿ�λ����ȷ��Χ������MoonContr�ж�
            if (strArr.Length >= 2)
            {
                if (!int.TryParse(strArr[1], out num1))
                    num1 = 1;
            }
            //��ȡ��ҹ��Ϣ
            if (strArr.Length >= 3)
                num2 = char.Parse(strArr[2]);
        }
        catch (Exception e)
        {
            Debug.LogError($"There is a problem when reading WeatherData.txt: {e}");
        }
        weatherNumber = num0;
        moonNumber = num1;
        DN = num2;
    }
    public void SetWeatherAnim(int weatherNumber)
    {
        //����¾��������һ������������
        if (_currentWeatherNumber == weatherNumber)
            return;
        string currentWeatherAnim;
        //�ص���ǰ�����Ķ���
        //if(_weatherNumber_weatherAnim_dict.TryGetValue(weatherNumber, out currentWeatherAnim))
        //  _weatherAnimator.SetBool(currentWeatherAnim, false);
        //���������滻��ǰ����
        _currentWeatherNumber = weatherNumber;
        //�����������Ķ���
        if (_weatherNumber_weatherAnim_dict.TryGetValue(weatherNumber, out currentWeatherAnim))
            _weatherAnimator.SetTrigger(currentWeatherAnim);
    }
    public void SetMoon(int moonNumber)
    {
        //Debug.Log(_mc);
        if (_mc)
        {
            _mc.moon = moonNumber;
            _mc.UpdateSprite();
        }
    }
    public void SetDay(char dayParam)
    {

        if (dayParam == 'D' && _weatherAnimator.GetFloat("����") != 1)
            StartCoroutine(DayAnimation(0,1,0.25f));
        //_weatherAnimator.SetFloat("����", 1);
        else if (dayParam == 'N' && _weatherAnimator.GetFloat("����") != 0)
            StartCoroutine(DayAnimation(1,0,0.25f));
        //_weatherAnimator.SetFloat("����", 0);
    }

    private IEnumerator DayAnimation(float strat, float tag,float time)
    {
        //�����л���Ŀ�����
        float v = 0,num = 0,timer = 0;//�ӽ��̶�
        //ͨ������ʱ���������
        while (v < 1)
        {
            num = Mathf.Lerp(strat, tag, v);
            _weatherAnimator.SetFloat("����", num);
            timer += Time.deltaTime;
            v = timer / time;
            yield return 0;
        }
        _weatherAnimator.SetFloat("����", tag);
    }
}
