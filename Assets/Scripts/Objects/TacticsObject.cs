using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 본 게임 내부의 모든 오브젝트 컨트롤 클래스들의 최상위 부모 클래스
public abstract class TacticsObject : MonoBehaviour
{
    public virtual void Interactive(TacticsObject interactTarget) { }

    public virtual bool OnTouchEvent(eTouchEvent touch) { return true; }

    // TODO : 오른쪽 왼쪽 텍스쳐 uv가 이상한 듯.. 확인 요망
    protected void SetCubeTexture(AtlasDataList atlasData, string imageName)
    {
        var imageData = atlasData.GetImageData(imageName);

        this.GetComponent<Renderer>().sharedMaterial = atlasData.GetMaterial();

        var mesh = this.GetComponent<MeshFilter>().mesh;

        var uv = new Vector2[24];

        var leftTop = new Vector2(imageData.offsetX, imageData.offsetY + imageData.scaleY);
        var rightTop = new Vector2(imageData.offsetX + imageData.scaleX, imageData.offsetY + imageData.scaleY);
        var leftBottom = new Vector2(imageData.offsetX, imageData.offsetY);
        var rightBottom = new Vector2(imageData.offsetX + imageData.scaleX, imageData.offsetY);

        uv[2] = leftTop;
        uv[3] = rightTop;
        uv[0] = leftBottom;
        uv[1] = rightBottom;

        uv[6] = leftTop;
        uv[7] = rightTop;
        uv[10] = leftBottom;
        uv[11] = rightBottom;

        uv[19] = leftTop;
        uv[17] = rightTop;
        uv[16] = leftBottom;
        uv[18] = rightBottom;

        uv[23] = leftTop;
        uv[21] = rightTop;
        uv[20] = leftBottom;
        uv[22] = rightBottom;

        uv[4] = leftTop;
        uv[5] = rightTop;
        uv[8] = leftBottom;
        uv[9] = rightBottom;

        uv[15] = leftTop;
        uv[13] = rightTop;
        uv[12] = leftBottom;
        uv[14] = rightBottom;

        mesh.uv = uv;
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
