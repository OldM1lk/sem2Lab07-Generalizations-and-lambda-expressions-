using System;
using System.Collections;
using System.Collections.Generic;

namespace GeneralizationsAndLambdaExspressions
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable
    {
        public enum Side
        {
            Left, Right
        }

        public class Node
        {
            public T Value;
            public Node Left;
            public Node Right;
            public Node Parent;

            public Node(T value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public Node Root;

        public Node Add(T value)
        {
            Node newNode = new Node(value);
            if (Root == null)
            {
                Root = newNode;
            }
            else
            {
                AddRecursively(newNode, Root);
            }
            return newNode;
        }

        private Node AddRecursively(Node newNode, Node currentNode)
        {
            int compareResult = newNode.Value.CompareTo(currentNode.Value);

            if (compareResult < 0)
            {
                if (currentNode.Left == null)
                {
                    currentNode.Left = newNode;
                    newNode.Parent = currentNode;
                }
                else
                {
                    return AddRecursively(newNode, currentNode.Left);
                }
            }
            else if (compareResult > 0)
            {
                if (currentNode.Right == null)
                {
                    currentNode.Right = newNode;
                    newNode.Parent = currentNode;
                }
                else
                {
                    return AddRecursively(newNode, currentNode.Right);
                }
            }
            else
            {
                throw new InvalidOperationException("Узел с таким значением уже существует");
            }
            return newNode;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new BinaryTreeEnumerator(Root);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> TraverseInOrder(Func<Node, Node> getLeft, Func<Node, Node> getRight)
        {
            var stack = new Stack<Node>();
            var current = Root;

            while (current != null || stack.Count > 0)
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = getLeft(current);
                }

                current = stack.Pop();
                yield return current.Value;
                current = getRight(current);
            }
        }

        private class BinaryTreeEnumerator : IEnumerator<T>
        {
            private List<Node> _nodes = new List<Node>();
            private int _index;

            public BinaryTreeEnumerator(Node root)
            {
                PushLeftNodes(root);
            }

            private void PushLeftNodes(Node node)
            {
                while (node != null)
                {
                    _nodes.Add(node);
                    node = node.Left;
                }
            }

            public T Current => _nodes[--_index].Value;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_index == 0)
                {
                    return false;
                }

                Node node = _nodes[--_index];
                PushLeftNodes(node.Right);
                return true;
            }

            public bool MovePrevious()
            {
                if (_index == _nodes.Count)
                {
                    return false;
                }

                Node node = _nodes[++_index];
                PushLeftNodes(node.Left);
                return true;
            }

            public static BinaryTreeEnumerator operator ++(BinaryTreeEnumerator enumerator)
            {
                enumerator.MoveNext();
                return enumerator;
            }

            public static BinaryTreeEnumerator operator --(BinaryTreeEnumerator enumerator)
            {
                enumerator.MovePrevious();
                return enumerator;
            }

            public void Dispose() { }

            public void Reset() { }
        }

        private void PrintTree(Node startNode, string indent = "", Side? side = null)
        {
            if (startNode != null)
            {
                string nodeSide;
                switch (side)
                {
                    case null:
                        nodeSide = "+";
                        break;
                    case Side.Left:
                        nodeSide = "L";
                        break;
                    default:
                        nodeSide = "R";
                        break;
                }

                Console.WriteLine($"{indent} [{nodeSide}] - {startNode.Value}");
                indent += "   ";
                // Рекурсивный вызов для левой и правой ветвей
                PrintTree(startNode.Left, indent, Side.Left);
                PrintTree(startNode.Right, indent, Side.Right);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tree = new BinaryTree<int>();

            tree.Add(5);
            tree.Add(3);
            tree.Add(7);
            tree.Add(2);
            tree.Add(4);
            tree.Add(6);
            tree.Add(8);

            Console.WriteLine("Центральный обход дерева:");

            foreach (var nodeValue in tree.TraverseInOrder(node => node.Left, node => node.Right))
            {
                Console.Write(nodeValue + " ");
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
