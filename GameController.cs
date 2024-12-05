using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HanoiTowerVisualization;

//namespace HanoiTowerVisualization
//{
    public class GameController : MonoBehaviour
    {
        public delegate void VisualizationCompleteHandler();
        public event VisualizationCompleteHandler OnVisualizationComplete = delegate { };
        
        public Torus TorusPrefab;

        [Space] 
        public Tower TowerFrom;
        public Tower TowerTo;
        public Tower TowerHelp;

        private readonly List<Torus> _toruses = new List<Torus>();
        private int _torusCount;

        public void Start()
        {
            //StartVisualization(3);
        }
        public void StartVisualization(int torusCount)
        {
            Clear();
            _torusCount = torusCount;
            StartCoroutine(StartVisualizationInternal());
        }


        private void Clear()
        {
            foreach (var toruse in _toruses)
                Destroy(toruse.gameObject);

            _toruses.Clear();
        }

        
        private IEnumerator StartVisualizationInternal()
        {
            yield return InstantiateToruses();
            yield return new WaitForSeconds(1.5f);
            yield return HanoiAlgorithm(0, TowerFrom, TowerTo, TowerHelp);

            yield return new WaitForSeconds(1f);
            OnVisualizationComplete();
        }

        private IEnumerator InstantiateToruses()
        {
            const float sizeMultiplyer = 0.1f;
            Color torusColor = Color.white;

            for (var i = 0; i < _torusCount; i++)
            {
                switch(i % 3)
                {
                    case 0:
                        torusColor = Color.red;
                        break;
                    case 1:
                        torusColor = Color.yellow;
                        break;
                    case 2:
                        torusColor = Color.blue;
                        break;
                }

                var torus = Instantiate(TorusPrefab);
                torus.gameObject.transform.localScale -= Vector3.one * sizeMultiplyer * i;
                torus.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = torusColor;
                torus.transform.GetChild(1).GetComponent<MeshRenderer>().material.color = torusColor;
                torus.transform.GetChild(2).GetComponent<MeshRenderer>().material.color = torusColor;
                torus.transform.GetChild(3).GetComponent<MeshRenderer>().material.color = torusColor;

                torus.transform.position = TowerFrom.TopPoint.position;
                torus.CurrentTower = TowerFrom;

                _toruses.Add(torus);
                yield return new WaitForSeconds(0.25f);
            }
        }

        
        private IEnumerator HanoiAlgorithm(int torusIndex, Tower from, Tower to, Tower help)
        {
            if (torusIndex >= _toruses.Count)
                yield break;

            yield return HanoiAlgorithm(torusIndex + 1, from, help, to);
            yield return MoveToTower(_toruses[torusIndex], to);
            yield return HanoiAlgorithm(torusIndex + 1, help, to, from);
        }
        

        private IEnumerator MoveToTower(Torus torus, Tower tower)
        {
            torus.Rigidbody.isKinematic = true;
            yield return MoveToPoint(torus.transform, torus.CurrentTower.TopPoint.position);
            yield return MoveToPoint(torus.transform, tower.TopPoint.position);
            torus.CurrentTower = tower;
            torus.Rigidbody.isKinematic = false;

            yield return new WaitForSeconds(1f);
        }

        private IEnumerator MoveToPoint(Transform torus, Vector3 destination)
        {
            const float time = 0.5f;
            var startPosition = torus.position;
            var startTime = Time.time;

            while (Time.time - startTime < time)
            {
                var newPosition = Vector3.Lerp(startPosition, destination, (Time.time - startTime) / time);
                torus.position = newPosition;
                yield return null;
            }
        }
    }
//}
