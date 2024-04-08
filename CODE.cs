using System;
using System.Collections;
using System.Collections.Generic;

namespace GeneealizationsAndLambdaExspressions
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        private class Node
        {
            public T Data;
            public Node Left;
            public Node Right;

            public Node(T data)
            {
                Data = data;
                Left = null;
                Right = null;
            }
        }

        private Node _root;
        private Node _current;

        public BinaryTree()
        {
            _root = null;
            _current = null;
        }

        public void Insert(T data)
        {
            _root = Insert(_root, data);
        }

        private Node Insert(Node node, T data)
        {
            if (node == null)
            {
                return new Node(data);
            }

            int compareResult = data.CompareTo(node.Data);

            if (compareResult < 0)
            {
                node.Left = Insert(node.Left, data);
            }
            else if (compareResult > 0)
            {
                node.Right = Insert(node.Right, data);
            }
            else
            {
                throw new InvalidOperationException("Узел с таким значением уже существует");
            }

            return node;
        }

        public void Reset()
        {
            _current = _root;
            if (_current == null)
            {
                return;
            }
            while (_current.Left != null)
            {
                _current = _current.Left;
            }
        }

        public void Next()
        {
            if (_current == null)
                return;
            if (_current.Right != null)
            {
                _current = _current.Right;
                while (_current.Left != null)
                {
                    _current = _current.Left;
                }
            }
            else
            {
                while (_current != null && _current.Right == null)
                {
                    _current = _current.Right;
                }
                if (_current != null)
                {
                    _current = _current.Right;
                    while (_current.Left != null)
                    {
                        _current = _current.Left;
                    }
                }
            }
        }

        public void Previous()
        {
            if (_current == null)
                return;
            if (_current.Left != null)
            {
                _current = _current.Left;
                while (_current.Right != null)
                {
                    _current = _current.Right;
                }
            }
            else
            {
                while (_current != null && _current.Left == null)
                {
                    _current = _current.Left;
                }
                if (_current != null)
                {
                    _current = _current.Left;
                    while (_current.Right != null)
                    {
                        _current = _current.Right;
                    }
                }
            }
        }

        public T Current()
        {
            if (_current != null)
            {
                return _current.Data;
            }
            else
            {
                return default(T);
            }
        }

        private List<Node> _nodes = new List<Node>();
        private int _index;

        private void PushLeftNodes(Node node)
        {
            while (node != null)
            {
                _nodes.Add(node);
                node = node.Left;
            }
        }

        public bool MovePrevious()
        {
            if (_index == _nodes.Count)
                return false;

            Node node = _nodes[++_index];
            PushLeftNodes(node.Left);
            return true;
        }

        public bool MoveNext()
        {
            if (_index == 0)
                return false;

            Node node = _nodes[--_index];
            PushLeftNodes(node.Right);
            return true;
        }

        public static BinaryTree<T> operator ++(BinaryTree<T> enumerator)
        {
            enumerator.MoveNext();
            return enumerator;
        }

        public static BinaryTree<T> operator --(BinaryTree<T> enumerator)
        {
            enumerator.MovePrevious();
            return enumerator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return PreOrderTraversal(_root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<T> PreOrderTraversal(Node node)
        {
            if (node != null)
            {
                yield return node.Data;
                foreach (var leftValue in PreOrderTraversal(node.Left))
                {
                    yield return leftValue;
                }
                foreach (var rightValue in PreOrderTraversal(node.Right))
                {
                    yield return rightValue;
                }
            }
        }

        public IEnumerable<T> PostOrderTraversal()
        {
            return PostOrderTraversal(_root);
        }

        private IEnumerable<T> PostOrderTraversal(Node node)
        {
            if (node != null)
            {
                foreach (var leftValue in PostOrderTraversal(node.Left))
                {
                    yield return leftValue;
                }
                foreach (var rightValue in PostOrderTraversal(node.Right))
                {
                    yield return rightValue;
                }
                yield return node.Data;
            }
        }

        public Func<IEnumerator<T>> InOrderTraversal()
        {
            return () =>
            {
                var stack = new Stack<Node>();
                var current = _root;

                IEnumerable<T> Enumerate()
                {
                    while (current != null || stack.Count > 0)
                    {
                        while (current != null)
                        {
                            stack.Push(current);
                            current = current.Left;
                        }

                        current = stack.Pop();
                        yield return current.Data;

                        current = current.Right;
                    }
                }

                return Enumerate().GetEnumerator();
            };
        }
    }

    class Progaram
    {
        static void Main(string[] args)
        {
            BinaryTree<int> tree = new BinaryTree<int>();
            tree.Insert(75);
            tree.Insert(57);
            tree.Insert(90);
            tree.Insert(32);
            tree.Insert(7);
            tree.Insert(44);
            tree.Insert(60);
            tree.Insert(86);
            tree.Insert(93);
            tree.Insert(99);

            Console.WriteLine("Прямой обход дерева (Корень -> Левый потомок -> Правый потомок):");

            foreach (var item in tree)
            {
                Console.Write(item + " ");
            }

            Console.WriteLine("\nОбратный обход дерева (Левый потомок -> Правый потомок -> Корень):");

            foreach (var item in tree.PostOrderTraversal())
            {
                Console.Write(item + " ");
            }

            Func<IEnumerator<int>> iterator = tree.InOrderTraversal();
            IEnumerator<int> enumerator = iterator();

            Console.WriteLine("\nЦентральный обход дерева (Левый потомок -> Корень -> Правый потомок):");

            while (enumerator.MoveNext())
            {
                Console.Write(enumerator.Current + " ");
            }

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
