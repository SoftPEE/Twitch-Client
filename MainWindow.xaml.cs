using System;
using System.Windows;
using System.Windows.Threading;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace Twitch_Clint001
{
  /// <summary>
  /// Interaktionslogik für MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private TwitchClient client;
    //private static readonly string ClientID = deine ClientID;

    private string name;
    private string oauth;
    private string channel;

    DispatcherTimer timer;

    private string timerText;
    private int timerValue;
    private int timerToEvent;

    private static readonly int maxChatListObject = 50;

    public MainWindow()
    {
      InitializeComponent();
      globalSetClientID();
      InitTimer();
    }

    private void InitTimer()
    {
      timer = new DispatcherTimer();
      timer.Tick += Timer_Tick;
      timer.Interval = new TimeSpan(0, 0, 1);  //Intervall auf 1 Sekunde festlegen

      tblockTimeSinceEnd.Text = "Timer: ";

    }

    private void Timer_Tick(object sender, EventArgs e)
    {
      timerToEvent--;

      tblockTimeSinceEnd.Text = "Timer: " + timerToEvent;

      if (timerToEvent <= 0)
      {
        client.SendMessage(timerText);
        timerToEvent = timerValue;
      }
    }

    //Connecten zum Client
    private void connect_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        name = tbName.Text.ToLower();
        oauth = tbOAuth.Text.ToLower();
        channel = tbChannel.Text.ToLower();
        

        if (name == "")
          throw new ApplicationException("Kein Name eingetragen!");
        if (oauth == "")
          throw new ApplicationException("Keine OAuthentication eingetragen!");
        if (channel == "")
          throw new ApplicationException("Keinen Channel eingetragen!");

        client = new TwitchClient(new ConnectionCredentials(name, oauth));

        //Twitch eigene Events registrieren
        RegisterTwitchEvents();

        Chat.Items.Insert(0, "Connecting...");
        controlListMemory();

        client.Connect();
        client.JoinChannel(channel);
      }
      catch (ApplicationException exept)
      {
        Chat.Items.Insert(0, exept.Message.ToString());
        controlListMemory();
      }
      
    }

    //Wird bei Connecten/Disconnecten verwendet um die Steuerelemente entsprechend des Zustandes zu aktiviert/deaktiviert
    void stateUserControl(bool state)
    {
      btSendMessage.IsEnabled         = state;
      tbSendManuellMessage.IsEnabled  = state;
      btAutoTextStart.IsEnabled       = state;
      tbTime.IsEnabled                = state;
      tbAutoText.IsEnabled            = state;
      btDisconnect.IsEnabled          = state;
      btConnect.IsEnabled             = !state;
      tbName.IsEnabled                = !state;
      tbOAuth.IsEnabled               = !state;
      tbChannel.IsEnabled             = !state;
    }

    private void disconnect_Click(object sender, RoutedEventArgs e)
    {
      client.LeaveChannel(channel);
      client.Disconnect();
      client = null;
      Chat.Items.Insert(0, "Disconnecting...");
      controlListMemory();
    }
    
    private void RegisterTwitchEvents()
    {
      client.OnConnected          += client_OnConnected;
      client.OnDisconnected       += Client_OnDisconnected;
      client.OnJoinedChannel      += client_OnJoinedChannel;
      client.OnClientLeftChannel  += Client_OnClientLeftChannel;
      client.OnMessageReceived    += Client_OnMessageReceived;
      client.OnMessageSent        += Client_OnMessageSent;
      client.OnConnectionError    += Client_OnConnectionError;
    }

    private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
    {
      this.Dispatcher.Invoke(() => Chat.Items.Insert(0, "Connection Error!"));
      this.Dispatcher.Invoke(() => controlListMemory());
    }

    private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
    {
      this.Dispatcher.Invoke(() => Chat.Items.Insert(0, e.SentMessage.DisplayName.ToString() + " : " + e.SentMessage.Message.ToString()));
      this.Dispatcher.Invoke(() => controlListMemory());
    }

    private void Client_OnClientLeftChannel(object sender, OnClientLeftChannelArgs e)
    {
      this.Dispatcher.Invoke(() => Chat.Items.Insert(0, "Client has leaved the Channel " + channel));
      this.Dispatcher.Invoke(() => controlListMemory());
    }

    private void Client_OnDisconnected(object sender, OnDisconnectedArgs e)
    {
      if (e.Username.ToString() == name)
      {
        this.Dispatcher.Invoke(() => Chat.Items.Insert(0, "Client is disconnected from mIRC Server"));
        this.Dispatcher.Invoke(() => stateUserControl(false));
        this.Dispatcher.Invoke(() => controlListMemory());
      }
     
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
      this.Dispatcher.Invoke(() => Chat.Items.Insert(0, e.ChatMessage.DisplayName.ToString() + " : " + e.ChatMessage.Message.ToString()));
      this.Dispatcher.Invoke(() => controlListMemory());
    }

    private void globalSetClientID()
    {
      TwitchApi.SetClientId(ClientID);
    }

    private void client_OnConnected(object sender, OnConnectedArgs e)
    {
      if (e.Username.ToString() == name)
      {
        this.Dispatcher.Invoke(() => Chat.Items.Insert(0, "Client is connected to mIRC Server"));
        this.Dispatcher.Invoke(() => stateUserControl(true));
        this.Dispatcher.Invoke(() => controlListMemory());
      }
     
    }

    private void client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
      this.Dispatcher.Invoke(() => Chat.Items.Insert(0, "Client has joined the Channel " + channel));
      this.Dispatcher.Invoke(() => controlListMemory());
    }

    //Start Automatisches Senden im Interval
    private void btAutoTextStart_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        timerValue = Convert.ToInt32(tbTime.Text.ToString());
        timerText = tbAutoText.Text.ToString();

        if (timerText == "")
          throw new ApplicationException("AutoText - Es wurde kein Text eingegeben!");

        timerToEvent = timerValue;
        timer.Start();

        tblockTimeSinceEnd.Text = "Timer: " + timerToEvent;

        btAutoTextStart.IsEnabled = false;
        btAutoTextStopp.IsEnabled = true;

        client.SendMessage(timerText);
      }
      catch (FormatException except)
      {
        Chat.Items.Insert(0, except.Message.ToString() + " - Falsches Zahlenformat eingegeben!");
        controlListMemory();
      }
      catch (ApplicationException except)
      {
        Chat.Items.Insert(0, except.Message.ToString());
        controlListMemory();
      }
      finally
      {
        tbAutoText.Text = "";
        tbTime.Text = "";
      }
    }

    //Stoppt Automatisches Senden im Interval
    private void btAutoTextStopp_Click(object sender, RoutedEventArgs e)
    {
      timer.Stop();

      tblockTimeSinceEnd.Text = "Timer: ";

      timerValue    = 0;
      timerToEvent  = 0;

      btAutoTextStart.IsEnabled = true;
      btAutoTextStopp.IsEnabled = false;
    }

    private void btSendMessage_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        string tempMessage = tbSendManuellMessage.Text.ToString();
        if (tempMessage == "")
          throw new ApplicationException("ManuellText - Es wurde kein Text eingegeben!");

        client.SendMessage(tbSendManuellMessage.Text.ToString());
      }
      catch (ApplicationException except)
      {
        Chat.Items.Insert(0, except.Message.ToString());
        controlListMemory();
      }
      finally
      {
        tbSendManuellMessage.Text = "";
      }
      
    }

    //Löscht bei Überschreitung der max erlaubten Listenelemente das letzte Element.
    void controlListMemory()
    {
      int size = Chat.Items.Count;
      if (size >= maxChatListObject)
      {
        Chat.Items.RemoveAt(Chat.Items.Count - 1);
      }
    }
  }
}