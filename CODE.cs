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
            private Stack<Node> stack = new Stack<Node>();
            private Node current;

            public BinaryTreeEnumerator(Node root)
            {
                current = root;
                PushLeftNodes();
            }

            private void PushLeftNodes()
            {
                while (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
            }

            public T Current
            {
                get
                {
                    if (stack.Count == 0)
                        return default;
                    return stack.Peek().Value;
                }
            }
            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (stack.Count == 0)
                    return false;

                current = stack.Pop();
                Node node = current.Right;
                while (node != null)
                {
                    stack.Push(node);
                    node = node.Left;
                }
                return true;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public void Dispose() { }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var tree = new BinaryTree<int>();

            tree.Add(75);
            tree.Add(57);
            tree.Add(90);
            tree.Add(32);
            tree.Add(7);
            tree.Add(44);
            tree.Add(60);
            tree.Add(86);
            tree.Add(93);
            tree.Add(99);

            Console.WriteLine("Прямой обход дерева:");

            foreach (var item in tree)
            {
                Console.Write(item + " ");
            }

            Console.WriteLine();

            Console.WriteLine("Обратный обход дерева:");
            TraversalPostOrder(tree.Root);

            Console.WriteLine();
            Console.WriteLine("Центральный обход дерева:");

            foreach (var nodeValue in tree.TraverseInOrder(node => node.Left, node => node.Right))
            {
                Console.Write(nodeValue + " ");
            }

            Console.WriteLine();
            Console.ReadKey();
        }

        static void TraversalPostOrder<T>(BinaryTree<T>.Node node) where T : IComparable
        {
            if (node != null)
            {
                TraversalPostOrder(node.Right);
                Console.Write(node.Value + " ");
                TraversalPostOrder(node.Left);
            }
        }
    }
}
