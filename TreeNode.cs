using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TraceFileTool
{
    public class TreeNode<T>
    {
        private readonly T _value;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        public TreeNode(T value)
        {
            _value = value;
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public TreeNode<T> Parent { get; private set; }
        public IEnumerable<TreeNode<T>> Parents { get {
                var parent = Parent;
                var result = new List<TreeNode<T>>();
                while(parent != null)
                {
                    result.Add(parent);
                    parent = parent.Parent;
                }
                return result;
            }
        }

        public T Value { get { return _value; } }

        public ReadOnlyCollection<TreeNode<T>> Children
        {
            get { return _children.AsReadOnly(); }
        }

        public TreeNode<T> AddChild(TreeNode<T> value)
        {
            var node = value;
            node.Parent = this;
            _children.Add(node);
            return node;
        }
        
        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public IEnumerable<TreeNode<T>> FlattenChildren()
        {
            return _children.Concat(_children.SelectMany(x => x.FlattenChildren()));
        }
    }
}
