using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Collections;

namespace DualListSerialization.Models
{
    public class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; 
        public string Data;
    }


    public class ListRandom: IEnumerable<ListNode>
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count;
        #region Enumeration
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public IEnumerator<ListNode> GetEnumerator()
        {
            var current = Head;
            while (current != null)
            {
                yield return current;
                current = current.Next;
            }
        }
        #endregion

        #region Serialization
        public void Serialize(Stream stream)
        {
            var nodeToRandomPointerDictionary = new Dictionary<ListNode, int>();

            foreach (var nodeTuple in this.Select((node, id) => (node, id)))
            {
                nodeToRandomPointerDictionary.Add(nodeTuple.node, nodeTuple.id);
            }

            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (var node in this)
                {
                    writer.WriteLine($"{node.Data}-{nodeToRandomPointerDictionary[node.Random]}");
                }
            }

        }


        public void Deserialize(Stream stream)
        {
            List<Tuple<string, int>> dataEntries = new List<Tuple<string, int>>();
            HashSet<int> faultyNodes = new HashSet<int>();
            using (StreamReader reader = new StreamReader(stream))
            {
                var counter = 0;
                while (!reader.EndOfStream)
                {
                    var rawData = reader.ReadLine();
                    var delimeterIndex = rawData.LastIndexOf("-");
                    if (delimeterIndex != -1)
                    {
                        var data = rawData.Substring(0, delimeterIndex);
                        int.TryParse(rawData.Substring(delimeterIndex + 1, rawData.Length - 1), out var randomId);
                        if (faultyNodes.Count > 0)
                        {
                            if (randomId >= faultyNodes.Min())
                            {
                                randomId -= faultyNodes.Where(x => x <= randomId).Count();
                            }
                        }
                        dataEntries.Add(new Tuple<string, int>(data, randomId));
                    }
                    else
                    {
                        faultyNodes.Add(counter);
                    }

                }
                Count = counter;

            }

            Head = new ListNode();
            ListNode current = Head;
            for (var i = 0; i < Count; i++)
            {
                current.Data = dataEntries[i].Item1;
                current.Next = new ListNode();
                if (i != this.Count - 1)
                {
                    current.Next.Previous = current;
                    current = current.Next;
                }
                else
                {
                    Tail = current;
                }
            }

            foreach (var nodeTuple in this.Select((node, index) => (node, index)))
            {
                nodeTuple.node.Random = GetRandomNode(dataEntries.ElementAt(nodeTuple.index).Item2);
            }
        }
        private ListNode GetRandomNode(int pointerIndex)
        {
            foreach (var nodeTuple in this.Select((node, index) => (node, index)))
            {
                if (nodeTuple.index == pointerIndex)
                    return nodeTuple.node;
            }
            return null;
        }
#endregion
    }

}
