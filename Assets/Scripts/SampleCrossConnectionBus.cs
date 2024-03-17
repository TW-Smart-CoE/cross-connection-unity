using CConn;
using UnityEngine;

public class SampleCrossConnectionBus : SceneSingleton<SampleCrossConnectionBus>
{
    private const uint DETECT_FLAG = 0xfffe1234;
    private readonly IBus bus = ConnectionFactory.CreateBus();
    private readonly int count = 0;

    private void OnDestroy() {
        bus.StopAll();
        bus.Cleanup();
    }

    protected override void Awake() {
        base.Awake();

        bus.SetLogger(LogManager.Instance);
        if (!bus.Initialize())
        {
            LogManager.Instance.Error("Initialize failed");
        }
    }

    private void Start()
    {
        if (!bus.Start(
            ConnectionType.TCP,
            ConfigProps.Create()
                .Put(PropKeys.PROP_PORT, 11001)
                .Put(PropKeys.PROP_RECV_BUFFER_SIZE, 8192),
            ConfigProps.Create()
                .Put(PropKeys.PROP_FLAG, DETECT_FLAG)
                .Put(PropKeys.PROP_SERVER_PORT, 11001)
                .Put(PropKeys.PROP_BROADCAST_PORT, 12000)
                .Put(PropKeys.PROP_BROADCAST_INTERVAL, 3000)
                .Put(PropKeys.PROP_BROADCAST_DATA, DataConverter.StringToBytes("hello data"))
                .Put(PropKeys.PROP_BROADCAST_DEBUG_MODE, true)
        ))
        {
            LogManager.Instance.Error("Start bus failed");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            bus.ResetRegister(
                ConnectionType.TCP,
                ConfigProps.Create()
                    .Put(PropKeys.PROP_FLAG, DETECT_FLAG)
                    .Put(PropKeys.PROP_SERVER_PORT, 11001)
                    .Put(PropKeys.PROP_BROADCAST_PORT, 12000)
                    .Put(PropKeys.PROP_BROADCAST_INTERVAL, 3000)
                    .Put(PropKeys.PROP_BROADCAST_DATA, DataConverter.StringToBytes("hello data next"))
                    .Put(PropKeys.PROP_BROADCAST_DEBUG_MODE, true)
            );
        }
    }
}
