public struct PixelStruct {
    public float x, y, r, g, b;

    public PixelStruct(float nX, float nY, float nR, float nG, float nB) {
        (x, y, r, g, b) = (nX, nY, nR, nG, nB);
    }

    public float[] ToArray() {
        float[] dataToSendPackaged = { x, y, r, g, b };
        return dataToSendPackaged;
    }
}
