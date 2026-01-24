using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum MovementLevel {
    Stationary,
    Movable,
    Moving
}

public class TextDescription : MonoBehaviour
{
    [HideInInspector] public MovementLevel movementLevel = MovementLevel.Movable;
    [HideInInspector] public string textName;
    [SerializeField] public string additionalDescription;

    [HideInInspector] public string soundDescription;
    public bool metaObj = false;

    public bool isPlayer = false;

    private bool added = false;

    private Vector3 centerCache;
    private Vector3 sizeCache;

    private GameObject player;
    void Start() {
        // Add null check to resolve the conflict with AutoTagger
        TextTree.Instance.AddToDescriptionList(this,ParentCount());

        // if the attached game object has an Audio Source, get the transcript or sounds description
        AudioSource audioSource = GetComponent<AudioSource>();

        // find player based on isPlayer boolean
        player = FindObjectsOfType<TextDescription>()
                        .Where(s => s.isPlayer)
                        .ToList()[0].gameObject;
    }

    public void AddToTree() {
        if (added) return;
        added = true;
        /*if (scripts != null)
        {
            scripts = gameObject.GetComponents<MonoBehaviour>();
        }*/
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();

        if (GetComponent<Renderer>())
            renderers.Add(GetComponent<Renderer>());

        if (renderers.Count > 0)
        {
            // Create an initial bounds that encapsulates the first renderer
            Bounds combinedBounds = renderers[0].bounds;

            // Iterate through the remaining renderers and expand the combined bounds
            for (int i = 1; i < renderers.Count; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            // Get the center and size of the combined bounding box
            centerCache = combinedBounds.center;
            sizeCache = combinedBounds.size;

            TextTree.Instance.AddNode(this,GetName(),movementLevel == MovementLevel.Moving || movementLevel == MovementLevel.Movable);
        }
        else
        {
            Debug.Log("No renderers found! object is " + gameObject.name);
            centerCache = transform.position;
            sizeCache = Vector3.zero;

            TextTree.Instance.AddNode(this, GetName(), movementLevel == MovementLevel.Moving || movementLevel == MovementLevel.Movable);
        }
    }

    public void UpdateTree(){
        if (!added) {AddToTree(); return;}
        TextTree.Instance.UpdateNode(this);
    }

    public string GetName() => string.IsNullOrEmpty(textName) ? gameObject.name : textName;
    public string GetDescription() {
        // This is where the Dynamic Information Retriever logic goes
        string temp = additionalDescription;
        Color? color = FindColor();
        if (color != null){
            temp += $" The color of this item has the HEX code {ColorUtility.ToHtmlStringRGB(color.Value)}.";
        }
        TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
        if (text != null){
            temp += $" The font size of the text on this item is {text.fontSize}";
        }
        Light light = GetComponent<Light>();
        if (light != null){
            temp += $" The intensity of the light source on this item is {light.intensity}";
        }

        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null){
            temp += $" For the audio source on this item,";
            if (audioSource.mute){
                temp += $" it is muted,";
            } else{
                temp += $" it is not muted,";
            }
            temp += $" the volume is {audioSource.volume}";
            temp += $" the pitch is {audioSource.pitch}";
            temp += $" the range is {audioSource.maxDistance}";
        }

        TextMeshProUGUI textElement = GetComponent<TextMeshProUGUI>();
        if (textElement != null){
            temp += $" the text on this item says: " + textElement.text;
        }


        List<string> messages = new List<string>()
        {
            "in front of player",
            "to the left of player",
            "to the right of player",
            "behind the player"
        };

        Transform playerTransform = player.transform;
        string directionalMessage;
        Debug.Log("player camera transform is facing"+ playerTransform.forward);
        if (playerTransform == null){
            throw new System.Exception("Player Not Found! Have you checked isPlayer boolean on the player object?");
        }
        else if (!metaObj){
            Vector3 directionToObject = (transform.position - playerTransform.position).normalized;
            Vector3 projectedPlayerH = Vector3.ProjectOnPlane(playerTransform.forward, Vector3.up).normalized;
            Vector3 projectedDirH = Vector3.ProjectOnPlane(directionToObject, playerTransform.up);
            float angleToTurnH = Vector3.SignedAngle(projectedPlayerH, projectedDirH, Vector3.up);

            float angleFromRight = Vector3.SignedAngle(playerTransform.right, directionToObject, playerTransform.forward);
            float angleFromUp    = Vector3.SignedAngle(playerTransform.up, directionToObject, playerTransform.forward);
            if ((angleToTurnH >=0 && angleToTurnH <= 60) || (angleToTurnH <= 0 && angleToTurnH >= -60)){
                directionalMessage = messages[0];
            } else if(angleToTurnH < -60 && angleToTurnH >= -120){
                directionalMessage = messages[1];
            } else if(angleToTurnH > 60 && angleToTurnH <= 120){
                directionalMessage = messages[2];
            } else{
                directionalMessage = messages[3];
            }
            temp += " This object is " + directionalMessage;
        } else if (isPlayer) {
            Vector3 player_front = playerTransform.forward;
            Vector3 player_back = -player_front;
            Vector3 player_right = playerTransform.right;
            Vector3 player_left = -player_right;
            temp += $"Player is facing {player_front}, player's backwards direction is {player_back}, player's leftward direction is {player_left}, and player's rightward direction is {player_right}.";
        }
        return temp;
    }
    public string[] GetScripts() {
        MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
        //string[] scriptNames = new string[scripts.Length];
        List<string> scriptNames = new List<string>();

        for (int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i] != null && scripts[i].GetType().Name.Length > 0)
            {
                scriptNames.Add(scripts[i].GetType().Name);
            }
        }
        return scriptNames.ToArray();
    }

    public (Vector3,Vector3) GetCenterAndSize() {
        if (movementLevel == MovementLevel.Stationary) return (centerCache,sizeCache);
        List<Renderer> renderers = GetComponentsInChildren<Renderer>().ToList();

        if (GetComponent<Renderer>())
            renderers.Add(GetComponent<Renderer>());

        if (renderers.Count > 0)
        {
            // Create an initial bounds that encapsulates the first renderer
            Bounds combinedBounds = renderers[0].bounds;

            // Iterate through the remaining renderers and expand the combined bounds
            for (int i = 1; i < renderers.Count; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            // Get the center and size of the combined bounding box
            Vector3 center = combinedBounds.center;
            Vector3 size = combinedBounds.size;

            return (center,size);
        }
        else 
        {
            return (transform.position,Vector3.zero);
        }
    }

    public (float, float) GetEmbodiedCoord(){
        // returns the direction of the object in accordance to the player and its distance to the player
        // first number is the angle in terms of how many degrees to the left/right
        // second number is the angle in terms of how many degrees to the up/down
        // third number is the distance of the object to the player.

        // firts, get the tranform of the player by searching the object named "Main Camera" in the scene.
        Transform playerTransform = player.transform;
        if (playerTransform == null){
            throw new System.Exception("Player Not Found! Did you make sure to check IsPlayer boolean on the player object?");
        }
        else{
            Vector3 directionToObject = (transform.position - playerTransform.position).normalized;
            // Vector3 projectedDir = Vector3.ProjectOnPlane(directionToObject, playerTransform.up);
            // float yawAngle = Vector3.SignedAngle(playerTransform.forward, projectedDir, playerTransform.up);
            // projectedDir = Vector3.ProjectOnPlane(directionToObject, playerTransform.right);
            // float pitchAngle = Vector3.SignedAngle(playerTransform.forward, projectedDir, -playerTransform.right);
            float rotation = Vector3.Angle(playerTransform.forward, directionToObject);
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            return (rotation, distance);
        }
    }

    private int ParentCount() {
        int count = 0;
        Transform t = transform;
        while (t != null) {
            count++;
            t = t.parent;
        }
        return count;
    }

    private Color? FindColor(){
        // if the attached game object has a "simple" material, get the hex code
        // Get the Renderer component attached to this GameObject
        Renderer renderer = GetComponent<Renderer>();
        // Check if the Renderer exists and has a material
        if (renderer != null && renderer.material != null)
        {
            Material material = renderer.material;

            // Log the color
            if (material.HasProperty("_MainTex") && material.GetTexture("_MainTex") != null)
            {
                Texture2D texture = renderer.material.GetTexture("_MainTex") as Texture2D;

                if (texture != null)
                {
                    // Ensure the texture is readable
                    if (!texture.isReadable)
                    {
                        // Debug.Log($"The texture of {textName} is not readable.");
                        return null;
                    }

                    // Get all pixels from the texture
                    Color[] pixels = texture.GetPixels();

                    // Calculate the average color
                    Color averageColor = GetAverageColor(pixels);

                    // Debug.Log($"Average Color of {textName}: " + ColorUtility.ToHtmlStringRGB(averageColor));
                    return averageColor;
                }
                else
                {
                    // Debug.Log("No texture assigned to _MainTex.");
                    return null;
                }
            }
            else
            {
                Color materialColor = renderer.material.color;
                // Debug.Log($"Material color of {textName}: " + ColorUtility.ToHtmlStringRGB(materialColor));
                return materialColor;
            }
        }
        else
        {
            // Debug.Log($"This GameObject {textName} does not have a material.");
            return null;
        }
    }

    private Color GetAverageColor(Color[] pixels)
    {
        float r = 0, g = 0, b = 0, a = 0;
        int totalPixels = pixels.Length;

        foreach (Color pixel in pixels)
        {
            r += pixel.r;
            g += pixel.g;
            b += pixel.b;
            a += pixel.a;
        }

        return new Color(r / totalPixels, g / totalPixels, b / totalPixels, a / totalPixels);
    }
}
