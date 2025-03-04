using System;
using System.Collections.Generic;
using UnityEngine;

namespace BuildingSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class StreetIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color invalidColor;
        public bool IsValid => colliders.Count <= 0;
        
        private BoxCollider boxCollider;

        private void Awake()
        {
            SetVisibility(false);
            boxCollider = GetComponent<BoxCollider>();
        }

        private void FixedUpdate()
        {
            spriteRenderer.color = IsValid ? normalColor : invalidColor;
        }
        
        private List<Collider> colliders = new List<Collider>();
        
        private void OnTriggerEnter(Collider other)
        {
            colliders.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            colliders.Remove(other);
        }

        public void SetVisibility(bool visible)
        {
            spriteRenderer.enabled = visible;
        }
        
        public void SetPosition(Vector3 position)
        {
            position.y = 0.01f;
            transform.position = position;
            
        }
    }
}
