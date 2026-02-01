using GameLoop;
using LevelEditor.UI;
using Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LevelEditor
{
    public enum EEditorMode
    {
        NONE,
        PLACE,
        SELECT
    }

    [System.Serializable]
    public class LevelItem
    {
        public Vector2 Position => m_position;
        public float Rotation => m_rotation;
        public int ID => m_id;

        [SerializeField]
        private Vector2 m_position;
        [SerializeField]
        private float m_rotation;
        [SerializeField]
        private int m_id;

        public LevelItem(Vector2 _position, float _rotation, int _id)
        {
            m_position = _position;
            m_rotation = _rotation;
            m_id = _id;
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public List<LevelItem> Items => m_items;

        [SerializeField]
        private List<LevelItem> m_items = new List<LevelItem>();
    }

    [System.Serializable]
    public class PlaceholderThumbnail
    {
        public Placeholder PlaceholderPrefab => m_placeholderPrefab;
        public Sprite Thumbnail => m_thumbnail;
        public GameObject ActualPrefab => m_actualPrefab;

        [SerializeField]
        private Placeholder m_placeholderPrefab;
        [SerializeField]
        private Sprite m_thumbnail;
        [SerializeField]
        private GameObject m_actualPrefab;
    }

    [DisallowMultipleComponent]
    public class LevelEditorManager : MonoBehaviour
    {
        public static LevelEditorManager Instance { get; private set; }
        public EEditorMode CurrentMode
        {
            get => m_currentMode;
            set
            {
                if (m_currentMode == value)
                {
                    return;
                }

                m_currentMode = value;
                UnbindInteractions();
                switch (m_currentMode)
                {
                    case EEditorMode.SELECT:
                        m_selectedPrefab = null;
                        LevelEditorUI.Instance.SetContainerVisibility(false);
                        InputSystem.actions.FindAction("Click").started += SelectPlaceholder;
                        InputSystem.actions.FindAction("Click").canceled += StopMoving;
                        break;
                    case EEditorMode.PLACE:
                        if (m_selectedPlaceholder != null)
                        {
                            m_selectedPlaceholder.Deselect();
                        }
                        LevelEditorUI.Instance.SetContainerVisibility(true);
                        InputSystem.actions.FindAction("Click").started += _ => PlacePlaceholder();
                        break;
                }
            }
        }

        private void StopMoving(InputAction.CallbackContext _context)
        {
            m_isMoving = false;
        }

        [SerializeField]
        private List<PlaceholderThumbnail> m_placeholderPrefabs = new List<PlaceholderThumbnail>();
        [SerializeField]
        private Transform m_placeHolderParent = null;
        [SerializeField]
        private GameManager m_gameManager = null;
        [SerializeField]
        private PlayerController m_player = null;
        [SerializeField]
        private Transform m_ingameUI = null;
        [SerializeField]
        private Transform m_editorUI = null;


        private Placeholder m_selectedPrefab = null;

        private EEditorMode m_currentMode = EEditorMode.NONE;
        private Placeholder m_selectedPlaceholder;
        private List<Placeholder> m_createdPlaceholders = new List<Placeholder>();
        private bool m_isMoving = false;
        private List<GameObject> m_gameplayObjects = new List<GameObject>();
        private Vector2 m_scrollWheelValue = Vector2.zero;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            m_createdPlaceholders = FindObjectsByType<Placeholder>(FindObjectsSortMode.None).ToList();

            InputSystem.actions.FindAction("ScrollWheel").performed += ctx =>
            {
                m_scrollWheelValue = ctx.ReadValue<Vector2>();
            };
        }

        private void Start()
        {
            LevelEditorUI.Instance.Initialize(m_placeholderPrefabs);
            LevelEditorUI.Instance.OnPlaceholderTileSelected += OnTileSelected;
            CurrentMode = EEditorMode.SELECT;
        }

        public void Save()
        {
            LevelData data = new LevelData();
            foreach (Placeholder placeholder in m_createdPlaceholders)
            {
                data.Items.Add(new LevelItem(placeholder.transform.position, placeholder.transform.rotation.eulerAngles.z, placeholder.PlaceholderId));
            }

            List<string> existingLevels = GetSavedLevels();
            string levelName = "NewLevel.json";
            int counter = 1;
            while (existingLevels.Contains(levelName))
            {
                levelName = $"NewLevel_{counter}.json";
                counter++;
            }

            string json = JsonUtility.ToJson(data, true);
            string filepath = Application.streamingAssetsPath + $"/Levels/{levelName}";
            System.IO.Directory.CreateDirectory(Application.streamingAssetsPath + "/Levels/");
            System.IO.File.WriteAllText(filepath, json);
        }

        public void Load(string _fileName)
        {
            string filePath = Application.streamingAssetsPath + "/Levels/" + _fileName;
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError($"Level file not found at path: {filePath}");
                return;
            }

            foreach (Placeholder placeholder in m_createdPlaceholders)
            {
                placeholder.OnSelect -= OnPlaceholderSelected;
                placeholder.OnDeselect -= OnPlaceholderDeselected;
                Destroy(placeholder.gameObject);
            }
            m_selectedPlaceholder = null;
            m_createdPlaceholders.Clear();

            string json = System.IO.File.ReadAllText(filePath);
            LevelData data = JsonUtility.FromJson<LevelData>(json);

            foreach (LevelItem item in data.Items)
            {
                PlaceholderThumbnail thumbnail = m_placeholderPrefabs.FirstOrDefault(t => t.PlaceholderPrefab.PlaceholderId == item.ID);
                if (thumbnail ==  null)
                {
                    Debug.LogError($"No placeholder prefab found for ID: {item.ID}");
                    continue;
                }
                Placeholder newPlaceholder = Instantiate(thumbnail.PlaceholderPrefab, item.Position, Quaternion.Euler(0f, 0f, item.Rotation));
                newPlaceholder.transform.parent = m_placeHolderParent;
                m_createdPlaceholders.Add(newPlaceholder);
                newPlaceholder.OnSelect += OnPlaceholderSelected;
                newPlaceholder.OnDeselect += OnPlaceholderDeselected;
            }
        }

        public List<string> GetSavedLevels()
        {
            string path = Application.streamingAssetsPath + "/Levels/";
            if (!System.IO.Directory.Exists(path))
            {
                return new List<string>();
            }
            return System.IO.Directory.GetFiles(path, "*.json").Select(f => System.IO.Path.GetFileName(f)).ToList();
        }

        public void Play()
        {
            m_gameManager.gameObject.SetActive(true);
            m_player.gameObject.SetActive(true);
            m_ingameUI.gameObject.SetActive(true);

            m_editorUI.gameObject.SetActive(false);
            m_placeHolderParent.gameObject.SetActive(false);
            foreach (Placeholder placeholder in m_createdPlaceholders)
            {
                PlaceholderThumbnail thumbnail = m_placeholderPrefabs.FirstOrDefault(t => t.PlaceholderPrefab.PlaceholderId == placeholder.PlaceholderId);
                if (thumbnail == null)
                {
                    continue;
                }
                GameObject gameplayObject = Instantiate(thumbnail.ActualPrefab, placeholder.transform.position, placeholder.transform.rotation);
                m_gameplayObjects.Add(gameplayObject);
            }
            CurrentMode = EEditorMode.NONE;
            StartCoroutine(StartGameAsync());
        }

        private IEnumerator StartGameAsync()
        {
            yield return null;
            m_gameManager.StartGame();
        }

        public void StopPlaying()
        {
            foreach (GameObject obj in m_gameplayObjects)
            {
                Destroy(obj);
            }
            m_gameplayObjects.Clear();
            m_gameManager.gameObject.SetActive(false);
            m_player.gameObject.SetActive(false);
            m_ingameUI.gameObject.SetActive(false);
            m_placeHolderParent.gameObject.SetActive(true);
            m_editorUI.gameObject.SetActive(true);

            CurrentMode = EEditorMode.PLACE;
        }

        private void OnTileSelected(Placeholder _prefab)
        {
            m_selectedPrefab = _prefab;
        }

        private void Update()
        {
            if (m_currentMode == EEditorMode.SELECT)
            {
                MovePlaceholder();
            }
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (CurrentMode == EEditorMode.SELECT)
                {
                    CurrentMode = EEditorMode.PLACE;
                }
                else if (CurrentMode == EEditorMode.PLACE)
                {
                    CurrentMode = EEditorMode.SELECT;
                }
            }
        }

        private void OnDestroy()
        {
            UnbindInteractions();
        }

        private void UnbindInteractions()
        {
            InputSystem.actions.FindAction("Click").started -= SelectPlaceholder;
            InputSystem.actions.FindAction("Click").canceled -= DeselectPlaceholder;
            InputSystem.actions.FindAction("Click").started -= _ => PlacePlaceholder();
        }

        private void DeselectPlaceholder(InputAction.CallbackContext _context)
        {
            if (m_selectedPlaceholder == null)
            {
                return;
            }

            m_selectedPlaceholder.Deselect();
        }

        private void SelectPlaceholder(InputAction.CallbackContext _context)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
            if (hit.collider == null)
            {
                if (m_selectedPlaceholder != null)
                {
                    m_selectedPlaceholder.Deselect();
                }
                return;
            }
            Placeholder placeholder = hit.collider.GetComponentInParent<Placeholder>();
            if (placeholder != null)
            {
                placeholder.Select();
                m_isMoving = true;
            }
        }

        private void MovePlaceholder()
        {
            if (m_selectedPlaceholder == null || !m_isMoving)
            {
                return;
            }

            Vector3 position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            position.z = 0f;
            m_selectedPlaceholder.transform.position = position;

            if (m_scrollWheelValue.y < 0.0f)
            {
                Vector3 angles = m_selectedPlaceholder.transform.localEulerAngles;
                angles.z -= 90.0f;
                m_selectedPlaceholder.transform.localEulerAngles = angles;
            }
            else if (m_scrollWheelValue.y > 0.0f)
            {
                Vector3 angles = m_selectedPlaceholder.transform.localEulerAngles;
                angles.z += 90.0f;
                m_selectedPlaceholder.transform.localEulerAngles = angles;
            }
        }

        private void PlacePlaceholder()
        {
            if (m_selectedPrefab == null)
            {
                return;
            }
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (mousePosition.y < 185.7419)
            {
                // ignore bottom part where ui lives
                return;
            }
            Vector3 position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            position.z = 0f;
            Placeholder newPlaceholder = Instantiate(m_selectedPrefab, position, Quaternion.identity);
            m_createdPlaceholders.Add(newPlaceholder);
            newPlaceholder.OnSelect += OnPlaceholderSelected;
            newPlaceholder.OnDeselect += OnPlaceholderDeselected;
            newPlaceholder.transform.parent = m_placeHolderParent;
        }

        private void OnPlaceholderSelected(Placeholder _placeholder)
        {
            m_selectedPlaceholder = _placeholder;
        }

        private void OnPlaceholderDeselected(Placeholder _placeholder)
        {
            if (m_selectedPlaceholder == _placeholder)
            {
                m_selectedPlaceholder = null;
            }
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Save Level"))
            {
                Save();
            }
            if (GUILayout.Button("Load Level"))
            {
                Load("NewLevel.json");
            }
        }
    }
}
