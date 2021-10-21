using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Vivox;
using UnityEngine.Android;
using VivoxUnity;

public class VivoxInit : MonoBehaviour
{
    // Start is called before the first frame update
    private Client client;
    
    private System.Uri serverURI = new System.Uri("https://unity.vivox.com/appconfig/9708e-ovrse-68493-test");
    private string tokenKey = "tlwVhkgcuvcRzoH3hcXkhC7AUSqBSXV";
    private string domain = "mtu1xp.vivox.com";
    private string issuer = "9708e-ovrse-68493-test";

    private ILoginSession loginSession;
    private bool logged = false;
    
    private IChannelSession channelSession;
    private TimeSpan timeSpan = new TimeSpan(90);

    private string channelAliveName = "aliveChannel";
    private string channelDeadName = "deadChannel";

    private string channelNext = "";

    private Action autoLogChannel = null;
    private Action autoSwitchChannel = null;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        client = new Client();
        client.Initialize();
    }

    private void OnApplicationQuit()
    {
        client.Uninitialize();
    }
    
    public void Bind_Login_Callback_Listeners(bool bind, ILoginSession loginSesh)
    {
        if (bind)
        {
            loginSesh.PropertyChanged += Login_Status;
        }
        else
        {
            loginSesh.PropertyChanged -= Login_Status;
        }

    }
    
    public void Bind_Channel_Callback_Listeners(bool bind, IChannelSession channelSesh)
    {
        if (bind)
        {
            channelSesh.PropertyChanged += On_Channel_Status_Changed;
        }
        else
        {
            channelSesh.PropertyChanged -= On_Channel_Status_Changed;
        }
    }

    #region Login_Methode 

    


    public void LoginUser()
    {
        // For this example, client is initialized.
        var nickname = GetComponent<NetworkNickname>().nickname;
        var account = new AccountId(issuer, Guid.NewGuid().ToString(), domain, nickname);
        
        
        loginSession = client.GetLoginSession(account);
        Bind_Login_Callback_Listeners(true, loginSession);
        
        loginSession.BeginLogin(serverURI, loginSession.GetLoginToken(tokenKey, timeSpan), ar =>
        {
            try
            {
                loginSession.EndLogin(ar);
            }
            catch (Exception e)
            {
                Bind_Login_Callback_Listeners(true, loginSession);
                Debug.Log(e.Message);
                return;
            }
            // At this point, login is successful and other operations can be performed.
        });
    }
    
    public void Logout()
    {
        loginSession.Logout();
    }
    
    public void Login_Status(object sender, PropertyChangedEventArgs loginArgs)
    {
        var source = (ILoginSession)sender;
        
        switch (source.State)
        {
            case LoginState.LoggingIn:
                Debug.Log("Logging In");
                break;
            
            case LoginState.LoggedIn:
                Debug.Log($"Logged In {loginSession.LoginSessionId.Name}");
                if (autoLogChannel != null)
                {
                    autoLogChannel.Invoke();
                }
                break;
            case LoginState.LoggingOut:
                Debug.Log("Logging out");
                break;
            case LoginState.LoggedOut:
                Debug.Log("LoggedOut");
                Bind_Login_Callback_Listeners(false, source);
                break;
        }
    }


    #endregion

    #region Channel_Methods
    
    public void JoinChannel(string channelName, bool IsAudio, bool IsText, bool switchTransmission, ChannelType channelType)
    {
    #if PLATFORM_ANDROID || UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
    #endif


        ChannelId channelId = new ChannelId(issuer, channelName, domain, channelType);
        channelSession = loginSession.GetChannelSession(channelId);
        Bind_Channel_Callback_Listeners(true, channelSession);

        channelSession.BeginConnect(IsAudio, IsText, switchTransmission, channelSession.GetConnectToken(tokenKey, timeSpan), ar => 
        {
            try
            {
                channelSession.EndConnect(ar);
            }
            catch(Exception e)
            {
                Bind_Channel_Callback_Listeners(false, channelSession);
                Debug.Log(e.Message);
            }
        });
    }
    
    public void Leave_Channel(IChannelSession channelToDisconnect, string channelName)
    {
        channelToDisconnect.Disconnect();
        loginSession.DeleteChannelSession(new ChannelId(issuer, channelName, domain));
    }

    public void On_Channel_Status_Changed(object sender, PropertyChangedEventArgs channelArgs)
    {
        IChannelSession source = (IChannelSession)sender;

        switch (source.ChannelState)
        {
            case ConnectionState.Connecting:
                Debug.Log("Channel Connecting");
                break;    
            case ConnectionState.Connected:
                Debug.Log($"{source.Channel.Name} Connected");
                break;       
            case ConnectionState.Disconnecting:
                Debug.Log($"{source.Channel.Name} disconnecting");
                break;    
            case ConnectionState.Disconnected:
                Debug.Log($"{source.Channel.Name} disconnected");
                if (autoSwitchChannel != null)
                {
                    autoSwitchChannel.Invoke();
                }
                break;
        }
    }

    #endregion


    #region Call_Method

    public void ConnectChannel()
    {
        autoLogChannel = ChannelAutoLog;
        autoSwitchChannel = null;
        LoginUser();
    }

    private void ChannelAutoLog()
    {
        bool alive = GetComponent<State>().isPlaying();
        string channelName = alive ? channelAliveName : channelDeadName;
        JoinChannel(channelName, true, false, true, ChannelType.NonPositional);
    }

    public void SwitchChannel()
    {
        if (channelSession == null)
        {
            return;
        }
        bool alive = GetComponent<State>().isPlaying();
        string channelName = alive ? channelAliveName : channelDeadName;
        string channelNameNew = !alive ? channelAliveName : channelDeadName;
        Leave_Channel(channelSession, channelName);
        channelNext = channelNameNew;
        autoSwitchChannel = switchChannelName;
    }

    private void switchChannelName()
    {
        JoinChannel(channelNext, true, false, true, ChannelType.Positional);
    }

    public void Disconnect()
    {
        bool alive = GetComponent<State>().isPlaying();
        string channelName = alive ? channelAliveName : channelDeadName;
        Leave_Channel(channelSession, channelName);
        Logout();
    }

    public void SwitchConnexion(bool toConnect)
    {
        if (toConnect == logged)
        {
            return;
        }

        if (toConnect)
        {
            ConnectChannel();
        }
        else
        {
            Disconnect();
        }
    }
    


    #endregion
    
    
    
}