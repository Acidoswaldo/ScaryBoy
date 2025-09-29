using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace iconPopUp
{
    public class IconPopUp : MonoBehaviour
    {

        public static IconPopUp instance;

        // ======= Pooling variables ======//

        [SerializeField] IconPopUp_PF _popUp;
        [SerializeField] Transform _holder;
        [SerializeField] int _size;
        public Dictionary<string, Queue<IconPopUp_PF>> _poolDictionary;
        public string _tag = "Icon";
        // ===== end Pooling variables ====//

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this);
            }
            _poolDictionary = new Dictionary<string, Queue<IconPopUp_PF>>();
            InsantiatePools();
        }

        void InsantiatePools()
        {
            Queue<IconPopUp_PF> objectPool = new Queue<IconPopUp_PF>();
            for (int i = 0; i < _size; i++)
            {
                IconPopUp_PF obj = Instantiate(_popUp);
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_holder);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                objectPool.Enqueue(obj);
            }
            _poolDictionary.Add(_tag, objectPool);
        }

        IconPopUp_PF SpawnFroomPool(string tag)
        {
            if (!_poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("pool With tag" + tag + " doesn't exist");
                return null;
            }
            IconPopUp_PF ObjectToSpawn = _poolDictionary[tag].Dequeue();
            ObjectToSpawn.gameObject.SetActive(true);
            _poolDictionary[tag].Enqueue(ObjectToSpawn);
            return ObjectToSpawn;
        }

        public static void Create(Vector3 pos, float number, Vector3 customScale)
        {
            IconPopUp_PF iconObject = instance.SpawnFroomPool(instance._tag);
            iconObject.transform.position = pos;
            iconObject.InitializePopUp(number, customScale);
        }
    }
}
