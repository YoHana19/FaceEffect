using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.iOS;

namespace FaceTrackingVFX
{
    sealed class ARFaceMeshBaker : MonoBehaviour
    {
        [SerializeField] RenderTexture _positionMap = default;
        [SerializeField] ComputeShader _vertexBaker = default;

        UnityARSessionNativeInterface _session;

        ComputeBuffer _positionBuffer;
        RenderTexture _tmpPositionMap;
        int _vertexCountID, _transformID, _positionBufferID, _positionMapID;

        [HideInInspector] public UnityEventArgInt GetVertexCount = new UnityEventArgInt();
        
        void Start()
        {
            // ComputeShader側に渡す各種パラメータ
            _vertexCountID = Shader.PropertyToID("VertexCount");
            _transformID = Shader.PropertyToID("Transform");
            _positionBufferID = Shader.PropertyToID("PositionBuffer");
            _positionMapID = Shader.PropertyToID("PositionMap");

            // 以下はARKit周りの初期化
            _session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

            Application.targetFrameRate = 60;
            var config = new ARKitFaceTrackingConfiguration();
            config.alignment = UnityARAlignment.UnityARAlignmentGravity;
            config.enableLightEstimation = true;

            if (config.IsSupported)
            {
                _session.RunWithConfig(config);
                UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdded;
                UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdated;
                UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemoved;
            }
        }

        // ComputeShaderに渡すバッファの生成
        void FaceAdded(ARFaceAnchor anchorData)
        {
            var vertexCount = anchorData.faceGeometry.vertices.Length;
            GetVertexCount.Invoke(vertexCount);
            _positionBuffer = new ComputeBuffer(vertexCount * 3, sizeof(float));
            _tmpPositionMap = _positionMap.Clone();
        }

        // 取得した頂点情報の書き込み
        void FaceUpdated(ARFaceAnchor anchorData)
        {
            if (_positionBuffer == null) return;

            var mapWidth = _positionMap.width;
            var mapHeight = _positionMap.height;
            var vCount = anchorData.faceGeometry.vertices.Length;

            // FaceGeometryから頂点とtransformの取得を行う
            _positionBuffer.SetData(anchorData.faceGeometry.vertices);
            gameObject.transform.localPosition = UnityARMatrixOps.GetPosition(anchorData.transform);
            gameObject.transform.localRotation = UnityARMatrixOps.GetRotation(anchorData.transform);

            // バッファに確保した頂点とtransformをComputeShaderにセットして実行。
            // ※transformはComputeSahder内で頂点に対するワールド変換を行う際に使用する。
            _vertexBaker.SetInt(_vertexCountID, vCount);
            _vertexBaker.SetMatrix(_transformID, gameObject.transform.localToWorldMatrix);
            _vertexBaker.SetBuffer(0, _positionBufferID, _positionBuffer);
            _vertexBaker.SetTexture(0, _positionMapID, _tmpPositionMap);

            // ComputeShaderの実行
            // 
            // Inspectorから指定される_positionMapのwidthとheightは「512 x 512」となるので、
            // ComputeShaderとしては「group(64, 64, 1)」「numthreads(8, 8, 1)」
            // → 「(64 * 8, 64 * 8, 1) → 512 x 512」のバッファに対する書き込みを行う事となる。
            _vertexBaker.Dispatch(0, mapWidth / 8, mapHeight / 8, 1);

            Graphics.CopyTexture(_tmpPositionMap, _positionMap);
        }

        void FaceRemoved(ARFaceAnchor anchorData)
        {
            Debug.Log("Face Removed");
            _positionBuffer.TryDispose();
            _tmpPositionMap.TryDestroy();
        }
    }
}