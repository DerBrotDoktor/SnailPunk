using System.Collections.Generic;
using Buildings;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildingSystem
{
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider))]
    public class MouseIndicator : MonoBehaviour
    {
        [SerializeField] private Grid grid;
        [SerializeField] private Camera myCamera;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Camera camera;
        
        private Vector3 lastMousePosition = Vector3.zero;
        public Vector3 GetMousePosition => lastMousePosition;
        private const float Offset = 0.005f;
        private const float colliderOffset = 0.01f;
        
        private SpriteRenderer spriteRenderer;
        private BoxCollider boxCollider;

        [Header("Colors")] 
        private bool shouldColorize = false;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color invalidColor;
        [SerializeField] private Color validColor;

        public bool IsColliding => colliders.Count > 0;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            boxCollider = GetComponent<BoxCollider>();
            spriteRenderer.color = normalColor;
            boxCollider.isTrigger = true;
        }

        private void Update()
        {
            GetCellPosition();
            transform.position = new Vector3(lastMousePosition.x + transform.localScale.x/2 + colliderOffset/2, lastMousePosition.y + Offset, lastMousePosition.z + transform.localScale.y/2 + colliderOffset/2);
        }
        
        public Vector3 GetCellPosition()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = myCamera.nearClipPlane;
            Ray ray = myCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                lastMousePosition = hit.point;
            }

            lastMousePosition = grid.CellToWorld(grid.WorldToCell(lastMousePosition));
            lastMousePosition.y = hit.point.y;
            return lastMousePosition;
        }

        public void SetSize(Vector3Int size)
        {
            transform.localScale = new Vector3(size.x - colliderOffset,size.z - colliderOffset, transform.localScale.y);
        }

        private List<Collider> colliders = new List<Collider>();
        
        private void OnTriggerEnter(Collider other)
        {
            colliders.Add(other);
            if (colliders.Count > 0 && shouldColorize)
            {
                spriteRenderer.color = invalidColor;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            colliders.Remove(other);
        }

        public void OnTriggerDestroy(Collider other)
        {
            if(colliders.Count <= 0 || !colliders.Contains(other)) return;
            
            ReloadCollisions();
        }

        public void ReloadCollisions()
        {
            colliders.Clear();
            
            Collider[] colls = Physics.OverlapBox(GetMousePosition, new Vector3(boxCollider.size.x/2, boxCollider.size.y/2, boxCollider.size.z/2), transform.rotation, layerMask);

            foreach (var coll in colls)
            {
                if (coll.isTrigger && coll.enabled)
                {
                    colliders.Add(coll);
                }
            }
        }
        
        public void SetColorize(bool value)
        {
            shouldColorize = value;
            if (!value)
            {
                spriteRenderer.color = normalColor;
            }
            
        }

        public void SetValidPosition(bool value)
        {
            if (!shouldColorize) return;

            spriteRenderer.color = value ? validColor : invalidColor;
        }
        
        public void SetRotation(float rotation)
        {
            transform.rotation = Quaternion.Euler(90, 0, rotation);
        }
    }
}
