using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

public enum DataType {
    PixelList, String }

public class Networking {
    private TcpClient client;
    private NetworkStream networkStream;

    public Networking(string ip, int port) {
        client = new TcpClient(ip, port);
        networkStream = client.GetStream();
    }

    public void SendArrayToServer(List<PixelStruct> arrayToSend) {
        int dataSize = sizeof(float) * 5;
        byte[] dataToSend = new byte[arrayToSend.Count * dataSize];

        for (int i = 0; i < arrayToSend.Count; i++) {
            Buffer.BlockCopy(arrayToSend[i].ToArray(), 0, dataToSend, i * dataSize, dataSize);
        }

        byte[] header = BitConverter.GetBytes((int)DataType.PixelList);
        networkStream.Write(header, 0, header.Length);
        networkStream.Write(dataToSend, 0, dataToSend.Length);
    }

    public void SendMessageToServer(string stringToSend) {
        byte[] header = BitConverter.GetBytes((int)DataType.String);
        byte[] dataToSend = Encoding.UTF8.GetBytes(stringToSend);
        networkStream.Write(header, 0, header.Length);
        networkStream.Write(dataToSend, 0, dataToSend.Length);
    }

    public object ReceiveArrayFromServer()
    {
        byte[] headerBytes = new byte[sizeof(int)];
        networkStream.Read(headerBytes, 0, headerBytes.Length);

        DataType dataType = (DataType)BitConverter.ToInt32(headerBytes, 0);
        if (dataType == DataType.PixelList) {
            const int bufferSize = 1024;
            byte[] receivedData = new byte[bufferSize];
            int bytesRead = networkStream.Read(receivedData, 0, receivedData.Length);
            int numberOfPixels = bytesRead / (sizeof(float) * 5);
            List<PixelStruct> receivedPixels = new List<PixelStruct>();

            for (int i = 0; i < numberOfPixels; i++) {
                float[] pixelData = new float[5];
                Buffer.BlockCopy(receivedData, i * sizeof(float) * 5, pixelData, 0, sizeof(float) * 5);

                PixelStruct receivedPixel = new PixelStruct(pixelData[0], pixelData[1], pixelData[2], pixelData[3], pixelData[4]);
                receivedPixels.Add(receivedPixel);
            }

            return receivedPixels;
        } else if (dataType == DataType.String) {
            const int bufferSize = 1024;
            byte[] receivedData = new byte[bufferSize];
            int bytesRead = networkStream.Read(receivedData, 0, receivedData.Length);
            return Encoding.UTF8.GetString(receivedData, 0, bytesRead);
        }
        return "null";
    }

    public void CloseConnection() {
        networkStream.Close();
    }
}
