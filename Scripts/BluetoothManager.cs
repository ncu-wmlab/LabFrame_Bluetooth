using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using LabFrame2023;

#if UNITY_STANDALONE_WIN
// using System.IO.Ports;
#endif

public class BluetoothManager : LabSingleton<BluetoothManager>, IManager
{
#if UNITY_STANDALONE_WIN
    // private SerialPort stream = null; 
#elif UNITY_ANDROID
    private AndroidJavaClass _bluetoothPlugin;
    private AndroidJavaObject _bluetoothPluginInstance;
#endif


    /* -------------------------------------------------------------------------- */

    public void ManagerInit()
    {
#if UNITY_ANDROID
        if (Application.isEditor)
        {
            Debug.LogWarning("[Bluetooth] Android plugin is not available in Editor.");
            return;
        }
        _bluetoothPlugin = new AndroidJavaClass("com.jcxyis.unitybluetooth.BluetoothManager");
        _bluetoothPluginInstance = _bluetoothPlugin.CallStatic<AndroidJavaObject>("getInstance");

        CheckPermission();
#endif
    }
    
    public IEnumerator ManagerDispose()
    {
        Stop();
        yield break;
    }
   
    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Init permission.
    /// It is recommended to call this method in the first scene (before connect).
    /// </summary>
    public void CheckPermission()
    {
#if UNITY_ANDROID
        _bluetoothPluginInstance.Call("CheckPermission");
#endif
    }

    /// <summary>
    /// Start scanning nearby bluetooth devices
    /// </summary>
    public void StartDiscovery()
    {
#if UNITY_ANDROID
        _bluetoothPluginInstance.Call("StartDiscovery");
#endif
    }

    /// <summary>
    /// Get the list of nearby devices.
    /// Don't forget to call <c>StartDiscovery()</c> first!
    /// </summary>
    /// <returns></returns>
    public List<BluetoothDevice> GetAvailableDevices()
    {
#if UNITY_ANDROID
        string[] devices = _bluetoothPluginInstance.Call<string[]>("GetAvailableDevices");
        List<BluetoothDevice> deviceList = new List<BluetoothDevice>();
        string[] deviceRawStr;
        for(int i = 0; i < devices.Length; i++)
        {
            deviceRawStr = devices[i].Split('|');
            deviceList.Add(new BluetoothDevice
            {
                name = deviceRawStr[0],
                mac = deviceRawStr[1]
            });
        }
        return deviceList;
#else
        throw new PlatformNotSupportedException("[Bluetooth] GetAvailableDevices is not supported on this platform.");
#endif
    }

    
    /// <summary>
    /// 連接藍芽 (PC)
    /// </summary>
    /// <param name="COM">COM 幾 (串口ID)；可至裝置管理員查看</param>
    /// <param name="baudRate"></param>
    /// <returns></returns>
    public IEnumerator ConnectCOM(string COM, int baudRate = 9600) 
    {
#if UNITY_STANDALONE_WIN 
        // bool isConnected = false;
        // while(!isConnected) // 重複開啟serial port接口直到連線成功為止
        // {
        //     try
        //     {
        //         stream = new SerialPort("COM" + COM, baudRate);// 呼吸 9600 舌壓 12800 
        //         stream.Open(); // 開啟serial port接口，才能收資料
        //         isConnected = true;
        //     }
           
        //     catch (Exception e)
        //     {
        //         Debug.Log("[Bluetooth] Retry connecting... Reason="+e);
        //     }
        //     yield return 0;
        // }
        // Debug.Log("[Bluetooth] 已連接！");
        throw new PlatformNotSupportedException("[Bluetooth] PC: Not supported yet.");
#else            
        throw new PlatformNotSupportedException("[Bluetooth] This platform is not supported.");
#endif
    }

    /// <summary>
    /// Connect to a device with the specified address.
    /// </summary>
    /// <param name="mac">Address</param>
    /// <param name="pin">If the device hasn't been bonded and it has PIN, then this field is required.</param>
    /// <returns></returns>
    public bool Connect(string mac, string pin = "")
    {
#if UNITY_STANDALONE_WIN
        throw new PlatformNotSupportedException("[Bluetooth] PC: use ConnectCOM instead.");
#elif UNITY_ANDROID
        return _bluetoothPluginInstance.Call<bool>("Connect", mac, pin);            
#endif
    }

    /// <summary>
    /// Is Connected?
    /// </summary>
    public bool IsConnected()
    {
#if UNITY_STANDALONE_WIN
        throw new PlatformNotSupportedException("[Bluetooth] PC: Not supported yet.");
        // return stream != null && stream.IsOpen;
#elif UNITY_ANDROID
        return _bluetoothPluginInstance.Call<bool>("IsConnected");
#endif
    }

    /// <summary>
    /// Get connected device, if no device is connected, return "|".
    /// </summary>
    /// <returns>{NAME}|{MAC}</returns>
    public string GetConnectedDevice()
    {
#if UNITY_STANDALONE_WIN
        throw new PlatformNotSupportedException("[Bluetooth] PC: Not supported.");
#elif UNITY_ANDROID
        return _bluetoothPluginInstance.Call<string>("GetConnectedDevice");
#endif
    }

    /// <summary>
    /// Send data to the remote device.
    /// You need to append newline (e.g. '\n' or '\r\n') by yourself.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>    
    public bool Send(string data)
    {
#if UNITY_STANDALONE_WIN
        throw new PlatformNotSupportedException("[Bluetooth] PC: Not supported yet.");
        // if(!stream.IsOpen)
        //     return false;
        // stream.WriteLine(data);            
        // return true;
#elif UNITY_ANDROID            
        return _bluetoothPluginInstance.Call<bool>("Send", data);
#endif
    }

    /// <summary>
    /// Input stream buffer size
    /// </summary>
    /// <returns></returns>
    public int Available()
    {
#if UNITY_STANDALONE_WIN
        throw new PlatformNotSupportedException("[Bluetooth] PC: Not supported yet.");
        // return stream.BytesToRead;
#elif UNITY_ANDROID
        return _bluetoothPluginInstance.Call<int>("Available");            
#endif
    }

    /// <summary>
    /// Read a line from input stream
    /// </summary>
    /// <returns></returns>
    public string ReadLine()
    {
#if UNITY_STANDALONE_WIN
        throw new PlatformNotSupportedException("[Bluetooth] PC: Not supported yet.");
        // return stream.ReadLine();
#elif UNITY_ANDROID            
        return _bluetoothPluginInstance.Call<string>("ReadLine");
#endif
    }

    /// <summary>
    /// Stop the connection
    /// </summary>
    public void Stop()
    {
#if UNITY_STANDALONE_WIN
        throw new PlatformNotSupportedException("[Bluetooth] PC: Not supported yet.");
        // stream.Close();
        // stream = null;
#elif UNITY_ANDROID
        _bluetoothPluginInstance.Call("Stop");            
#endif
    }


}