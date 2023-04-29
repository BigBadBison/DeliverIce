using UnityEditor.SceneManagement;

[System.Serializable]
public struct HexCoordinates {

    public readonly int Q;

    public readonly int R;

    public readonly int S;

    public HexCoordinates (int q, int r) {
        Q = q;
        R = r;
        S = -Q - R;
    }
    
    public override int GetHashCode () {
        return Q * 100 + R;
    }
}