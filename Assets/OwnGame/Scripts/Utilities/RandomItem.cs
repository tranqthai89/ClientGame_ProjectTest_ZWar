using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevToolkit
{
    [Serializable]
    public class RandomItem
    {
        [Serializable]
        public class Item
        {
            public object valueCheck;
            public int weight;
        }

        public List<Item> originalItems = new List<Item>();
        public int originalTotalWeight;

        public List<Item> items = new List<Item>();
        public int totalWeight;

        public RandomItem()
        {
            Reset();
        }
        public void Reset()
        {
            originalItems = new List<Item>();
            originalTotalWeight = 0;

            items = new List<Item>();
            totalWeight = 0;
        }
        private void ResetToOriginal()
        {
            totalWeight = originalTotalWeight;

            items.Clear();
            for(int i = 0; i < originalItems.Count; i++)
            {
                Item _item = new Item 
                {
                    valueCheck = originalItems[i].valueCheck,
                    weight = originalItems[i].weight,
                };
                items.Add(_item);
            }
        }
        public void AddItem(object _valueCheck, int _weight)
        {
            if(_weight <= 0)
            {
                return;
            }
            Item _item = new Item
            {
                valueCheck = _valueCheck,
                weight = _weight 
            };
            originalTotalWeight += _weight;
            originalItems.Add(_item);

            totalWeight = originalTotalWeight;
            Item _itemClone = new Item
            {
                valueCheck = _item.valueCheck,
                weight = _item.weight,
            };
            items.Add(_itemClone);
        }
        public Item Roll(params object[] _listValueCheckReject)
        {
            // - Check Logic trước - //
            if(_listValueCheckReject != null && _listValueCheckReject.Length > 0)
            {
                int _totalWeightCollectedCheck = 0;
                for (int i = 0; i < originalItems.Count; i++)
                {
                    bool _flag = false;
                    for (int j = 0; j < _listValueCheckReject.Length; j++)
                    {
                        if (originalItems[i].valueCheck.Equals(_listValueCheckReject[j]))
                        {
                            _flag = true;
                            break;
                        }
                    }
                    if (_flag)
                    {
                        continue;
                    }

                    // số lượt quay ít nhất để có thể xuất hiện
                    int _tmp = originalItems[i].weight * 10;
                    if (_tmp >= originalTotalWeight)
                    {
                        // - Tồn tại 1 item có thể random => break - //
                        _totalWeightCollectedCheck += originalItems[i].weight;
                        break;
                    }
                }
                if (_totalWeightCollectedCheck == 0)
                {
                    return null;
                }
            }

            // - Nếu check logic ko có lỗi thì tiếp tục 
            int _totalWeightCollected = 0;
            List<Item> _pool = new List<Item>();
            for (int i = 0; i < items.Count; i++)
            {
                if(_listValueCheckReject != null && _listValueCheckReject.Length > 0)
                {
                    bool _flag = false;
                    for(int j = 0; j < _listValueCheckReject.Length; j++)
                    {
                        if (items[i].valueCheck.Equals(_listValueCheckReject[j]))
                        {
                            _flag = true;
                            break;
                        }
                    }
                    if (_flag)
                    {
                        continue;
                    }
                }

                // số lượt quay ít nhất để có thể xuất hiện
                int _tmp = items[i].weight * 10;
                if (_tmp >= totalWeight)
                {
                    _pool.Add(items[i]);
                    _totalWeightCollected += items[i].weight;
                }
            }

            if(_totalWeightCollected == 0 || _pool.Count == 0)
            {
                // - Trường hợp ko collect được item nào => reset về original luôn
                ResetToOriginal();

                return Roll(_listValueCheckReject);
            }
            else
            {
                int _randomValue = MyConstant.random.Next(_totalWeightCollected);

                int _currentWeight = 0;
                for (int i = 0; i < _pool.Count; i++)
                {
                    _currentWeight += _pool[i].weight;
                    if (_randomValue < _currentWeight)
                    {
                        // - Mỗi lần roll ra thì hạ weight xuống, khi về = 0 thì remove nó đi
                        _pool[i].weight--;
                        totalWeight--;
                        if (_pool[i].weight <= 0)
                        {
                            items.Remove(_pool[i]);
                            if (items.Count == 0)
                            {
                                ResetToOriginal();
                            }
                        }
                        return _pool[i];
                    }
                }

                return null; // Nếu có lỗi, trả về null
            }
        }
    }

}
