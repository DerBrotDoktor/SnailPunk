using System;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Snails
{
    public class Movement : MonoBehaviour
    {
        public event Action TargetReached;

        [SerializeField] private float speed = 5f;
        [SerializeField] private float rotationSpeed = 360f;

        private Pathfinder pathfinder;
        private List<Vector3> path;
        private int pathIndex;
        public bool IsMoving { get; private set; }
        public Vector3 TargetPosition { get; private set; }

        private void Awake()
        {
            pathfinder = FindObjectOfType<Pathfinder>();

            if (pathfinder == null) 
            {
                throw new Exception("Pathfinder not found");
            }
        }

        private void Update()
        {
            // Bewege die Schnecke, wenn sie sich bewegen soll und der Pfad existiert
            if (!IsMoving || path == null || path.Count <= 0 || pathIndex >= path.Count) return;
            MoveAlongPath();
        }

        // Setzt das Ziel für die Schnecke
        public void SetDestination(Vector3 position, bool force = false)
        {
            print(name + " Set Destination: " + position + " " + force);
            
            path = new List<Vector3>();
            pathIndex = 0;
            
            path = pathfinder.FindPath(transform.position, position);

            if (force && path.Count <= 0)
            {
                path.Add(position);
                IsMoving = true;
                return;
            }
            
            IsMoving = !(path == null || path.Count <= 0);
        }
        
        private void MoveAlongPath()
        {
            if (!IsMoving) return;
            float distance = Vector3.Distance(transform.position, path[pathIndex]);
            
            if (distance < 0.05f)
            {
                pathIndex++;
                if (pathIndex >= path.Count)
                {
                    IsMoving = false;
                    TargetReached?.Invoke();
                    return;
                }
            }
            
            Vector3 direction = (path[pathIndex] - transform.position).normalized;
            RotateTowards(direction);
            transform.Translate(direction * (speed * Time.deltaTime * Game.TimeScale), Space.World);
        }

        // Dreht die Schnecke in Richtung der Bewegung
        private void RotateTowards(Vector3 direction)
        {
            if (direction == Vector3.zero) return; // Keine Drehung nötig, wenn keine Richtung vorhanden

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * Game.TimeScale);
        }
    }
}

