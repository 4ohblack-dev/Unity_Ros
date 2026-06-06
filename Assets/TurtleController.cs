using UnityEngine;
using UnityEngine.InputSystem; // 👈 新しい入力システムを使うために追加！
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class TurtleController : MonoBehaviour
{
    private ROSConnection ros;
    public string topicName = "/turtle1/cmd_vel";

    public float linearSpeed = 2.0f;
    public float angularSpeed = 2.0f;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<TwistMsg>(topicName);
    }

    void Update()
    {
        // 現在のキーボードの状態を取得
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // キーボードが認識されていなければ何もしない

        float moveInput = 0f;
        float turnInput = 0f;

        // 👈 新方式でのキー入力判定（矢印キーとWASDに対応）
        if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) moveInput = 1f;
        if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) moveInput = -1f;
        if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) turnInput = 1f;
        if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) turnInput = -1f;

        // 入力があった場合、ROS2にメッセージを送信
        if (moveInput != 0 || turnInput != 0)
        {
            SendTwistMessage(moveInput * linearSpeed, turnInput * angularSpeed);
        }
    }

    void SendTwistMessage(float linearX, float angularZ)
    {
        TwistMsg twist = new TwistMsg();

        RosMessageTypes.Geometry.Vector3Msg linearVel = new RosMessageTypes.Geometry.Vector3Msg();
        linearVel.x = linearX;

        RosMessageTypes.Geometry.Vector3Msg angularVel = new RosMessageTypes.Geometry.Vector3Msg();
        angularVel.z = angularZ;

        twist.linear = linearVel;
        twist.angular = angularVel;

        ros.Publish(topicName, twist);
    }
}