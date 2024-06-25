using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameSever.Protocol;



public class Main : SingletonMonoBehaviour<Main>
{
    public float updateSize;
    public string updateSizeString;
    public GameObject updatePanel;

    public GameObject updateInfo;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI subPromptText;
    public Button updateButton;

    //登陆面板
    public GameObject loginPanel;
    public TextMeshProUGUI loginPromptText;
    public Button loginButton;
    public Button openSignupPanelButton;
    public TMP_InputField loginAccount;
    public TMP_InputField loginPassword;

    //注册面板
    public GameObject signupPanel;
    public TextMeshProUGUI signupPromptText;
    public string lastSignup;
    public Button signupButton;
    public Button openLoginPanelButton;
    public TMP_InputField signupName;
    public TMP_InputField signupAccount;
    public TMP_InputField signupPassword;
    public TMP_InputField signupConfirmPassword;
    
    //过渡页面
    public GameObject loadingPanel;
    
    private int index;
    
    protected override void Awake()
    {
        if (UserDataManager.Instance == null)
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        loadingPanel.SetActive(false);
        
        base.Awake();
    }

    private void Start()
    {
        if (!UserDataManager.Instance.first)
        {
            loginPanel.SetActive(false);
            signupPanel.SetActive(false);
            updatePanel.SetActive(true);
            subPromptText.text = "";
            promptText.text = "检查更新中";
            updateButton.gameObject.SetActive(false);
            StartCoroutine(ABUpdateManager.Instance.DownloadABCompareInfo());
            UserDataManager.Instance.first = true;
        }
        else
        {
            loginPanel.SetActive(true);
            signupPanel.SetActive(false);
            updatePanel.SetActive(false);
            loginButton.onClick.AddListener(Login);
            openSignupPanelButton.onClick.AddListener(OpenRegisterPanel);
        }
    }

    void Update()
    {
        if (ABUpdateManager.Instance.checkOver && !ABUpdateManager.Instance.updateOver && index == 0)
        {
            index++;
            updateButton.gameObject.SetActive(true);
            if (ABUpdateManager.Instance.downLoadList.Count == 0)
            {
                promptText.text = "已经是最新版";
                subPromptText.text = "";
                updateInfo.SetActive(true);
                updateButton.GetComponentInChildren<TextMeshProUGUI>().text = "确定";
                updateButton.onClick.AddListener(UpdateLoginEvent);
            }
            else
            {
                promptText.text = "有可更新内容";
                //显示待下载大小
                updateSize = ABUpdateManager.Instance.totalSize;
                if (updateSize < 1024f)
                    updateSizeString = updateSize.ToString("0.00") + "B";
                else if (updateSize >= 1024f && updateSize < 1024f * 1024f)
                    updateSizeString = (updateSize / 1024f).ToString("0.00") + "KB";
                else if (updateSize > 1024f * 1024f)
                    updateSizeString = (updateSize / (1024f * 1024f)).ToString("0.00") + "MB";
                subPromptText.text = "更新大小为：" + updateSizeString;

                updateInfo.SetActive(true);
                updateButton.GetComponentInChildren<TextMeshProUGUI>().text = "更新";

                updateButton.onClick.AddListener(() =>
                {
                    if (!ABUpdateManager.Instance.beginUpdate)
                    {
                        //开始下载对比文件
                        StartCoroutine(ABUpdateManager.Instance.DownloadABFile());
                        ABUpdateManager.Instance.beginUpdate = true;
                    }

                    promptText.text = "更新中";
                    subPromptText.text = "";
                    updateButton.onClick.AddListener(() => { });
                });
            }
        }
        
        if (ABUpdateManager.Instance.updateOver && index == 1)
        {
            promptText.text = "更新完成";
            updateButton.GetComponentInChildren<TextMeshProUGUI>().text = "确定";

            updateButton.onClick.AddListener(UpdateLoginEvent);
            index++;
        }

        if (loginPanel.activeSelf)
        {
            if (loginAccount.isFocused && Input.GetKeyDown(KeyCode.Tab))
                loginPassword.ActivateInputField();
            else if (loginPassword.isFocused && Input.GetKeyDown(KeyCode.Tab))
                loginAccount.ActivateInputField();
            else if (!loginPassword.isFocused && !loginAccount.isFocused && Input.GetKeyDown(KeyCode.Tab))
                loginAccount.ActivateInputField();

            if (Input.GetKeyDown(KeyCode.Return))
                Login();
        }
        
        if (signupPanel.activeSelf)
        {
            if (signupName.isFocused && Input.GetKeyDown(KeyCode.Tab))
                signupAccount.ActivateInputField();
            else if (signupAccount.isFocused && Input.GetKeyDown(KeyCode.Tab))
                signupPassword.ActivateInputField();
            else if (signupPassword.isFocused && Input.GetKeyDown(KeyCode.Tab))
                signupConfirmPassword.ActivateInputField();
            else if (signupConfirmPassword.isFocused && Input.GetKeyDown(KeyCode.Tab))
                signupName.ActivateInputField();
            else if (!signupName.isFocused && !signupAccount.isFocused && !signupPassword.isFocused && !signupConfirmPassword.isFocused && Input.GetKeyDown(KeyCode.Tab))
                signupName.ActivateInputField();

            if (Input.GetKeyDown(KeyCode.Return))
                Register();
            if (Input.GetKeyDown(KeyCode.Escape))
                OpenLoginPanel();
        }

        
    }

