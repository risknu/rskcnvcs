using System.Net.Sockets;

public class Networking {

    private TcpClient client;
    private NetworkStream networkStream;

    public Networking(string ip, int port) {
        client = new TcpClient(ip, port);
        networkStream = client.GetStream();
    }

    public static bool CanConnect(string ip, int port) {
        try {
            using (var testClient = new TcpClient()) {
                testClient.Connect(ip, port);
                return true;
            }
        } catch { return false; }
    }

    public void SendArrayToServer(List<PixelStruct> arrayToSend) {
        int dataSize = sizeof(float) * 5;
        byte[] dataToSend = new byte[arrayToSend.Count * dataSize];

        for (int i = 0; i < arrayToSend.Count; i++) {
            Buffer.BlockCopy(arrayToSend[i].ToArray(), 0, dataToSend, i * dataSize, dataSize);
        }

        networkStream.Write(dataToSend, 0, dataToSend.Length);
    }

    public List<PixelStruct> ReceiveArrayFromServer() {
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
    }

    public void CloseConnection() {networkStream.Close();}
}
