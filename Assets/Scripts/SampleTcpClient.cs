using System;
using UnityEngine;
using CConn;

public class SampleTcpClient : SceneSingleton<SampleTcpClient>
{
    private const string TEST_TOPIC = "/execute_cmd_list";
    private const uint DETECT_FLAG = 0xfffe1234;

    private int count = 0;

    private IConnection connection;
    private INetworkDetector detector;

    private void OnDataArrived(string topic, Method method, byte[] data)
    {
        LogManager.Instance.Info($"{topic} {method} {DataConverter.BytesToString(data)}");
    }

    private void OnConnectionStateChanged(ConnectionState connectionState, Exception exception)
    {
        LogManager.Instance.Info(connectionState);
        if (connectionState == ConnectionState.CONNECTED)
        {
            connection.Subscribe(TEST_TOPIC, Method.REQUEST, OnDataArrived);
        }
    }

    protected override void Awake()
    {
        base.Awake();             

        connection = ConnectionFactory.CreateConnection(ConnectionType.TCP);
        connection.SetLogger(LogManager.Instance);
        connection.ConnectionStateChanged += OnConnectionStateChanged;

        detector = ConnectionFactory.CreateNetworkDetector(NetworkDiscoveryType.UDP);
        detector.SetLogger(LogManager.Instance);
    }

    private void OnDestroy() {
        detector.StopDiscover();

        connection.Stop();
        connection.ConnectionStateChanged -= OnConnectionStateChanged;
    }

    private void Start()
    {
        detector.StartDiscover(
            ConfigProps.create()
                .Put(PropKeys.PROP_FLAG, DETECT_FLAG),
            (props) =>
            {
                detector.StopDiscover();

                var ip = props.Get(PropKeys.PROP_SERVER_IP, "");
                var port = props.Get(PropKeys.PROP_SERVER_PORT, 0);

                LogManager.Instance.Info($"Found {ip} {port}");

                if (ip != "" && port != 0)
                {
                    connection.Start(
                        ConfigProps.create()
                            .Put(PropKeys.PROP_IP, ip)
                            .Put(PropKeys.PROP_PORT, port)
                            .Put(PropKeys.PROP_AUTO_RECONNECT, true)
                            .Put(PropKeys.PROP_MAX_RECONNECT_RETRY_TIME, 8)
                            .Put(PropKeys.PROP_RECV_BUFFER_SIZE, 8192)
                    );
                }
            } 
        );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))     
        {
            if (connection.GetState() == ConnectionState.CONNECTED)
            {
                connection.Publish(
                    TEST_TOPIC,
                    Method.REQUEST,
                    DataConverter.StringToBytes($"data {count++}")
                    );
            }
        }
    }
}
