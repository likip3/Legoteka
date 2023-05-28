using UnityEngine;

public struct RenderTask
{
    public RenderTexture texture;
    public GameObject gameObject;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Material material;

    /// <summary>
    /// The struct, which holds all the information to create the preview image.
    /// </summary>
    /// <param name="texture">The RenderTexture which receives the final image.</param>
    /// <param name="gameObject">The GameObject to create a preview from.</param>
    public RenderTask(RenderTexture texture, GameObject gameObject)
    {
        this.texture = texture;
        this.gameObject = gameObject;
        this.position = Vector3.zero;
        this.rotation = Quaternion.identity;
        this.scale = Vector3.one;
        this.material = gameObject.GetComponent<MeshRenderer>().material;
    }

    public RenderTask(RenderTexture texture, GameObject gameObject, Material material)
    {
        this.texture = texture;
        this.gameObject = gameObject;
        this.position = Vector3.zero;
        this.rotation = Quaternion.identity;
        this.scale = Vector3.one;
        this.material = material;
    }

    /// <summary>
    /// The struct, which holds all the information to create the preview image.
    /// </summary>
    /// <param name="texture">The RenderTexture which receives the final image.</param>
    /// <param name="gameObject">The GameObject to create a preview from.</param>
    /// <param name="position">The offset from the camera.</param>
    /// <param name="rotation">The rotation of the object.</param>
    public RenderTask(RenderTexture texture, GameObject gameObject, Vector3 position, Quaternion rotation)
    {
        this.texture = texture;
        this.gameObject = gameObject;
        this.position = position;
        this.rotation = rotation;
        this.scale = Vector3.one;
        this.material = null;
        if (gameObject.TryGetComponent<MeshRenderer>(out var temp))
        {
            this.material = temp.material;
        }
    }

    /// <summary>
    /// The struct, which holds all the information to create the preview image.
    /// </summary>
    /// <param name="texture">The RenderTexture which receives the final image.</param>
    /// <param name="gameObject">The GameObject to create a preview from.</param>
    /// <param name="position">The offset from the camera.</param>
    /// <param name="rotation">The rotation of the object.</param>
    /// <param name="scale">The scale of the object.</param>
    public RenderTask(RenderTexture texture, GameObject gameObject,
        Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.texture = texture;
        this.gameObject = gameObject;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.material = gameObject.GetComponent<MeshRenderer>().material;
    }
}