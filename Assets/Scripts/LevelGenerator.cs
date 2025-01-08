using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelGenerator : MonoBehaviour
{
    enum BitType {Wall, Inside, Outside};

    public string level_layouts_dir;
    public string transition_layouts_dir;

    public GameObject player;
    public Transform player_camera;
    public GameObject top_wall;
    public GameObject bottom_wall;
    public GameObject floor;
    public GameObject outside;
    public GameObject entrance_bars;
    public GameObject entrance_floor;

    private int offset = 0;
    private int prev_level_index = -1;

    private string[] level_layout_files;
    private string[] transition_layout_files;

    private int size = 1;

    private int current_room = 0;
    private Queue<GameObject> room_gameobjects = new Queue<GameObject>();
    private int num_active_rooms = 5;
    private bool is_transition = true;

    private float max_distance = 128f;


    Texture2D LoadPNG(string filePath) {
        Texture2D tex = null;

        if (File.Exists(filePath)) {
            var fileContent = File.ReadAllBytes(filePath);
            tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.LoadImage(fileContent);
        }
        return tex;
    }

    Vector2 GetPosition(int i, int j, int width, int height) {
        float x = (float)(i - width/2)*size + (float)size/2.0f;
        float y = (float)(j - height/2)*size + (float)size/2.0f;
        return new Vector2(x, y);
    }

    GameObject BuildRoom(Texture2D levelBitmap, string name) {
        GameObject room = new GameObject(name);
        room.SetActive(false);

        int texWidth = levelBitmap.width;
        int texHeight = levelBitmap.height;

        offset += texHeight/2;

        // Floor
        GameObject floor_instance = Instantiate(floor);
        floor_instance.transform.parent = room.transform;
        Vector2 tileSize = new Vector2((float)texWidth, (float)texHeight);
        SpriteRenderer spriteRenderer = floor_instance.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.material != null) {
            spriteRenderer.size = tileSize;
        }

        // Bitmap
        BitType[,] bitmap = new BitType[texWidth+2, texHeight+2];
        
        // "Outside" buffer on edges
        for (int i = 0; i < bitmap.GetLength(0); i++) {
            bitmap[i, 0] = BitType.Outside;
            bitmap[i, bitmap.GetLength(1)-1] = BitType.Outside;
        }

        for (int j = 0; j < bitmap.GetLength(1); j++) {
            bitmap[0, j] = BitType.Outside;
            bitmap[bitmap.GetLength(0)-1, j] = BitType.Outside;
        }

        for (int i = 0; i < texWidth; i++) {
            for (int j = 0; j < texHeight; j++)
            { 
                Color pixel = levelBitmap.GetPixel(i, j);
                var rgb = ((int)(pixel.r*255), (int)(pixel.g*255), (int)(pixel.b*255));
                
                // print(rgb);
                switch(rgb) {
                    case (0, 0, 0):
                        bitmap[i+1, j+1] = BitType.Wall;
                        break;
                    case (255, 255, 255):
                        bitmap[i+1, j+1] = BitType.Inside;
                        break;
                    case (237, 28, 36):
                        break;
                    default:
                        bitmap[i+1, j+1] = BitType.Outside;
                        break;
                }
            }
        }

        // Walls

        for (int i = 1; i < texWidth+1; i++) {
            for (int j = 1; j < texHeight+1; j++) {
                Vector2 pos = GetPosition(i-1, j-1, texWidth, texHeight);
                switch(bitmap[i, j]) {
                    case BitType.Wall: {  
                        GameObject prefab = bottom_wall;
                        var down = bitmap[i, j-1];
                        if(down == BitType.Inside) {
                            prefab = top_wall;
                        }
                        GameObject instance = Instantiate(prefab, pos, Quaternion.identity);
                        instance.transform.parent = room.transform;
                        break;
                    }
                    case BitType.Outside: {
                        GameObject instance = Instantiate(outside, pos, Quaternion.identity);
                        instance.transform.parent = room.transform;
                        break;
                    }
                    default:
                        break;
                }
            }
        }

        // Entrance Bars
        GameObject entrance_bars_instance = Instantiate(entrance_bars, new Vector3(0, -texHeight/2, 0), Quaternion.identity);
        entrance_bars_instance.transform.parent = room.transform;

        room.transform.position = new Vector3(0, offset, 0);
        room.SetActive(true);
        
        offset += texHeight/2;

        return room;
    }

    GameObject LoadRandomTransition(string name) {
        int transition_index = Random.Range(0, transition_layout_files.Length/2)*2;
        string randomFile = transition_layout_files[transition_index];

        Texture2D levelBitmap = LoadPNG(randomFile);
        if (levelBitmap == null) {
            Debug.LogError("Failed to load texture from file: " + randomFile);
            return null;
        }

        GameObject room = BuildRoom(levelBitmap, name);
        return room;
    }

    GameObject LoadRandomLevel(string name) {
        int level_index = -1;
        do {
            level_index = Random.Range(0, level_layout_files.Length/2)*2;
        } while(level_index == prev_level_index);
        string randomFile = level_layout_files[level_index];

        Texture2D levelBitmap = LoadPNG(randomFile);
        if (levelBitmap == null) {
            Debug.LogError("Failed to load texture from file: " + randomFile);
            return null;
        }

        prev_level_index = level_index;

        GameObject room = BuildRoom(levelBitmap, name);
        return room;
    }

    // Start is called before the first frame update
    void Start()
    {
        offset = (int)entrance_floor.transform.position.y;

        level_layout_files = Directory.GetFiles(level_layouts_dir);
        if(level_layout_files.Length == 0) {
            Debug.Log("No Level Layouts in directory");
            return;
        }
        transition_layout_files = Directory.GetFiles(transition_layouts_dir);
        if(transition_layout_files.Length == 0) {
            Debug.Log("No Transition Layouts in directory");
            return;
        }
        GameObject transition1 = LoadRandomTransition("Transition");
        room_gameobjects.Enqueue(transition1);
        GameObject level1 = LoadRandomLevel("Level");
        room_gameobjects.Enqueue(level1);
    }

    void Update()
    {
        if(player.transform.position.y >= max_distance) {
            if(entrance_floor) {
                Destroy(entrance_floor);
            }

            Vector3 reset = new Vector3(0, max_distance, 0);
            player_camera.position -= reset;
            player.transform.position -= reset;
            foreach (GameObject room in room_gameobjects) {
                room.transform.position -= reset;
            }

            offset -= (int)max_distance;
        }   
    }

    public int IncrementRoom() {
        current_room++;
        GameObject room;
        if(is_transition) {
            room = LoadRandomTransition("Transition");
        } else {
            room = LoadRandomLevel("Level");
        }
        room_gameobjects.Enqueue(room);
        is_transition = !is_transition;

        while(room_gameobjects.Count > num_active_rooms) {
            GameObject room_to_delete = room_gameobjects.Dequeue();
            Destroy(room_to_delete);
        }

        return current_room;
    }
}