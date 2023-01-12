using CConn;

public struct Msg
{
    public int value;
}

public class SampleCrossConnectionBus : SceneSingleton<SampleCrossConnectionBus>
{
    private const uint DETECT_FLAG = 0xfffe1234;
    private IBus bus = ConnectionFactory.CreateBus();
    private int count = 0;

    private void OnDestroy() {
        bus.StopAll();
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
            ConfigProps.create()
                .Put(PropKeys.PROP_PORT, 11001)
                .Put(PropKeys.PROP_RECV_BUFFER_SIZE, 8192),
            ConfigProps.create()
                .Put(PropKeys.PROP_FLAG, DETECT_FLAG)
                .Put(PropKeys.PROP_SERVER_PORT, 11001)
                .Put(PropKeys.PROP_BROADCAST_PORT, 12000)
                .Put(PropKeys.PROP_BROADCAST_INTERVAL, 3000)
        ))
        {
            LogManager.Instance.Error("Start bus failed");
        }
    }
}
