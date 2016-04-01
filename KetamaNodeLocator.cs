using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KetamaHash
{
    public class KetamaNodeLocator
    {
        private SortedList<long, string> _nodes;
        private SortedList<long, string> m_Nodes
        {
            get { return _nodes ?? (_nodes = new SortedList<long, string>()); }
        }

        /// <summary>
        /// 一致性哈希的实现
        /// </summary>
        /// <param name="nodes">实际节点</param>
        /// <param name="nodeCopies">虚拟节点的个数</param>
        public KetamaNodeLocator(IEnumerable<string> nodes, int nodeCopies)
        {
            //对所有节点，生成nCopies个虚拟结点
            foreach (var node in nodes)
            {
                //每四个虚拟结点为一组
                for (var i = 0; i < nodeCopies / 4; i++)
                {
                    //getKeyForNode方法为这组虚拟结点得到惟一名称 
                    var digest = HashAlgorithm.ComputeMd5(node + i);
                    //Md5是一个16字节长度的数组，将16字节的数组每四个字节一组，分别对应一个虚拟结点，这就是为什么上面把虚拟结点四个划分一组的原因 
                    for (var h = 0; h < 4; h++)
                    {
                        var m = HashAlgorithm.Hash(digest, h);
                        m_Nodes[m] = node;
                    }
                }
            }
        }
        /// <summary>
        /// 一致性哈希的实现
        /// </summary>
        /// <param name="nodes">实际节点</param>
        public KetamaNodeLocator(IEnumerable<string> nodes)
            : this(nodes, 10)
        {
        }

        /// <summary>
        /// 查找Key值所在的节点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetNodes(string key)
        {
            var digest = HashAlgorithm.ComputeMd5(key);
            var hash = HashAlgorithm.Hash(digest, 0);
            return GetNodeForKey(hash);
        }

        /// <summary>
        /// 通过Key值获取节点
        /// </summary>
        /// <param name="hash"></param>
        private string GetNodeForKey(long hash)
        {
            var key = hash;
            //如果找到这个节点，直接取节点，返回   
            if (m_Nodes.ContainsKey(key)) return m_Nodes[key];

            //得到大于当前key的那个子Map，然后从中取出第一个key，就是大于且离它最近的那个key 说明详见: http://www.javaeye.com/topic/684087
            var tailMap = from coll in _nodes
                where coll.Key > hash
                select new { coll.Key };

            var tail = tailMap.FirstOrDefault();

            key = tail == null ? m_Nodes.First().Key : tail.Key;
            return m_Nodes[key];
        }
    }
}