    public void UpdateLoginEvent()
    {
        StartCoroutine( NetworkManager.Instance.CheckConnect());
        OpenLoginPanel();
        openSignupPanelButton.onClick.AddListener(OpenRegisterPanel);
        loginButton.onClick.AddListener(Login);
    }

    public void OpenLoginPanel()
    {
        loginAccount.text = "";
        loginPassword.text = "";
        loginPromptText.text = "";
        loginPanel.SetActive(true);
        updatePanel.SetActive(false);
        signupPanel.SetActive(false);
    }

    public void OpenRegisterPanel()
    {
        signupAccount.text = "";
        signupPassword.text = "";
        signupName.text = "";
        signupConfirmPassword.text = "";
        signupPromptText.text = "";
        signupPanel.SetActive(true);
        openLoginPanelButton.onClick.AddListener(OpenLoginPanel);
        signupButton.onClick.AddListener(Register);
    }
    
    /// <summary>
    /// 注册
    /// </summary>
    public void Register()
    {
        if (!String.IsNullOrWhiteSpace(lastSignup) && lastSignup == signupAccount.text)
            return;
        if (signupName.text.Length < 2 || signupName.text.Length > 8)
        {
            signupPromptText.text = "用户名长度需要2~8个字母以内";
            return;
        }
        if (signupAccount.text.Length < 4 || signupAccount.text.Length > 8)
        {
            signupPromptText.text = "账号长度需要4~8个字母以内";
            return;
        }
        if (signupPassword.text.Length < 4 || signupPassword.text.Length > 8)
        {
            signupPromptText.text = "密码长度需要4~8个字母以内";
            return;
        }
        if (signupPassword.text != signupConfirmPassword.text)
        {
            signupPromptText.text = "输入密码不一致";
            return;
        }
        lastSignup = signupAccount.text;
        SignupProto proto = new SignupProto(signupName.text,signupAccount.text,signupPassword.text);
        NetworkManager.Instance.SendTcpMsg(proto.ToArray());
        
        signupButton.onClick.RemoveAllListeners();
        signupButton.onClick.AddListener(() => { });
    }
    
    /// <summary>
    /// 登陆
    /// </summary>
    public void Login()
    {
        if (loginAccount.text.Length < 4 || loginAccount.text.Length > 8)
        {
            loginPromptText.text = "账号长度需要4~8个字母以内";
            return;
        }
        if (loginPassword.text.Length < 4 || loginPassword.text.Length > 8)
        {
            loginPromptText.text = "密码长度需要4~8个字母以内";
            return;
        }
        if (!NetworkManager.Instance.IsConnect())
        {
            loginPromptText.text = "连接服务器失败";
            return;
        }
        
        loadingPanel.SetActive(true);
        LoginProto loginProto = new LoginProto(loginAccount.text,loginPassword.text);
        NetworkManager.Instance.SendTcpMsg(loginProto.ToArray());
        UserDataManager.Instance.ownerInfo.admin = loginAccount.text;
        updatePanel.SetActive(false);
        
        loginButton.onClick.RemoveAllListeners();
        loginButton.onClick.AddListener(() => { });
    }
}