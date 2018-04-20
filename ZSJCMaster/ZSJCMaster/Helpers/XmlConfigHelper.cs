using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ZSJCMaster.Helpers
{
    // <summary>
    /// XML配置文件读写类
    /// </summary>
    public class XmlConfigHelper
    {
        XDocument xd;
        #region 节点

        /// <summary>
        /// 获取所有节点的名称
        /// </summary>
        /// <param name="includeRootNode">是否包含根节点</param>
        /// <returns>所有节点的名称数组</returns>
        public string[] GetAllNodeNames(bool includeRootNode = false)
        {
            if (xd == null) { return null; }
            XNode[] nodes;
            if (includeRootNode)
            {
                nodes = xd.Descendants().Where(n => n.NodeType == XmlNodeType.Element)
                        .Distinct().ToArray();
            }
            else
            {
                nodes = xd.Root.Descendants().Where(n => n.NodeType == XmlNodeType.Element)
                        .Distinct().ToArray();
            }
            string[] nodeNames = new string[nodes.Length];
            for (int i = 0; i < nodeNames.Length; i++)
            {
                nodeNames[i] = (nodes[i] as XElement).Name.ToString();
            }
            return nodeNames.Distinct().ToArray();
        }

        /// <summary>
        /// 获取指定名称的节点数量
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="parentName">父节点名称，如果为空，则为根节点</param>
        /// <param name="parentNodeIndex">具有该名称的所有父节点的索引，可以通过指定index指定父节点，默认-1将匹配所有父节点</param>
        /// <returns>匹配该名称的节点的数量</returns>
        public int GetCountByNodeName(string nodeName, string parentName = null, int parentNodeIndex = -1)
        {
            if (xd == null) { return -1; }
            if (string.IsNullOrEmpty(nodeName))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(parentName))
            {
                parentName = xd.Root.Name.ToString();
            }
            if (nodeName.Equals(xd.Root.Name))
            {
                return 1;
            }
            else if (parentName.Equals(xd.Root.Name.ToString()))
            {
                return xd.Root.Descendants().Where(n => n.Name == nodeName).ToList().Count;
            }
            else
            {
                List<XElement> parents = xd.Root.Descendants().Where(p => p.Name == parentName).ToList();
                List<XElement> elements = new List<XElement>();
                if (parentNodeIndex == -1 || parentNodeIndex > parents.ToList().Count)
                {
                    foreach (var parent in parents)
                    {
                        List<XElement> list = parent.Descendants().Where(n => n.Name == nodeName).ToList();
                        foreach (var item in list)
                        {
                            elements.Add(item);
                        }
                    }
                }
                else
                {
                    List<XElement> list = parents[parentNodeIndex].Descendants().Where(n => n.Name == nodeName).ToList();
                    foreach (var item in list)
                    {
                        elements.Add(item);
                    }
                }
                return elements.Count;
            }
        }

        /// <summary>
        /// 获得具有指定名称和值的节点的索引
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="value">该节点的值，如果多个节点拥有该值，则抛出异常</param>
        /// <returns>具有该名称的多个节点中，具有该值的节点的索引</returns>
        public int GetIndexByNodeValue(string nodeName, string value)
        {
            if (xd == null) { return -1; }
            if (nodeName == xd.Root.Name.ToString())
            {
                return 0;
            }
            List<XElement> nodes = xd.Root.Descendants().Where(n => n.Name == nodeName).ToList();
            if (nodes == null) { return -1; }
            List<XElement> elements = nodes.Where(n => n.Value == value).ToList();
            if (elements.Count > 1)
            {
                throw new Exception("多个节点具有该值");
            }
            return nodes.IndexOf(elements[0]);
        }

        /// <summary>
        /// 获得指定名称和特性的节点索引
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrNamesAndValuesString">特性名称和相应的值的字符串形式，如 attr = "value",如果该节点有多个特性，需将所有的特性及值列出</param>
        /// <returns>具有该名称的多个节点中，具有给定特性的节点的索引</returns>
        public int GetIndexByAttrbutes(string nodeName, params string[] attrNamesAndValuesString)
        {
            if (xd == null) { return -1; }
            if (nodeName == xd.Root.Name.ToString())
            {
                return 0;
            }
            List<XElement> nodes = xd.Root.Descendants().Where(n => n.Name == nodeName).ToList();
            if (nodes == null) { return -1; }
            bool find = true;
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (!node.HasAttributes ||
                     node.Attributes().ToArray().Length < attrNamesAndValuesString.Length)
                {
                    continue;
                }
                foreach (var attr in node.Attributes().ToArray())
                {
                    string attrString = attr.ToString();
                    bool flag = attrNamesAndValuesString.Any(s => s == attrString);
                    if (!flag)
                    {
                        find = false;
                        break;
                    }
                    else
                    {
                        find = true;
                    }
                }
                if (find)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 新增节点
        /// </summary>
        /// <param name="parentNodeName">父节点名称,如果为空，那么父节点就为根节点</param>
        /// <param name="nodeName">新节点的名称</param>
        /// <param name="value">新节点的值</param>
        public void AddNode(string parentNodeName, string nodeName, string value)
        {
            if (xd == null) { return; }
            //如果父节点为空，那么就是在根节点下添加子节点
            if (string.IsNullOrEmpty(parentNodeName))
            {
                xd.Root.Add(new XElement(nodeName, value));
            }
            else
            {
                //如果父节点不为空，那么首先查找这个节点，然后把新节点依次添加进去
                var result = xd.Root.DescendantsAndSelf().Where(n => n.Name == parentNodeName).ToList();
                foreach (var ele in result)
                {
                    ele.Add(new XElement(nodeName, value));
                }
            }
        }

        /// <summary>
        /// 移除指定的节点
        /// </summary>
        /// <param name="nodeName">要移除的节点名称</param>
        /// <param name="index">如果包含多个名称为该名称的节点，可以通过指定index移除特定的节点，默认-1将移除所有符合该名称的节点</param>
        public void RemoveNode(string nodeName, int index = -1)
        {
            if (xd == null) { return; }
            if (xd.Root != null)
            {
                var result = xd.Root.DescendantsAndSelf().Where(n => n.Name == nodeName).ToList();
                if (index > -1)
                {
                    if (result.Count > index)
                    {
                        result[index].Remove();
                    }
                }
                else
                {
                    foreach (var node in result)
                    {
                        node.Remove();
                    }
                }
            }
        }

        /// <summary>
        /// 插入节点
        /// </summary>
        /// <param name="prevNodeName">被插入节点名称，将在此节点前插入新节点</param>
        /// <param name="nodeName">新节点名称</param>
        /// <param name="value">新节点的值</param>
        /// <param name="index">如果包含多个名称为该名称的节点，可以通过指定index在特定的节点前插入节点，默认-1将在所有符合该名称的节点上插入节点</param>
        public void InsertNode(string prevNodeName, string nodeName, string value, int index = -1)
        {
            if (xd == null) { return; }
            if (xd.Root != null)
            {
                var result = xd.Root.DescendantsAndSelf().Where(n => n.Name == prevNodeName).ToList();
                if (index > -1)
                {
                    if (result.Count > index)
                    {
                        result[index].AddBeforeSelf(new XElement(nodeName, value));
                    }
                }
                else
                {
                    foreach (var ele in result)
                    {
                        ele.AddBeforeSelf(new XElement(nodeName, value));
                    }
                }
            }
        }

        /// <summary>
        /// 修改节点值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="value">值</param>
        /// <param name="index">如果包含多个名称为该名称的节点，可以通过指定index修改特定的节点，默认-1将修改所有符合该名称的节点的值</param>
        public void UpdateNodeValue(string nodeName, string value, int index = -1)
        {
            if (xd == null) { return; }
            try
            {
                var result = xd.Descendants().Where(n => n.Name == nodeName).ToList();
                if (index > -1)
                {
                    if (result.Count > index)
                    {
                        result[index].SetValue(value);
                    }

                }
                else
                {
                    foreach (var ele in result)
                    {
                        ele.SetValue(value);
                    }
                }
            }
            catch (XmlException)
            {
                throw new XmlException("不是标准的XML文本");
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 读取单个节点值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <returns>节点值</returns>
        public string ReadNodeValue(string nodeName)
        {
            if (xd == null) { return null; }
            try
            {
                var element = xd.Descendants().FirstOrDefault(n => n.Name == nodeName);
                if (element != null)
                {
                    return element.Value;
                }
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 读取多个节点值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="index">可将索引设置为-1以获取所有结果，否则请设置具体索引以获取指定结果，默认-1将取回所有符合该名称的值</param>
        /// <returns>节点值数组</returns>
        public string[] ReadNodeValues(string nodeName, int index = -1)
        {
            if (xd == null) { return null; }
            var result = xd.Descendants().Where(n => n.Name == nodeName).ToList();
            if (index > -1)
            {
                if (result.Count > index)
                {
                    return new string[] { result[index].Value };
                }
            }
            string[] values = new string[result.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = result[i].Value;
            }
            return values;
        }

        /// <summary>
        /// 获取指定节点的个数
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <returns>符合该名称的节点个数</returns>
        public int GetNodesCount(string nodeName)
        {
            if (xd == null) { return -1; }
            int count = xd.Descendants().Count(n => n.Name == nodeName);
            return count;
        }
        #endregion 节点

        #region 特性(Attribute)
        /// <summary>
        /// 给节点增加特性(Attribute)
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">新特性的名称</param>
        /// <param name="value">新特性的值</param>
        /// <param name="index">如果包含多个名称为该节点名称的节点，可以通过指定index在特定的节点增加特性，默认-1将在所有符合该名称的节点上新增该特性</param>
        public void AddAttribute(string nodeName, string attrName, string value, int index = -1)
        {
            if (xd == null) { return; }
            var result = xd.Descendants().Where(n => n.Name == nodeName).ToList();
            if (index > -1)
            {
                if (result.Count > index)
                {
                    result[index].SetAttributeValue(attrName, value);
                    return;
                }
            }
            foreach (var ele in result)
            {
                var attrResult = ele.Attributes(attrName).ToList();
                ele.SetAttributeValue(attrName, value);
            }
        }

        /// <summary>
        /// 给节点移除特性(Attribute)
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">特性名称</param>
        /// <param name="index">如果包含多个名称为该节点名称的节点，可以通过指定index在特定的节点移除特性，默认-1将在所有符合该名称的节点上移除该特性</param>
        public void RemoveAttribute(string nodeName, string attrName, int index = -1)
        {
            AddAttribute(nodeName, attrName, null, index);
        }
        /// <summary>
        /// 给节点更新特性(Attribute)
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">特性名称</param>
        /// <param name="value">特性值</param>
        /// <param name="index">如果包含多个名称为该节点名称的节点，可以通过指定index在特定的节点更新特性，默认-1将在所有符合该名称的节点上更新该特性</param>
        public void UpdateAttributeValue(string nodeName, string attrName, string value, int index = -1)
        {
            AddAttribute(nodeName, attrName, value, index);
        }

        /// <summary>
        /// 读取单个节点的指定特性值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">特性名称</param>
        /// <returns>特性的值</returns>
        public string ReadAttributeValue(string nodeName, string attrName)
        {
            if (xd == null) { return null; }
            var result = xd.Descendants().FirstOrDefault(n => n.Name == nodeName);
            if (result != null)
            {
                var attr = result.Attributes().SingleOrDefault(a => a.Name == attrName);
                return attr == null ? null : attr.Value;
            }
            return null;
        }

        /// <summary>
        /// 读取多个节点的指定特性值
        /// </summary>
        /// <param name="nodeName">节点名称</param>
        /// <param name="attrName">特性名称</param>
        /// <param name="index">如果有多个符合该节点名称的节点，可通过设置索引获取指定节点的特性值，默认-1返回所有符合该节点名称的所有特性值</param>
        /// <returns></returns>
        public string[] ReadAttributeValues(string nodeName, string attrName, int index = -1)
        {
            if (xd == null) { return null; }
            var result = xd.Descendants().Where(n => n.Name == nodeName).ToList();
            if (index > -1)
            {
                if (result.Count > index)
                {
                    var element = result[index].Attributes().SingleOrDefault(a => a.Name == attrName);
                    if (element != null)
                    {
                        return new string[] { element.Value };
                    }
                }
            }
            List<string> list = new List<string>();
            for (int i = 0; i < result.Count; i++)
            {
                var element = result[i].Attributes().SingleOrDefault(a => a.Name == attrName);
                if (element != null)
                {
                    list.Add(element.Value);
                }

            }
            return list.ToArray();
        }
        #endregion 特性(Attribute)

        #region 加载、创建、保存、转换
        /// <summary>
        /// 将指定的xml文本转换为xml配置文件
        /// </summary>
        /// <param name="xmlText">要转换的xml文本</param>
        public void CreateConfigFromText(string xmlText)
        {
            try
            {
                if (!string.IsNullOrEmpty(xmlText))
                {
                    xd = XDocument.Parse(xmlText);
                }
            }
            catch (XmlException)
            {

                throw new XmlException("不是标准的XML文本");
            }
            catch (Exception)
            {
                throw;
            }


        }
        /// <summary>
        /// 创建一个指定根节点的xml配置文件
        /// </summary>
        /// <param name="rootNodeName">根节点名称</param>
        public void CreateConfig(string rootNodeName)
        {
            //创建根节点
            XElement rootNode = new XElement(rootNodeName);
            xd = new XDocument();
            xd.Add(rootNode);
        }

        /// <summary>
        /// 加载xml配置文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                xd = XDocument.Load(filePath);
            }
        }

        /// <summary>
        /// 将当前内存中的xml对象输出为文本形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (xd == null) { return string.Empty; }
            return xd.ToString();
        }

        /// <summary>
        /// 将修改后的结果保存到磁盘
        /// </summary>
        /// <param name="fileName">包含路径的完整文件名</param>
        public void Save(string fileName)
        {
            if (xd == null) { return; }
            xd.Save(fileName);
        }
        #endregion 加载、创建、保存、转换

    }
}
